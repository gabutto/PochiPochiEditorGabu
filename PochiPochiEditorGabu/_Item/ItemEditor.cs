using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using PochiPochiEditorGabu.Constants;
using PochiPochiEditorGabu.FileReaders;
using PochiPochiEditorGabu.Helpers;
using PochiPochiEditorGabu.Managers;

namespace PochiPochiEditorGabu._Item
{
    public partial class ItemEditor : Form
    {
        protected byte[] _romData;
        protected IniFileReader _config;
        protected TblFileReader _tblReader;
        protected ReservationManager _reservationManager;
        protected UIStateManager _uiStateManager;

        private EntryManager<ItemSpriteEntry> _itemSpriteManager;
        private EntryManager<ItemDataEntry> _itemDataManager;
        private EntryManager<ItemEffectEntry> _itemEffectManager;

        private bool _isUpdatingUI = false;
        private int _currentItemIdx = 0;

        private byte[] _currentDescData = null;
        private bool _isUpdatingFieldUseSync = false;
        private int _pokeBallPocketIdx = -1;
        private int _prevPocketIdx = -1;

        public ItemEditor(
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

            LoadAllDataToUI(_currentItemIdx);
        }

        private void InitializeManagers()
        {
            _itemSpriteManager = EntryManager<ItemSpriteEntry>.Create(
                _romData, _tblReader, _config, "ItemSpriteTableAddress", "ItemDataCount");

            _itemDataManager = EntryManager<ItemDataEntry>.Create(
                _romData, _tblReader, _config, "ItemDataTableAddress", "ItemDataCount");

            uint? itemEffectTableAddr = _config.GetAddr("ItemEffectTableAddress");
            int itemEffectCount = _config.GetInt("ItemEffectLastIndex") - _config.GetInt("ItemEffectFirstIndex") + 1;
            _itemEffectManager = new EntryManager<ItemEffectEntry>(_romData, _tblReader);
            _itemEffectManager.Load(itemEffectTableAddr, itemEffectCount);
        }

        private void InitializeEventHandlers()
        {
            btnSave.Click += btnSave_Click;
            this.FormClosing += ItemEditor_FormClosing;

            cmbItemName.SelectedIndexChanged += cmbItemName_SelectedIndexChanged;
            txtItemRename.TextChanged += txtItemRename_TextChanged;

            txtSpriteImgAddr.TextChanged += SpriteAddress_TextChanged;
            txtSpritePalAddr.TextChanged += SpriteAddress_TextChanged;
            btnSpriteImport.Click += btnSpriteImport_Click;
            btnSpriteExport.Click += btnSpriteExport_Click;

            txtDescAddr.TextChanged += txtDescAddr_TextChanged;
            txtDescString.TextChanged += txtDescString_TextChanged;

            cmbPocketIdx.SelectedIndexChanged += cmbPocketIdx_SelectedIndexChanged;
            nudFieldUseType.ValueChanged += nudFieldUseType_ValueChanged;
            cmbFieldUseType.SelectedIndexChanged += cmbFieldUseType_SelectedIndexChanged;
        }

        private void InitializeControls()
        {
            //cmbItemName
            var itemNames = _itemDataManager.Working
                 .Select(entry => entry._ItemName)
                 .ToArray();
            cmbItemName.Items.AddRange(itemNames);

            ControlHelper.AttachAddressAutoFormat(
                txtSpriteImgAddr, txtSpritePalAddr,
                txtDescString,
                txtItemEffectAddr,
                txtFieldUseAddr, txtBattleUseAddr);
            ControlHelper.AttachExternalBorder(picSprite);
            ControlHelper.AttachRadioButtonToTextBoxFocus(rbSpriteImgAddr, txtSpriteImgAddr);
            ControlHelper.AttachRadioButtonToTextBoxFocus(rbSpritePalAddr, txtSpritePalAddr);

            ControlHelper.LoadComboBoxFromTextFile(cmbHoldEffectIdx, "txt/ItemDataHoldEffectIdx.txt");
            ControlHelper.LoadComboBoxFromTextFile(cmbCanHold, "txt/ItemDataCanHold.txt");
            ControlHelper.LoadComboBoxFromTextFile(cmbPocketIdx, "txt/ItemDataPocketIdx.txt");
            ControlHelper.LoadComboBoxFromTextFile(cmbFieldUseType, "txt/ItemDataFieldUseType.txt");
            ControlHelper.LoadComboBoxFromTextFile(cmbBattleUseType, "txt/ItemDataBattleUseType.txt");
        }

