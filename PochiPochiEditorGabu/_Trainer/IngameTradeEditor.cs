using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using PochiPochiEditorGabu.Constants;
using PochiPochiEditorGabu.FileReaders;
using PochiPochiEditorGabu.Helpers;
using PochiPochiEditorGabu.Managers;

namespace PochiPochiEditorGabu._Trainer
{
    public partial class IngameTradeEditor : Form
    {
        protected byte[] _romData;
        protected IniFileReader _config;
        protected TblFileReader _tblReader;
        protected ReservationManager _reservationManager;

        private UIStateManager _uiStateManager;

        private EntryManager<TradeDataEntry> _tradeDataManager;

        private EntryManager<PokemonNameEntry> _pokemonNameManager;
        private EntryManager<PokemonIconImageEntry> _iconImgManager;
        private EntryManager<PokemonIconPaletteIndexEntry> _iconPalIdxManager;
        private EntryManager<PokemonIconPaletteAddressEntry> _iconPalAddrManager;
        private EntryManager<ItemSpriteEntry> _itemSpriteManager;
        private EntryManager<ItemDataEntry> _itemDataManager;
        private EntryManager<AbilityNameEntry> _abilityNameManager;
        private EntryManager<PokemonNatureEntry> _natureManager;
        private EntryManager<PokemonStatsNormalEntry> _statsNormalManager;
        private EntryManager<PokemonStatsExpansionEntry> _statsExpansionManager;

        private bool _isUpdatingUI = false;
        private int _currentTradeIdx = 0;
        private bool _isStatsExpanded = false;

        public IngameTradeEditor(
            byte[] romData,
            IniFileReader config,
            TblFileReader tblReader,
            ReservationManager reservationManager)
        {
            InitializeComponent();
            _romData = romData;
            _config = config;
            _tblReader = tblReader;
            _reservationManager = reservationManager;

            InitializeManagers();
            InitializeEventHandlers();
            InitializeControls();
            InitializeUIStates();

            LoadTradeDataToUI(_currentTradeIdx);
        }

        private void InitializeManagers()
        {
            // trainde data
            _tradeDataManager = EntryManager<TradeDataEntry>.Create(
                _romData, _tblReader, _config, "IngameTradeTableAddress", "IngameTradeCount");

            // pokemon name
            _pokemonNameManager = EntryManager<PokemonNameEntry>.Create(
                _romData, _tblReader, _config, "PokemonNameTableAddress", "PokemonNameCount");

            // pokemon icon
            _iconImgManager = EntryManager<PokemonIconImageEntry>.Create(
                _romData, _tblReader, _config, "PokemonIconImageTableAddress", "PokemonIconCount");
            _iconPalIdxManager = EntryManager<PokemonIconPaletteIndexEntry>.Create(
                _romData, _tblReader, _config, "PokemonIconPaletteIndexTableAddress", "PokemonIconCount");
            _iconPalAddrManager = EntryManager<PokemonIconPaletteAddressEntry>.Create(
                _romData, _tblReader, _config, "PokemonIconPaletteAddressTableAddress", "PokemonIconPaletteAddressCount");

            // item sprite
            _itemSpriteManager = EntryManager<ItemSpriteEntry>.Create(
                _romData, _tblReader, _config, "ItemSpriteTableAddress", "ItemDataCount");

            // item name
            _itemDataManager = EntryManager<ItemDataEntry>.Create(
                _romData, _tblReader, _config, "ItemDataTableAddress", "ItemDataCount");

            // ability name
            _abilityNameManager = EntryManager<AbilityNameEntry>.Create(
                _romData, _tblReader, _config, "AbilityNameTableAddress", "AbilityNameCount");

            // nature
            _natureManager = EntryManager<PokemonNatureEntry>.Create(
                _romData, _tblReader, _config, "PokemonNatureTableAddress", "PokemonNatureCount");

            // stats
            _isStatsExpanded = _config.GetBool("IsAppliedCFRU") && _config.GetBool("EnableStatsExpansion");
            if (_isStatsExpanded)
            {
                _statsExpansionManager = EntryManager<PokemonStatsExpansionEntry>.Create(
                    _romData, _tblReader, _config, "PokemonStatsTableAddress", "PokemonStatsCount");
            }
            else
            {
                _statsNormalManager = EntryManager<PokemonStatsNormalEntry>.Create(
                    _romData, _tblReader, _config, "PokemonStatsTableAddress", "PokemonStatsCount");
            }
        }

