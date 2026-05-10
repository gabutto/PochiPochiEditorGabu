using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using PochiPochiEditorGabu.Constants;
using PochiPochiEditorGabu.FileReaders;
using PochiPochiEditorGabu.Helpers;
using PochiPochiEditorGabu.Managers;

namespace PochiPochiEditorGabu._Trainer
{
    public partial class TrainerListEditor : Form
    {
        protected byte[] _romData;
        protected IniFileReader _config;
        protected TblFileReader _tblReader;
        protected ReservationManager _reservationManager;

        private UIStateManager _uiStateManager;

        private EntryManager<TrainerListEntry> _trainerListManager;

        private EntryManager<TrainerClassNameEntry> _classNameManager;
        private EntryManager<ItemSpriteEntry> _itemSpriteManager;
        private EntryManager<ItemDataEntry> _itemDataManager;
        private EntryManager<PokemonNameEntry> _pokemonNameManager;
        private EntryManager<MoveNameEntry> _moveNameManager;
        private EntryManager<TrainerSpriteImageEntry> _trainerImgManager;
        private EntryManager<TrainerSpritePaletteEntry> _trainerPalManager;
        private EntryManager<PokemonIconImageEntry> _iconImgManager;
        private EntryManager<PokemonIconPaletteIndexEntry> _iconPalIdxManager;
        private EntryManager<PokemonIconPaletteAddressEntry> _iconPalAddrManager;

        private bool _isUpdatingUI = false;
        private int _currentTrainerIdx = 0;

        private IList _currentPartyEntries;
        private const string PartyBinaryKey = "PartyBinary";

        public TrainerListEditor(
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

            LoadTrainerDataToUI(_currentTrainerIdx);
        }
        private void InitializeManagers()
        {
            // list
            _trainerListManager = EntryManager<TrainerListEntry>.Create(
                _romData, _tblReader, _config, "TrainerListTableAddress", "TrainerListEntryCount");

            // class
            _classNameManager = EntryManager<TrainerClassNameEntry>.Create(
                _romData, _tblReader, _config, "TrainerClassNameTableAddress", "TrainerClassNameCount");

            // item sprite
            _itemSpriteManager = EntryManager<ItemSpriteEntry>.Create(
                _romData, _tblReader, _config, "ItemSpriteTableAddress", "ItemDataCount");

            // item name
            _itemDataManager = EntryManager<ItemDataEntry>.Create(
                _romData, _tblReader, _config, "ItemDataTableAddress", "ItemDataCount");

            // pokemon name
            _pokemonNameManager = EntryManager<PokemonNameEntry>.Create(
                _romData, _tblReader, _config, "PokemonNameTableAddress", "PokemonNameCount");

            // move name
            _moveNameManager = EntryManager<MoveNameEntry>.Create(
                _romData, _tblReader, _config, "MoveNameTableAddress", "MoveNameCount");

            // trainer img
            _trainerImgManager = EntryManager<TrainerSpriteImageEntry>.Create(
                _romData, _tblReader, _config, "TrainerSpriteImageTableAddress", "TrainerSpriteCount");

            //trainer pal
            _trainerPalManager = EntryManager<TrainerSpritePaletteEntry>.Create(
                _romData, _tblReader, _config, "TrainerSpritePaletteTableAddress", "TrainerSpriteCount");

            // icon
            _iconImgManager = EntryManager<PokemonIconImageEntry>.Create(
                _romData, _tblReader, _config, "PokemonIconImageTableAddress", "PokemonIconCount");
            _iconPalIdxManager = EntryManager<PokemonIconPaletteIndexEntry>.Create(
                _romData, _tblReader, _config, "PokemonIconPaletteIndexTableAddress", "PokemonIconCount");
            _iconPalAddrManager = EntryManager<PokemonIconPaletteAddressEntry>.Create(
                _romData, _tblReader, _config, "PokemonIconPaletteAddressTableAddress", "PokemonIconPaletteAddressCount");
        }