        private void InitializeUIStates()
        {
            _uiStateManager = new UIStateManager(hasChanges => btnSave.Enabled = hasChanges);
            btnSave.Enabled = false;
            _uiStateManager.AddControls(
                txtItemRename,
                txtSpriteImgAddr, txtSpritePalAddr,
                txtDescAddr,
                txtItemEffectAddr,
                nudIdx, nudPrice, cmbHoldEffectIdx, nudEffectValue, cmbCanHold, nudUnknownValue, cmbPocketIdx,
                nudFieldUseType, cmbFieldUseType, txtFieldUseAddr,
                cmbBattleUseType, txtBattleUseAddr,
                nudSpecialIdx);
            _uiStateManager.AddBinaries(
                (txtDescString, null));
        }

        private void LoadAllDataToUI(int idx)
        {
            _isUpdatingUI = true;
            _reservationManager.ClearAllReservations();

            _currentItemIdx = idx;
            LoaNameToUI(idx);
            LoadSpriteToUI(idx);
            LoadItemDataToUI(idx);
            LoadEffectAddrToUI(idx);

            _isUpdatingUI = false;
            _uiStateManager.UpdateInitialValues();
        }

        private void LoaNameToUI(int idx)
        {
            cmbItemName.SelectedIndex = idx;
            nudItemId.Value = idx;
            txtItemIdHex.Text = idx.ToString("X4");

            // Load pokemon name
            txtItemRename.Text = _itemDataManager.Working[idx]._ItemName;
        }

        private void txtItemRename_TextChanged(object sender, EventArgs e)
        {
            if (_isUpdatingUI) return;

            int pokemonNameEntryLength = _config.GetInt("ItemNameMaxLength");
            int maxAllowedBytes = pokemonNameEntryLength - 1;
            string currentText = txtItemRename.Text;
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

                int savedSelectionStart = txtItemRename.SelectionStart;
                txtItemRename.Text = currentText;
                txtItemRename.SelectionStart = Math.Min(savedSelectionStart, currentText.Length);

                _isUpdatingUI = false;
            }

            string validName = _tblReader.BytesToString(currentBytes, 0, currentBytes.Length);

            _isUpdatingUI = true;

            int idx = _currentItemIdx;
            cmbItemName.Items[idx] = validName;
            _itemDataManager.Working[idx]._ItemName = validName;

            _isUpdatingUI = false;
        }

        private void RestoreItemName(int idx)
        {
            var originalEntry = _itemDataManager.Original[idx];
            var workingEntry = _itemDataManager.Working[idx];

            var restoredEntry = CloneHelper.Clone(originalEntry);
            workingEntry._ItemName = restoredEntry._ItemName;

            cmbItemName.Items[idx] = originalEntry._ItemName;
        }