        private void InitializeEventHandlers()
        {
            btnSave.Click += btnSave_Click;
            this.FormClosing += IngameTradeEditor_FormClosing;

            lstTradeData.SelectedIndexChanged += lstTradeData_SelectedIndexChanged;
            txtDataName.TextChanged += txtDataName_TextChanged;
            txtTrainerName.TextChanged += txtTrainerName_TextChanged;
            rbTrainerGenderMale.CheckedChanged += TrainerGender_CheckedChanged;
            rbTrainerGenderFemale.CheckedChanged += TrainerGender_CheckedChanged;
            txtDataPid.TextChanged += txtDataPid_TextChanged;
            rbDataAbility1.CheckedChanged += DataAbility_CheckChanged;
            rbDataAbility2.CheckedChanged += DataAbility_CheckChanged;

            cmbPokemon1.SelectedIndexChanged += cmbPokemon1_SelectedIndexChanged;
            cmbPokemon2.SelectedIndexChanged += cmbPokemon2_SelectedIndexChanged;
            cmbDataItem.SelectedIndexChanged += cmbDataItem_SelectedIndexChanged;
        }

        private void InitializeControls()
        {
            // cmbDataNature
            cmbDataNature.BeginUpdate();
            cmbDataNature.Items.Clear();
            for (int i = 0; i < _natureManager.Count; i++)
            {
                uint textAddr = _natureManager.Original[i].pTextAddr - GbaConstants.BaseAddr;
                string text = _tblReader.BytesToString(_romData, (int)textAddr, 8);
                cmbDataNature.Items.Add(text);
            }
            cmbDataNature.EndUpdate();

            // lstTradeData
            lstTradeData.BeginUpdate();
            lstTradeData.Items.Clear();
            for (int i = 0; i < _config.GetInt("IngameTradeCount"); i++)
            {
                lstTradeData.Items.Add($"交換データ{i:X2}");
            }
            lstTradeData.EndUpdate();

            if (lstTradeData.Items.Count > 0)
            {
                lstTradeData.SelectedIndex = 0;
            }

            // pokemon for cmb
            var pokemonNames = _pokemonNameManager.Original
                 .Select(entry => entry._PokemonName)
                 .ToArray();
            cmbPokemon1.Items.AddRange(pokemonNames);
            cmbPokemon2.Items.AddRange(pokemonNames);

            // item for cmb
            var itemNames = _itemDataManager.Original
                             .Select(entry => entry._ItemName)
                             .ToArray();
            cmbDataItem.Items.AddRange(itemNames);

            // ability for cmb
            var abilityNames = _abilityNameManager.Original
                             .Select(entry => entry._AbilityName)
                             .ToArray();
            cmbDataAbility.Items.AddRange(abilityNames);

            ControlHelper.AttachAddressAutoFormat(
                txtDataPidHex);
            ControlHelper.AttachExternalBorder(
                picPokemon1, picPokemon2, picDataItem);
        }

        private void InitializeUIStates()
        {
            btnSave.Enabled = false;
            _uiStateManager = new UIStateManager(hasChanges => btnSave.Enabled = hasChanges);

            _uiStateManager.AddControls(
                txtTrainerName, nudTrainerIdx1, nudTrainerIdx2,
                txtDataName, cmbDataItem, nudDataMailIdx,
                txtDataPid);
            _uiStateManager.AddControlsRecursive(
                grpUnknownValues,
                grpPokemon1, grpPokemon2,
                grpDataIv, grpDataCondiSheen);
            _uiStateManager.AddRadioButtons(
                new[] { rbTrainerGenderMale, rbTrainerGenderFemale },
                new[] { rbDataAbility1, rbDataAbility2 });
        }