        private void InitializeEventHandlers()
        {
            btnSave.Click += btnSave_Click;
            this.FormClosing += PokemonEditor_FormClosing;

            lstTrainerData.SelectedIndexChanged += lstTrainerData_SelectedIndexChanged;
            txtName.TextChanged += txtName_TextChanged;
            nudSpriteIdx.ValueChanged += nudTrainerSpriteIdx_ValueChanged;
            cmbPartyPokemon.SelectedIndexChanged += cmbPartyPokemon_SelectedIndexChanged;
            foreach (var cmb in new[] {
                cmbHoldItem1,
                cmbHoldItem2,
                cmbHoldItem3,
                cmbHoldItem4,
                cmbPartyItem})
            {
                cmb.SelectedIndexChanged += ItemComboBox_SelectedIndexChanged;
            }

            cmbPartyData.SelectedIndexChanged += cmbPartyData_SelectedIndexChanged;

            EventHandler partyDataChangedHandler = (sender, e) =>
            {
                if (_isUpdatingUI) return;
                UpdateCurrentPartyEntryFromUI();
            };

            cmbPartyPokemon.SelectedIndexChanged += partyDataChangedHandler;
            nudPartyLevel.ValueChanged += partyDataChangedHandler;
            nudPartyIv.ValueChanged += partyDataChangedHandler;
            cmbPartyEv.SelectedIndexChanged += partyDataChangedHandler;
            cmbPartyItem.SelectedIndexChanged += partyDataChangedHandler;
            cmbPartyMove1.SelectedIndexChanged += partyDataChangedHandler;
            cmbPartyMove2.SelectedIndexChanged += partyDataChangedHandler;
            cmbPartyMove3.SelectedIndexChanged += partyDataChangedHandler;
            cmbPartyMove4.SelectedIndexChanged += partyDataChangedHandler;
        }

        private void InitializeControls()
        {
            int spriteCount = _config.GetInt("TrainerSpriteCount");
            nudSpriteIdx.Maximum = spriteCount - 1;

            // lstTrainerData
            lstTrainerData.Items.Clear();
            for (int i = 0; i < _trainerListManager.Count; i++)
            {
                string name = _trainerListManager.Working[i]._Name;
                lstTrainerData.Items.Add($"{i:X4} - {name}");
            }

            if (lstTrainerData.Items.Count > 0)
            {
                lstTrainerData.SelectedIndex = 0;
            }

            // cmbClassIdx
            var classNames = _classNameManager.Original
                             .Select(entry => entry._ClassName)
                             .ToArray();
            cmbClassIdx.Items.AddRange(classNames);

            // item for cmb
            var itemNames = _itemDataManager.Original
                             .Select(entry => entry._ItemName)
                             .ToArray();
            cmbHoldItem1.Items.AddRange(itemNames);
            cmbHoldItem2.Items.AddRange(itemNames);
            cmbHoldItem3.Items.AddRange(itemNames);
            cmbHoldItem4.Items.AddRange(itemNames);
            cmbPartyItem.Items.AddRange(itemNames);

            // pokemon name for cmb
            var pokemonNames = _pokemonNameManager.Original
                             .Select(entry => entry._PokemonName)
                             .ToArray();
            cmbPartyPokemon.Items.AddRange(pokemonNames);

            // move for cmb
            var moveNames = _moveNameManager.Original
                             .Select(entry => entry._MoveName)
                             .ToArray();
            cmbPartyMove1.Items.AddRange(moveNames);
            cmbPartyMove2.Items.AddRange(moveNames);
            cmbPartyMove3.Items.AddRange(moveNames);
            cmbPartyMove4.Items.AddRange(moveNames);

            ControlHelper.AttachAddressAutoFormat(txtPartyAddr);
            ControlHelper.AttachExternalBorder(
                picTrainerSprite,
                picHoldItem1, picHoldItem2, picHoldItem3, picHoldItem4,
                picPartyPokemon, picPartyItem);
            ControlHelper.LoadComboBoxFromTextFile(cmbDataType, "txt/TrainerDataType.txt");
            ControlHelper.LoadComboBoxFromTextFile(cmbPartyEv, "txt/TrainerEvTable.txt");
        }

        private void InitializeUIStates()
        {
            btnSave.Enabled = false;
            _uiStateManager = new UIStateManager(hasChanges => btnSave.Enabled = hasChanges);
            _uiStateManager.AddControlsRecursive(grpTrainerData);
        }