        private void cmbItemName_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_isUpdatingUI) return;

            int newIndex = cmbItemName.SelectedIndex;
            if (newIndex == _currentItemIdx) return;

            if (btnSave.Enabled)
            {
                _isUpdatingUI = true;

                ControlHelper.HandleUnsavedChanges(
                    () =>
                    {
                        SaveCurrentAllData(_currentItemIdx);
                        ResetControls();
                        LoadAllDataToUI(newIndex);
                    },
                    () =>
                    {
                        RestoreItemName(_currentItemIdx);
                        ResetControls();
                        LoadAllDataToUI(newIndex);
                    },
                    () =>
                    {
                        cmbItemName.SelectedIndex = _currentItemIdx;
                    }

                );

                _isUpdatingUI = false;
            }
            else
            {
                ResetControls();
                LoadAllDataToUI(newIndex);
            }
        }

        private void LoadSpriteToUI(int idx)
        {
            DataBindingHelper.BindObjectToControls(this, _itemSpriteManager.Working[idx]);
            DisplaySprite();
        }

        private void SpriteAddress_TextChanged(object sender, EventArgs e)
        {
            if (_isUpdatingUI) return;
            DisplaySprite();
        }

        private void DisplaySprite()
        {
            picSprite.Image?.Dispose();
            picSprite.Image = null;

            using (Bitmap rawSprite = GetCurrentItemBitmap())
            {
                if (rawSprite != null)
                {
                    picSprite.Image = ImageManager.ScalePixelArt(rawSprite);
                    picSprite.Refresh();
                }
                else
                {
                    picSprite.Image = null;
                }
            }
        }

        private Bitmap GetCurrentItemBitmap()
        {
            if (!ControlHelper.TryParseAddress(txtSpriteImgAddr.Text, out uint imageOffset) ||
                !ControlHelper.TryParseAddress(txtSpritePalAddr.Text, out uint paletteOffset))
            {
                return null;
            }
                
            try
            {
                Color[] palette;
                byte[] imageData;

                // palette
                var paletteRes = _reservationManager.GetReservation(txtSpritePalAddr);
                if (paletteRes != null)
                    palette = ImageManager.DecompressPalette(paletteRes.Data, 0, true);
                else
                    palette = ImageManager.DecompressPalette(_romData, paletteOffset, true);

                // image
                var imageRes = _reservationManager.GetReservation(txtSpriteImgAddr);
                if (imageRes != null)
                    imageData = ImageManager.DecompressLZ77(imageRes.Data, 0);
                else
                    imageData = ImageManager.DecompressLZ77(_romData, imageOffset);

                return ImageManager.CreateSprite(imageData, palette, GbaConstants.ItemSpriteSize, GbaConstants.ItemSpriteSize, true);
            }
            catch
            {
                return null;
            }
        }

        private void btnSpriteImport_Click(object sender, EventArgs e)
        {
            if (!ControlHelper.ValidateAndFormatInputTextBox(txtSpriteImportAddr, out uint? targetAddress)) return;

            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = GbaConstants.ImageImportFilter;
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    using (var bmp = new Bitmap(ofd.FileName))
                    {
                        byte[] imageData;
                        Color[] palette;

                        if (!ImageManager.ExtractImageAndPalette(
                            bmp,
                            GbaConstants.ItemSpriteSize,
                            GbaConstants.ItemSpriteSize,
                            out imageData,
                            out palette))
                        {
                            return;
                        }

                        if (rbSpriteImgAddr.Checked)
                        {
                            var compressedData = ImageManager.CompressLZ77(imageData);
                            _reservationManager.SetReservation(txtSpriteImgAddr, (uint)targetAddress, compressedData);
                        }
                        else if (rbSpritePalAddr.Checked)
                        {
                            var compressedPalette = ImageManager.CompressPalette(palette, true);
                            _reservationManager.SetReservation(txtSpritePalAddr, (uint)targetAddress, compressedPalette);
                        }

                        DisplaySprite();
                    }
                }
            }
        }

        private void btnSpriteExport_Click(object sender, EventArgs e)
        {
            if (picSprite.Image == null) return;

            using (var sfd = new SaveFileDialog())
            {
                sfd.Filter = GbaConstants.ImageExportFilter;
                sfd.FileName = $"item_sprite_{(int)nudItemId.Value:D4}";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    using (var exportBmp = GetCurrentItemBitmap())
                    {
                        if (exportBmp != null)
                        {
                            ImageManager.ExportIndexedImage(exportBmp, sfd.FileName);
                        }
                    }
                }
            }
        }

        private void LoadItemDataToUI(int idx)
        {
            DataBindingHelper.BindObjectToControls(this, _itemDataManager.Working[idx]);
            DisplayItemDesc();
            UpdateFieldUseControlsState();
        }

        private void txtDescAddr_TextChanged(object sender, EventArgs e)
        {
            if (_isUpdatingUI) return;
            DisplayItemDesc();
        }
        private void txtDescString_TextChanged(object sender, EventArgs e)
        {
            if (_isUpdatingUI) return;

            byte[] bytes = _tblReader.StringToBytes(txtDescString.Text, true);
            _currentDescData = bytes;
            _uiStateManager.UpdateBinary(txtDescString, bytes);
        }

        private void DisplayItemDesc()
        {
            if (ControlHelper.TryParseAddress(txtDescAddr.Text, out uint address))
            {
                List<byte> descriptionBytes = new List<byte>();
                uint i = 0;

                while (address + i < _romData.Length)
                {
                    byte b = _romData[(int)(address + i)];
                    descriptionBytes.Add(b);
                    if (b == 0xFF)
                        break;
                    i++;
                }

                byte[] byteArr = descriptionBytes.ToArray();
                _currentDescData = byteArr;
                txtDescString.Text = _tblReader.BytesToString(byteArr, 0, 255);
                _uiStateManager.UpdateBinary(txtDescString, byteArr);
            }
            else
            {
                _currentDescData = null;
                txtDescString.Text = string.Empty;
                _uiStateManager.UpdateBinary(txtDescString, null);
            }
        }

        private void UpdateFieldUseControlsState()
        {
            if (_isUpdatingUI) return;

            var selectedPocket = cmbPocketIdx.SelectedItem;
            bool isPokeBall = false;
            if (selectedPocket != null)
            {
                int pocketIndex = Convert.ToInt32(cmbPocketIdx.SelectedValue);
                isPokeBall = (pocketIndex == _pokeBallPocketIdx);
            }

            if (isPokeBall)
            {
                nudFieldUseType.Enabled = true;
                cmbFieldUseType.Enabled = false;
                cmbFieldUseType.SelectedIndex = -1;
            }
            else
            {
                nudFieldUseType.Enabled = false;
                cmbFieldUseType.Enabled = true;
                int fieldUseValue = (int)nudFieldUseType.Value;
                if (fieldUseValue >= 0 && fieldUseValue < cmbFieldUseType.Items.Count)
                {
                    cmbFieldUseType.SelectedIndex = fieldUseValue;
                }
                else
                {
                    cmbFieldUseType.SelectedIndex = -1;
                }
            }
        }

        private void cmbPocketIdx_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_isUpdatingUI) return;

            int newPocketIndex = Convert.ToInt32(cmbPocketIdx.SelectedValue);
            bool wasMonsterBall = (_prevPocketIdx == _pokeBallPocketIdx);
            bool isMonsterBall = (newPocketIndex == _pokeBallPocketIdx);

            if (wasMonsterBall && !isMonsterBall)
            {
                _isUpdatingFieldUseSync = true;
                nudFieldUseType.Value = 0m;
                _isUpdatingFieldUseSync = false;
            }

            _prevPocketIdx = newPocketIndex;
            UpdateFieldUseControlsState();
        }

        private void nudFieldUseType_ValueChanged(object sender, EventArgs e)
        {
            if (_isUpdatingFieldUseSync) return;
        }

        private void cmbFieldUseType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_isUpdatingFieldUseSync) return;

            if (cmbFieldUseType.Enabled)
            {
                int newIndex = cmbFieldUseType.SelectedIndex;
                if (newIndex >= 0)
                {
                    _isUpdatingFieldUseSync = true;
                    nudFieldUseType.Value = newIndex;
                    _isUpdatingFieldUseSync = false;
                }
            }
        }

        private void LoadEffectAddrToUI(int idx)
        {
            int actualIndex = idx - _config.GetInt("ItemEffectFirstIndex");
            bool isValid = actualIndex >= 0 && actualIndex < _itemEffectManager.Count;

            if (isValid)
            {
                ControlHelper.SetControlsEnabled(grpItemEffect, true);
                DataBindingHelper.BindObjectToControls(this, _itemEffectManager.Working[idx]);
            }
            else
            {
                ControlHelper.SetControlsEnabled(grpItemEffect, false);
                ControlHelper.ResetControls(grpItemEffect);
            };
        }















        private void ResetControls()
        {
            txtSpriteImportAddr.Text = String.Empty;
        }

        private void SaveCurrentAllData(int idx)
        {
            SaveCurrentSprite(idx);
            SaveCurrentItemDescData();
            SaveCurrentItemData(idx);
            SaveCurrentEffectAddr(idx);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveCurrentAllData(_currentItemIdx);
            _uiStateManager.UpdateInitialValues();
        }

        private void ItemEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (btnSave.Enabled)
            {
                _isUpdatingUI = true;

                ControlHelper.HandleUnsavedChanges(
                    () =>
                    {
                        SaveCurrentAllData(_currentItemIdx);
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

        private void SaveCurrentSprite(int idx)
        {
            var textboxes = new[]
{
                txtSpriteImgAddr,
                txtSpritePalAddr,
            };

            foreach (var txt in textboxes)
            {
                var res = _reservationManager.GetReservation(txt);
                if (res != null && res.Data != null)
                {
                    Array.Copy(res.Data, 0, _romData, (int)res.Address, res.Data.Length);
                    _reservationManager.ClearReservation(txt);
                }
            }

            DataBindingHelper.BindControlsToObject(this, _itemSpriteManager.Working[idx]);
            _itemSpriteManager.Save(idx);
        }

        private void SaveCurrentItemDescData()
        {
            if (!ControlHelper.TryParseAddress(txtDescAddr.Text, out uint address)) return;

            if (_uiStateManager.HasBinaryChanges(txtDescString) && _currentDescData != null)
            {
                _tblReader.WriteToRom(_romData, address, _currentDescData);
            }
        }

        private void SaveCurrentItemData(int idx)
        {
            DataBindingHelper.BindControlsToObject(this, _itemDataManager.Working[idx]);
            _itemDataManager.Save(idx);
        }

        private void SaveCurrentEffectAddr(int idx)
        {

        }
    }
}