        private void LoadTradeDataToUI(int idx)
        {
            _isUpdatingUI = true;
            _currentTradeIdx = idx;

            DataBindingHelper.BindObjectToControls(this, _tradeDataManager.Original[idx]);

            // Load data name
            txtDataName.Text = _tradeDataManager.Original[idx]._DataName;
            // Load trainer name
            txtTrainerName.Text = _tradeDataManager.Original[idx]._TrainerName;

            // trainer gender
            switch (_tradeDataManager.Original[idx]._TrainerGender)
            {
                case 0x0:
                    rbTrainerGenderMale.Checked = true;
                    break;
                case 0x1:
                    rbTrainerGenderFemale.Checked = true;
                    break;
            }

            // sprite
            UpdatePokemonIcon(cmbPokemon1, picPokemon1);
            UpdatePokemonIcon(cmbPokemon2, picPokemon2);
            UpdateItemSprite(cmbDataItem, picDataItem);

            //pid
            int decValue = Convert.ToInt32(txtDataPidHex.Text, 16);
            txtDataPid.Text = decValue.ToString();

            // ability fix
            switch (_tradeDataManager.Original[idx]._DataAbilityValue)
            {
                case 0x0:
                    rbDataAbility1.Checked = true;
                    break;
                case 0x1:
                    rbDataAbility2.Checked = true;
                    break;
            }

            UpdataCalcPID();

            _isUpdatingUI = false;
            _uiStateManager.UpdateInitialValues();
        }

        private void lstTradeData_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_isUpdatingUI) return;

            int newIndex = lstTradeData.SelectedIndex;
            if (newIndex == _currentTradeIdx) return;