        private void LoadTrainerDataToUI(int idx)
        {
            _isUpdatingUI = true;
            _reservationManager.ClearAllReservations();

            _currentTrainerIdx = idx;

            DataBindingHelper.BindObjectToControls(this, _trainerListManager.Original[idx]);

            // name
            txtName.Text = _trainerListManager.Original[idx]._Name;

            // party data
            LoadPartyData(idx);

            UpdateTrainerSprite();
            UpdatePokemonIcon();
            UpdateItemImages();

            _isUpdatingUI = false;
            _uiStateManager.UpdateInitialValues();
        }

        private void lstTrainerData_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_isUpdatingUI) return;

            int newIndex = lstTrainerData.SelectedIndex;
            if (newIndex == _currentTrainerIdx) return;

            if (btnSave.Enabled)
            {
                ControlHelper.HandleUnsavedChanges(
                    () =>
                    {
                        SaveCurrentData(_currentTrainerIdx);
                        LoadTrainerDataToUI(newIndex);
                    },
                    () =>
                    {
                        RestoreData(_currentTrainerIdx);
                        LoadTrainerDataToUI(newIndex);
                    },
                    () =>
                    {
                        lstTrainerData.SelectedIndex = _currentTrainerIdx;
                    }

                );
            }
            else
            {
                LoadTrainerDataToUI(newIndex);
            }
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            if (_isUpdatingUI) return;

            int nameEntryLength = _config.GetInt("TrainerNameEntryLength");
            int maxAllowedBytes = nameEntryLength - 1;
            string currentText = txtName.Text;
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

                int savedSelectionStart = txtName.SelectionStart;
                txtName.Text = currentText;
                txtName.SelectionStart = Math.Min(savedSelectionStart, currentText.Length);

