using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
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
        private EntryManager<TrainerPartyEntry00> _trainerParty00Manager;
        private EntryManager<TrainerPartyEntry01> _trainerParty01Manager;
        private EntryManager<TrainerPartyEntry02> _trainerParty02Manager;
        private EntryManager<TrainerPartyEntry03> _trainerParty03Manager;

        private EntryManager<TrainerClassNameEntry> _classNameManager;
        private EntryManager<ItemSpriteEntry> _itemSpriteManager;
        private EntryManager<ItemDataEntry> _itemDataManager;
        private EntryManager<PokemonNameEntry> _pokemonNameManager;
        private EntryManager<MoveNameEntry> _moveNameManager;
        private EntryManager<TrainerSpriteImageEntry> _trainerImgManager;
        private EntryManager<TrainerSpritePaletteEntry> _trainerPalManager;

        private bool _isUpdatingUI = false;
        private int _currentTrainerIdx = 0;

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
        }

        private void InitializeEventHandlers()
        {
            btnSave.Click += btnSave_Click;
            this.FormClosing += PokemonEditor_FormClosing;

            lstTrainerData.SelectedIndexChanged += lstTrainerData_SelectedIndexChanged;
            txtName.TextChanged += txtName_TextChanged;
            nudSpriteIdx.ValueChanged += nudTrainerSpriteIdx_ValueChanged;
            foreach (var nud in new[] {
                cmbHoldItem1,
                cmbHoldItem2,
                cmbHoldItem3,
                cmbHoldItem4,
                cmbPartyItem})
            {
                nud.SelectedIndexChanged += ItemComboBox_SelectedIndexChanged;
            }
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

            UpdateTrainerSprite();
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
            _trainerListManager.Working[_currentTrainerIdx]._Name = validName;
            lstTrainerData.Items[_currentTrainerIdx] = $"{_currentTrainerIdx:X4} - {validName}";
            _isUpdatingUI = false;
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

        private void ItemComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_isUpdatingUI) return;
            UpdateItemImages();
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

        private void nudTrainerSpriteIdx_ValueChanged(object sender, EventArgs e)
        {
            if (_isUpdatingUI) return;
            UpdateTrainerSprite();
        }

















        private void RestoreData(int idx)
        {
            // name
            string name = _trainerListManager.Original[idx]._Name;
            lstTrainerData.Items[_currentTrainerIdx] = $"{_currentTrainerIdx:X4} - {name}";
        }

        private void SaveCurrentData(int idx)
        {
            // trainer

            // name
            _trainerListManager.Save(idx);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            // something
            _uiStateManager.UpdateInitialValues();
        }

        private void PokemonEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (btnSave.Enabled)
            {
                _isUpdatingUI = true;

                ControlHelper.HandleUnsavedChanges(
                    () =>
                    {
                        // something
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

                _isUpdatingUI = false;
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
    }
}