            if (btnSave.Enabled)
            {
                ControlHelper.HandleUnsavedChanges(
                    saveAction: () =>
                    {
                        SaveCurrentData(_currentTradeIdx);
                        LoadTradeDataToUI(newIndex);
                    },
                    discardAction: () =>
                    {
                        LoadTradeDataToUI(newIndex);
                    },
                    cancelAction: () =>
                    {
                        lstTradeData.SelectedIndex = _currentTradeIdx;
                    }

                );
            }
            else
            {
                LoadTradeDataToUI(newIndex);
            }
        }

        private void txtDataName_TextChanged(object sender, EventArgs e)
        {
            if (_isUpdatingUI) return;

            int dataNameMaxLength = _config.GetInt("IngameTradePokemonNameMaxLength");
            int maxAllowedBytes = dataNameMaxLength - 1;
            string currentText = txtDataName.Text;
            byte[] currentBytes = _tblReader.StringToBytes(currentText, false);

            if (currentBytes.Length > maxAllowedBytes)
            {
                _isUpdatingUI = true;

                while (currentText.Length > 0)
                {
                    currentBytes = _tblReader.StringToBytes(currentText, false);
                    if (currentBytes.Length <= maxAllowedBytes) break;

                    currentText = currentText.Substring(0, currentText.Length - 1);
                }

                int savedSelectionStart = txtDataName.SelectionStart;
                txtDataName.Text = currentText;
                txtDataName.SelectionStart = Math.Min(savedSelectionStart, currentText.Length);

                _isUpdatingUI = false;
            }

            string validName = _tblReader.BytesToString(currentBytes, 0, currentBytes.Length);

            // direct
            _tradeDataManager.Working[_currentTradeIdx]._DataName = validName;
        }

        private void txtTrainerName_TextChanged(object sender, EventArgs e)
        {
            if (_isUpdatingUI) return;

            int trainerNameMaxLength = _config.GetInt("IngameTradeTrainerNameMaxLength");
            int maxAllowedBytes = trainerNameMaxLength - 1;
            string currentText = txtTrainerName.Text;
            byte[] currentBytes = _tblReader.StringToBytes(currentText, false);

            if (currentBytes.Length > maxAllowedBytes)
            {
                _isUpdatingUI = true;

                while (currentText.Length > 0)
                {
                    currentBytes = _tblReader.StringToBytes(currentText, false);
                    if (currentBytes.Length <= maxAllowedBytes) break;

                    currentText = currentText.Substring(0, currentText.Length - 1);
                }

                int savedSelectionStart = txtTrainerName.SelectionStart;
                txtTrainerName.Text = currentText;
                txtTrainerName.SelectionStart = Math.Min(savedSelectionStart, currentText.Length);

                _isUpdatingUI = false;
            }

            string validName = _tblReader.BytesToString(currentBytes, 0, currentBytes.Length);

            // direct
            _tradeDataManager.Working[_currentTradeIdx]._TrainerName = validName;
        }

        private void TrainerGender_CheckedChanged(object sender, EventArgs e)
        {
            // direct
            if (rbTrainerGenderMale.Checked)
            {
                _tradeDataManager.Working[_currentTradeIdx]._TrainerGender = 0x0;
            }
            else if (rbTrainerGenderFemale.Checked)
            {
                _tradeDataManager.Working[_currentTradeIdx]._TrainerGender = 0x1;
            }
        }

        private void cmbPokemon1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_isUpdatingUI) return;
            UpdatePokemonIcon(cmbPokemon1, picPokemon1);
        }

        private void cmbPokemon2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_isUpdatingUI) return;
            UpdatePokemonIcon(cmbPokemon2, picPokemon2);
            UpdataCalcPID();
        }

        private void cmbDataItem_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_isUpdatingUI) return;
            UpdateItemSprite(cmbDataItem, picDataItem);
        }

        private void UpdatePokemonIcon(ComboBox cmb, PictureBox pic)
        {
            Bitmap icon = null;

            if (cmb.SelectedIndex >= 0 && cmb.SelectedIndex < cmb.Items.Count)
            {
                int idx = cmb.SelectedIndex;
                icon = GetPokemonIcon(idx, true);
            }

            pic.Image?.Dispose();
            pic.Image = null;
            pic.Image = icon;
        }

        private void UpdateItemSprite(ComboBox cmb, PictureBox pic)
        {
            Bitmap icon = null;

            if (cmb.SelectedIndex >= 0 && cmb.SelectedIndex < cmb.Items.Count)
            {
                int idx = cmb.SelectedIndex;
                icon = GetItemSprite(idx, true);
            }

            pic.Image?.Dispose();
            pic.Image = null;
            pic.Image = icon;
        }

        private void txtDataPid_TextChanged(object sender, EventArgs e)
        {
            if (_isUpdatingUI) return;
            UpdataCalcPID();
        }

        private void DataAbility_CheckChanged(object sender, EventArgs e)
        {
            if (_isUpdatingUI) return;

            if (rbDataAbility1.Checked)
            {
                _tradeDataManager.Working[_currentTradeIdx]._DataAbilityValue = 0x00;
            }
            else if (rbDataAbility2.Checked)
            {
                _tradeDataManager.Working[_currentTradeIdx]._DataAbilityValue = 0x01;
            }

            UpdataCalcPID();
        }

        private void UpdataCalcPID()
        {
            string newValueStr = txtDataPid.Text.Trim();

            if (uint.TryParse(newValueStr, out uint newValue))
            {
                txtDataPidHex.Text = newValue.ToString("X8");
                CalculateNature(newValue);
                CalculateGender(newValue);
                CalculateAbility(newValue);
            }
            else
            {
                ControlHelper.ResetControls(grpDataPid, new[] { "txtDataPid" });
            }
        }

        private void CalculateNature(uint newValue)
        {
            int natureIndex = (int)(newValue % _config.GetInt("PokemonNatureCount"));
            cmbDataNature.SelectedIndex = natureIndex;
        }

        private void CalculateGender(uint newValue)
        {
            int pokemonIndex = cmbPokemon2.SelectedIndex;
            byte genderValue;

            if (_isStatsExpanded)
            {
                genderValue = _statsExpansionManager.Original[pokemonIndex].StatsGender;
            }
            else
            {
                genderValue = _statsNormalManager.Original[pokemonIndex].StatsGender;
            }

            switch (genderValue)
            {
                case 0x0: // only male
                    rbDataGenderMale.Checked = true;
                    return;

                case 0xFE: // only female
                    rbDataGenderFemale.Checked = true;
                    return;

                case 0xFF: // unknown gender
                    rbDataGenderUnknown.Checked = true;
                    return;
            }

            // try calc
            byte natureValue = (byte)(newValue & GbaConstants.Mask8Bits);

            if (natureValue < genderValue)
            {
                rbDataGenderFemale.Checked = true;
            }
            else
            {
                rbDataGenderMale.Checked = true;
            }
        }

        private void CalculateAbility(uint newValue)
        {
            // eventually dispose
            int pokemonIndex = cmbPokemon2.SelectedIndex;
            int ability1Id;
            int ability2Id;

            if (_isStatsExpanded)
            {
                ability1Id = _statsExpansionManager.Original[pokemonIndex].StatsAbility1;
                ability2Id = _statsExpansionManager.Original[pokemonIndex].StatsAbility2;
            }
            else
            {
                ability1Id = _statsNormalManager.Original[pokemonIndex].StatsAbility1;
                ability2Id = _statsNormalManager.Original[pokemonIndex].StatsAbility2;
            }

            int abilityIndex = ability1Id; // default

            if ((ability2Id != -1) && ((newValue & 1) == 1))
            {
                abilityIndex = ability2Id;
            }

            cmbDataAbility.SelectedIndex = abilityIndex;

            // ?
            OverrideAbility();
        }

        private void OverrideAbility()
        {
            int abilityValue = _tradeDataManager.Working[_currentTradeIdx]._DataAbilityValue;

            int pokemonIndex = cmbPokemon2.SelectedIndex;
            int ability1Id;
            int ability2Id;

            if (_isStatsExpanded)
            {
                ability1Id = _statsExpansionManager.Original[pokemonIndex].StatsAbility1;
                ability2Id = _statsExpansionManager.Original[pokemonIndex].StatsAbility2;
            }
            else
            {
                ability1Id = _statsNormalManager.Original[pokemonIndex].StatsAbility1;
                ability2Id = _statsNormalManager.Original[pokemonIndex].StatsAbility2;
            }

            int abilityCount = cmbDataAbility.Items.Count;
            int selectedIndex = -1;

            switch (abilityValue)
            {
                case 0x0:
                    if (ability1Id < abilityCount)
                    {
                        selectedIndex = ability1Id;
                    }
                    break;

                case 0x1:
                    if (ability2Id != 0 && ability2Id < abilityCount)
                    {
                        selectedIndex = ability2Id;
                    }
                    else if (ability1Id < abilityCount)
                    {
                        selectedIndex = ability1Id;
                    }
                    break;
            }

            if (selectedIndex >= 0)
            {
                cmbDataAbility.SelectedIndex = selectedIndex;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveCurrentData(_currentTradeIdx);
            _uiStateManager.UpdateInitialValues();
        }

        private void IngameTradeEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (btnSave.Enabled)
            {
                ControlHelper.HandleUnsavedChanges(
                    saveAction: () =>
                    {
                        SaveCurrentData(_currentTradeIdx);
                    },
                    discardAction: () =>
                    {
                        // unnecessary
                    },
                    cancelAction: () =>
                    {
                        e.Cancel = true;
                    }
                );
            }
        }

        private void SaveCurrentData(int idx)
        {
            DataBindingHelper.BindControlsToObject(this, _tradeDataManager.Working[idx]);
            _tradeDataManager.Save(idx, true, GbaConstants.PaddingByte, GbaConstants.PaddingByte);
        }

        private Bitmap GetPokemonIcon(int idx, bool showBackColor)
        {
            uint? imageAddress = _iconImgManager.Original[idx].pIconImgAddr - GbaConstants.BaseAddr;
            if (!imageAddress.HasValue) return null;

            int palIndex = _iconPalIdxManager.Original[idx].IconPalIdx;
            var entry = _iconPalAddrManager.Working[palIndex];
            uint palettePtr = entry._IconPaletteAddr;
            if (palettePtr == 0) return null;
            uint paletteAddress = palettePtr - GbaConstants.BaseAddr;

            try
            {
                byte[] image = new byte[GbaConstants.IconBytesPerFrame];
                Array.Copy(_romData, (int)imageAddress.Value, image, 0, GbaConstants.IconBytesPerFrame);
                Color[] palette = ImageManager.DecompressPalette(_romData, paletteAddress, false);
                return ImageManager.CreateSprite(
                    image,
                    palette,
                    GbaConstants.IconFrameSize,
                    GbaConstants.IconFrameSize,
                    showBackColor);
            }
            catch
            {
                return null;
            }
        }

        private Bitmap GetItemSprite(int idx, bool showBackColor)
        {
            uint? imgAddr = _itemSpriteManager.Original[idx].pSpriteImgAddr - GbaConstants.BaseAddr;
            uint? palAddr = _itemSpriteManager.Original[idx].pSpritePalAddr - GbaConstants.BaseAddr;

            if (!imgAddr.HasValue || !palAddr.HasValue) return null;

            try
            {
                byte[] image = ImageManager.DecompressLZ77(_romData, imgAddr.Value);
                Color[] palette = ImageManager.DecompressPalette(_romData, palAddr.Value, true);
                return ImageManager.CreateSprite(
                    image,
                    palette,
                    GbaConstants.ItemSpriteSize,
                    GbaConstants.ItemSpriteSize,
                    showBackColor);
            }
            catch
            {
                return null;
            }
        }
    }
}