                _isUpdatingUI = false;
            }

            string validName = _tblReader.BytesToString(currentBytes, 0, currentBytes.Length);

            _isUpdatingUI = true;
            // direct
            _trainerListManager.Working[_currentTrainerIdx]._Name = validName;
            lstTrainerData.Items[_currentTrainerIdx] = $"{_currentTrainerIdx:X4} - {validName}";
            _isUpdatingUI = false;
        }

        private void LoadPartyData(int idx)
        {
            uint? partyAddrOffset = _trainerListManager.Original[idx].pPartyAddr;
            uint? actualPartyAddr = 
                (partyAddrOffset == 0) 
                ? null 
                : partyAddrOffset - GbaConstants.BaseAddr;
            int partyCount = _trainerListManager.Original[idx].PartyCount;
            byte dataType = _trainerListManager.Original[idx].DataType;

            // iv or ev?
            bool isCFRU = _config.GetBool("IsAppliedCFRU");
            bool enableTrainerEV = _config.GetBool("EnableTrainerEV");
            bool isEvMode = isCFRU && enableTrainerEV;

            rbPartyEv.Checked = isEvMode;
            rbPartyIv.Checked = !isEvMode;
            rbPartyEv.Enabled = isEvMode;
            cmbPartyEv.Enabled = isEvMode;
            rbPartyIv.Enabled = !isEvMode;
            nudPartyIv.Enabled = !isEvMode;

            _currentPartyEntries = new List<object>();

            if (actualPartyAddr.HasValue && partyCount > 0)
            {
                switch (dataType)
                {
                    case 0:
                        _currentPartyEntries = IoHelper.ReadStructures<TrainerPartyEntry00>(
                            _romData, 
                            actualPartyAddr, 
                            partyCount, 
                            _tblReader).Cast<object>().ToList();
                        break;
                    case 1:
                        _currentPartyEntries = IoHelper.ReadStructures<TrainerPartyEntry01>(
                            _romData, 
                            actualPartyAddr, 
                            partyCount, 
                            _tblReader).Cast<object>().ToList();
                        break;
                    case 2:
                        _currentPartyEntries = IoHelper.ReadStructures<TrainerPartyEntry02>(
                            _romData, 
                            actualPartyAddr, 
                            partyCount, 
                            _tblReader).Cast<object>().ToList();
                        break;
                    case 3:
                        _currentPartyEntries = IoHelper.ReadStructures<TrainerPartyEntry03>(
                            _romData, 
                            actualPartyAddr, 
                            partyCount,
                            _tblReader).Cast<object>().ToList();
                        break;
                }
            }

            UpdatePartyBinaryState(true);

            _isUpdatingUI = true;
            cmbPartyData.Items.Clear();
            if (partyCount > 0)
            {
                for (int i = 0; i < partyCount; i++)
                {
                    cmbPartyData.Items.Add($"{i + 1}体目");
                }
                cmbPartyData.SelectedIndex = 0;
                UpdatePartyUIForIndex(0);
            }
            else
            {
                ClearPartyUI();
            }
            _isUpdatingUI = false;
        }

        private void UpdatePartyUIForIndex(int pIdx)
        {
            if (_currentPartyEntries == null || pIdx < 0 || pIdx >= _currentPartyEntries.Count) return;

            object entry = _currentPartyEntries[pIdx];
            DataBindingHelper.BindObjectToControls(grpPartyData, entry);

            // _PartyIvOrEv
            ushort ivOrEv = 0;
            if (entry is TrainerPartyEntry00 p00) ivOrEv = p00._PartyIvOrEv;
            else if (entry is TrainerPartyEntry01 p01) ivOrEv = p01._PartyIvOrEv;
            else if (entry is TrainerPartyEntry02 p02) ivOrEv = p02._PartyIvOrEv;
            else if (entry is TrainerPartyEntry03 p03) ivOrEv = p03._PartyIvOrEv;

            if (rbPartyEv.Checked)
            {
                cmbPartyEv.SelectedIndex = ivOrEv < cmbPartyEv.Items.Count ? ivOrEv : -1;
                nudPartyIv.Value = Math.Max(nudPartyIv.Minimum, 0);
            }
            else
            {
                nudPartyIv.Value = Math.Min(ivOrEv, nudPartyIv.Maximum);
                cmbPartyEv.SelectedIndex = -1;
            }

            byte dataType = _trainerListManager.Working[_currentTrainerIdx].DataType;
            UpdatePartyUIByDataType(dataType);
            UpdatePokemonIcon();
            UpdateItemImages();
        }

        private void ClearPartyUI()
        {
            DisableAndResetControl(grpPartyData);
            cmbPartyData.Enabled = false;
        }

        private void DisableAndResetControl(Control target)
        {
            target.Enabled = false;

            if (target is ComboBox cmb) cmb.SelectedIndex = -1;
            else if (target is NumericUpDown nud) nud.Value = Math.Max(nud.Minimum, 0);
            else if (target is TextBox txt) txt.Text = string.Empty;
            else if (target is CheckBox chk) chk.Checked = false;
            else if (target is RadioButton rb) rb.Checked = false;

            target.ResetControls();
        }

        private void UpdatePartyUIByDataType(byte dataType)
        {
            grpPartyData.Enabled = true;
            grpPartyData.SetControlsEnabled(true);

            cmbPartyData.Enabled = true;

            bool isEvMode = _config.GetBool("IsAppliedCFRU") && _config.GetBool("EnableTrainerEV");
            rbPartyEv.Enabled = isEvMode;
            cmbPartyEv.Enabled = isEvMode;
            rbPartyIv.Enabled = !isEvMode;
            nudPartyIv.Enabled = !isEvMode;

            if (dataType == 0) // no item, no move
            {
                DisableAndResetControl(cmbPartyItem);
                DisableAndResetControl(grpPartyMoves);
            }
            else if (dataType == 1) // no item, has move
            {
                DisableAndResetControl(cmbPartyItem);
            }
            else if (dataType == 2) // has item、no move
            {
                DisableAndResetControl(grpPartyMoves);
            }
            else if (dataType == 3) // has item、has move
            {
                //
            }
        }

        private void UpdatePartyBinaryState(bool isInitial)
        {
            byte[] partyBinary = null;

            if (_currentPartyEntries != null && _currentPartyEntries.Count > 0)
            {
                Type entryType = _currentPartyEntries[0].GetType();
                int entrySize = GetPartyEntrySize(entryType);
                int totalSize = entrySize * _currentPartyEntries.Count;
                partyBinary = new byte[totalSize];

                if (entryType == typeof(TrainerPartyEntry00))
                {
                    IoHelper.WriteStructures(
                        partyBinary, 
                        0, 
                        _currentPartyEntries.Cast<TrainerPartyEntry00>(), 
                        _tblReader, null, false);
                }
                else if (entryType == typeof(TrainerPartyEntry01))
                {
                    IoHelper.WriteStructures(
                        partyBinary, 
                        0, 
                        _currentPartyEntries.Cast<TrainerPartyEntry01>(),
                        _tblReader, null, false);
                }
                else if (entryType == typeof(TrainerPartyEntry02))
                {
                    IoHelper.WriteStructures(
                        partyBinary,
                        0, 
                        _currentPartyEntries.Cast<TrainerPartyEntry02>(),
                        _tblReader, null, false);
                }
                else if (entryType == typeof(TrainerPartyEntry03))
                {
                    IoHelper.WriteStructures(
                        partyBinary, 
                        0, 
                        _currentPartyEntries.Cast<TrainerPartyEntry03>(), 
                        _tblReader, null, false);
                }
            }

            if (isInitial)
            {
                _uiStateManager.AddBinaries((PartyBinaryKey, partyBinary));
            }

            _uiStateManager.UpdateBinary(PartyBinaryKey, partyBinary);
        }

        private int GetPartyEntrySize(Type type)
        {
            int size = 0;
            var fields = type.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            foreach (var field in fields)
            {
                if (field.FieldType.IsValueType)
                {
                    size += Marshal.SizeOf(field.FieldType);
                }
            }
            return size;
        }

        private void UpdateCurrentPartyEntryFromUI()
        {
            int pIdx = cmbPartyData.SelectedIndex;
            if (pIdx < 0 || _currentPartyEntries == null || pIdx >= _currentPartyEntries.Count) return;

            object entry = _currentPartyEntries[pIdx];
            DataBindingHelper.BindControlsToObject(grpPartyData, entry);

            // _PartyIvOrEv
            ushort ivOrEv = rbPartyEv.Checked
                ? (ushort)Math.Max(0, cmbPartyEv.SelectedIndex)
                : (ushort)nudPartyIv.Value;

            if (entry is TrainerPartyEntry00 p00)
            {
                p00._PartyIvOrEv = ivOrEv;
            }
            else if (entry is TrainerPartyEntry01 p01)
            {
                p01._PartyIvOrEv = ivOrEv;
            }
            else if (entry is TrainerPartyEntry02 p02)
            {
                p02._PartyIvOrEv = ivOrEv;
            }
            else if (entry is TrainerPartyEntry03 p03)
            {
                p03._PartyIvOrEv = ivOrEv;
            }

            UpdatePartyBinaryState(false);
        }

        private void cmbPartyData_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_isUpdatingUI) return;
            _isUpdatingUI = true;
            UpdatePartyUIForIndex(cmbPartyData.SelectedIndex);
            _isUpdatingUI = false;
        }

        private void ItemComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_isUpdatingUI) return;
            UpdateItemImages();
        }

        private void UpdateItemImages()
        {
            var itemControls = new[]
            {
                (Combo: cmbHoldItem1, Pic: picHoldItem1),
                (Combo: cmbHoldItem2, Pic: picHoldItem2),
                (Combo: cmbHoldItem3, Pic: picHoldItem3),
                (Combo: cmbHoldItem4, Pic: picHoldItem4),
                (Combo: cmbPartyItem, Pic: picPartyItem)
            };

            foreach (var item in itemControls)
            {
                Bitmap sprite = null;

                if (item.Combo.SelectedIndex >= 0 && item.Combo.SelectedIndex < item.Combo.Items.Count)
                {
                    int itemId = item.Combo.SelectedIndex;
                    sprite = GetItemSprite(itemId, true);
                }

                item.Pic.Image?.Dispose();
                item.Pic.Image = null;
                item.Pic.Image = sprite;
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

        private void UpdateTrainerSprite()
        {
            Bitmap sprite = null;

            if (nudSpriteIdx.Value >= 0 && nudSpriteIdx.Value <= nudSpriteIdx.Maximum)
            {
                int idx = (int)nudSpriteIdx.Value;
                sprite = GetTrainerSprite(idx, true);
            }

            picTrainerSprite.Image?.Dispose();
            picTrainerSprite.Image = null;
            picTrainerSprite.Image = sprite;
        }

        private Bitmap GetTrainerSprite(int idx, bool showBackColor)
        {
            uint? imgAddr = _trainerImgManager.Original[idx].pSpriteImgAddr - GbaConstants.BaseAddr;
            uint? palAddr = _trainerPalManager.Original[idx].pSpritePalAddr - GbaConstants.BaseAddr;

            if (!imgAddr.HasValue || !palAddr.HasValue) return null;

            try
            {
                byte[] image = ImageManager.DecompressLZ77(_romData, imgAddr.Value);
                Color[] palette = ImageManager.DecompressPalette(_romData, palAddr.Value, true);
                return ImageManager.CreateSprite(
                    image,
                    palette,
                    GbaConstants.SpriteSize,
                    GbaConstants.SpriteSize,
                    showBackColor);
            }
            catch
            {
                return null;
            }
        }

        private void nudTrainerSpriteIdx_ValueChanged(object sender, EventArgs e)
        {
            if (_isUpdatingUI) return;
            UpdateTrainerSprite();
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

        private void UpdatePokemonIcon()
        {
            Bitmap sprite = null;

            if (cmbPartyPokemon.SelectedIndex >= 0 && cmbPartyPokemon.SelectedIndex <= cmbPartyPokemon.Items.Count)
            {
                int idx = cmbPartyPokemon.SelectedIndex;
                sprite = GetPokemonIcon(idx, true);
            }

            picPartyPokemon.Image?.Dispose();
            picPartyPokemon.Image = null;
            picPartyPokemon.Image = sprite;
        }

        private void cmbPartyPokemon_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_isUpdatingUI) return;
            UpdatePokemonIcon();
        }

        private void RestoreData(int idx)
        {
            // name
            string name = _trainerListManager.Original[idx]._Name;
            lstTrainerData.Items[_currentTrainerIdx] = $"{_currentTrainerIdx:X4} - {name}";
        }

        private void SaveCurrentData(int idx)
        {
            // trainer including name
            _trainerListManager.Save(idx);

            // party data
            if (_currentPartyEntries != null && _currentPartyEntries.Count > 0)
            {
                uint? partyAddrOffset = _trainerListManager.Working[idx].pPartyAddr;
                if (partyAddrOffset.HasValue && partyAddrOffset.Value != 0)
                {
                    int actualPartyAddr = (int)(partyAddrOffset.Value - GbaConstants.BaseAddr);
                    byte dataType = _trainerListManager.Working[idx].DataType;

                    if (dataType == 0)
                        IoHelper.WriteStructures(
                            _romData, 
                            actualPartyAddr, 
                            _currentPartyEntries.Cast<TrainerPartyEntry00>(), 
                            _tblReader, null, false);
                    else if (dataType == 1)
                        IoHelper.WriteStructures(
                            _romData, 
                            actualPartyAddr, 
                            _currentPartyEntries.Cast<TrainerPartyEntry01>(), 
                            _tblReader, null, false);
                    else if (dataType == 2)
                        IoHelper.WriteStructures(
                            _romData, 
                            actualPartyAddr,
                            _currentPartyEntries.Cast<TrainerPartyEntry02>(), 
                            _tblReader, null, false);
                    else if (dataType == 3)
                        IoHelper.WriteStructures(
                            _romData, 
                            actualPartyAddr, 
                            _currentPartyEntries.Cast<TrainerPartyEntry03>(),
                            _tblReader, null, false);
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveCurrentData(_currentTrainerIdx);
            _uiStateManager.UpdateInitialValues();
        }

        private void PokemonEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (btnSave.Enabled)
            {
                ControlHelper.HandleUnsavedChanges(
                    () =>
                    {
                        SaveCurrentData(_currentTrainerIdx);
                    },
                    () =>
                    {
                        // unnecessary
                    },
                    () =>
                    {
                        e.Cancel = true;
                    }
                );
            }
        }
    }
}
