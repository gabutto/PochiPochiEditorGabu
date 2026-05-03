using System;
using System.Drawing;
using System.Windows.Forms;

using PochiPochiEditorGabu.Constants;
using PochiPochiEditorGabu.FileReaders;
using PochiPochiEditorGabu.Helpers;
using PochiPochiEditorGabu.Managers;

namespace PochiPochiEditorGabu._Trainer
{
    public partial class TrainerSpriteEditor : Form
    {
        protected byte[] _romData;
        protected IniFileReader _config;
        protected TblFileReader _tblReader;
        protected ReservationManager _reservationManager;
        protected UIStateManager _uiStateManager;

        private EntryManager<TrainerSpriteImageEntry> _ImgManager;
        private EntryManager<TrainerSpritePaletteEntry> _PalManager;
        private EntryManager<TrainerSpriteYOffsetEntry> _yOffsetManager;
        private EntryManager<TrainerSpriteAnimationPointerEntry> _animManager;

        private bool _isUpdatingUI;
        private int _currentSpriteIdx = 0;

        public TrainerSpriteEditor(byte[] romData, IniFileReader config, TblFileReader tblReader, ReservationManager reservationManager)
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

            LoadDataToUI(_currentSpriteIdx);
        }

        private void InitializeManagers()
        {
            _ImgManager = EntryManager<TrainerSpriteImageEntry>.Create(
                _romData, _tblReader, _config, "TrainerSpriteImageTableAddress", "TrainerSpriteCount");
            _PalManager = EntryManager<TrainerSpritePaletteEntry>.Create(
                _romData, _tblReader, _config, "TrainerSpritePaletteTableAddress", "TrainerSpriteCount");
            _yOffsetManager = EntryManager<TrainerSpriteYOffsetEntry>.Create(
                _romData, _tblReader, _config, "TrainerSpriteYOffsetTableAddress", "TrainerSpriteCount");
            _animManager = EntryManager<TrainerSpriteAnimationPointerEntry>.Create(
                _romData, _tblReader, _config, "TrainerSpriteAnimationPointerTableAddress", "TrainerSpriteCount");
        }

        private void InitializeEventHandlers()
        {
            nudSprite.ValueChanged += nudSprite_ValueChanged;
            txtSpriteImgAddr.TextChanged += SpriteAddress_TextChanged;
            txtSpritePalAddr.TextChanged += SpriteAddress_TextChanged;
            btnSpriteImport.Click += btnSpriteImport_Click;
            btnSpriteExport.Click += btnSpriteExport_Click;
            btnSave.Click += btnSave_Click;
            this.FormClosing += TrainerSpriteEditor_FormClosing;
        }

        private void SpriteAddress_TextChanged(object sender, EventArgs e)
        {
            if (_isUpdatingUI) return;
            DisplayTrainerSprite();
        }

        private void InitializeControls()
        {
            int spriteCount = _config.GetInt("TrainerSpriteCount");
            nudSprite.Maximum = spriteCount - 1;

            ControlHelper.AttachAddressAutoFormat(txtSpriteImgAddr, txtSpritePalAddr);
            ControlHelper.AttachExternalBorder(picSprite);
            ControlHelper.AttachNumericUpDownNavigators(nudSprite, btnSpritePrev, btnSpriteNext);
            ControlHelper.AttachRadioButtonToTextBoxFocus(rbSpriteImgAddr, txtSpriteImgAddr);
            ControlHelper.AttachRadioButtonToTextBoxFocus(rbSpritePalAddr, txtSpritePalAddr);
        }

        private void InitializeUIStates()
        {
            _uiStateManager = new UIStateManager(hasChanges => btnSave.Enabled = hasChanges);
            btnSave.Enabled = false;
            _uiStateManager.AddControls(txtSpriteImgAddr, txtSpritePalAddr, nudSpriteYOffset);
        }


        private void LoadDataToUI(int idx)
        {
            _isUpdatingUI = true;
            _reservationManager.ClearAllReservations();

            _currentSpriteIdx = idx;

            DataBindingHelper.BindObjectToControls(this, _ImgManager.Working[idx]);
            DataBindingHelper.BindObjectToControls(this, _PalManager.Working[idx]);
            DataBindingHelper.BindObjectToControls(this, _yOffsetManager.Working[idx]);
            DataBindingHelper.BindObjectToControls(this, _animManager.Working[idx]);

            // Load anim data addr
            uint prtAddr = _animManager.Working[idx].pAnimPtrAddr - GbaConstants.BaseAddr;
            if (IoHelper.TryReadGbaPointer(prtAddr, _romData, out uint? dataAddr))
            {
                if (dataAddr == null)
                {
                    txtAnimDataAddr.Text = "null";
                }
                else
                {
                    txtAnimDataAddr.Text = dataAddr.Value.ToString("X8");
                }
            }

            DisplayTrainerSprite();

            _isUpdatingUI = false;
            _uiStateManager.UpdateInitialValues();
        }

        private void nudSprite_ValueChanged(object sender, EventArgs e)
        {
            if (_isUpdatingUI) return;

            int newIndex = (int)nudSprite.Value;
            if (newIndex == _currentSpriteIdx) return;

            if (btnSave.Enabled)
            {
                _isUpdatingUI = true;

                ControlHelper.HandleUnsavedChanges(
                    () =>
                    {
                        SaveCurrentData(_currentSpriteIdx);
                        ControlHelper.ResetControls(grpImportExport);
                        LoadDataToUI(newIndex);
                    },
                    () =>
                    {
                        ControlHelper.ResetControls(grpImportExport);
                        LoadDataToUI(newIndex);
                    },
                    () =>
                    {
                        nudSprite.Value = _currentSpriteIdx;
                    }

                );

                _isUpdatingUI = false;
            }
            else
            {
                ControlHelper.ResetControls(grpImportExport);
                LoadDataToUI(newIndex);
            }
        }

        private void DisplayTrainerSprite()
        {
            bool isImageValid = ControlHelper.TryParseAddress(txtSpriteImgAddr.Text, out uint imageOffset);
            bool isPaletteValid = ControlHelper.TryParseAddress(txtSpritePalAddr.Text, out uint paletteOffset);

            if (!isImageValid || !isPaletteValid)
            {
                picSprite.Image?.Dispose();
                picSprite.Image = null;
                return;
            }

            try
            {
                Color[] palette;
                byte[] imageData;

                var paletteRes = _reservationManager.GetReservation(txtSpritePalAddr);
                if (paletteRes?.Data != null)
                {
                    palette = ImageManager.DecompressPalette(paletteRes.Data, 0, true);
                }
                else
                {
                    palette = ImageManager.DecompressPalette(_romData, paletteOffset, true);
                }

                var imageRes = _reservationManager.GetReservation(txtSpriteImgAddr);
                if (imageRes?.Data != null)
                {
                    imageData = ImageManager.DecompressLZ77(imageRes.Data, 0);
                }
                else
                {
                    imageData = ImageManager.DecompressLZ77(_romData, imageOffset);
                }

                Bitmap sprite = ImageManager.CreateSprite(imageData, palette, GbaConstants.SpriteSize, GbaConstants.SpriteSize, true);

                picSprite.Image?.Dispose();
                picSprite.Image = sprite;
                picSprite.Refresh();
            }
            catch
            {
                picSprite.Image?.Dispose();
                picSprite.Image = null;
            }
        }

        private void btnSpriteImport_Click(object sender, EventArgs e)
        {
            if (!ControlHelper.ValidateAndFormatInputTextBox(txtSpriteImportAddr, out uint? targetAddress)) return;

            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = GbaConstants.ImageImportFilter;
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    using (Bitmap bmp = new Bitmap(ofd.FileName))
                    {
                        byte[] imageData = null;
                        Color[] palette = null;

                        if (!ImageManager.ExtractImageAndPalette(bmp, GbaConstants.SpriteSize, GbaConstants.SpriteSize, out imageData, out palette))
                        {
                            return;
                        }

                        if (rbSpriteImgAddr.Checked)
                        {
                            var compressedData = ImageManager.CompressLZ77(imageData);
                            _reservationManager.SetReservation(txtSpriteImgAddr, targetAddress.Value, compressedData);
                        }
                        else if (rbSpritePalAddr.Checked)
                        {
                            var compressedPalette = ImageManager.CompressPalette(palette, true);
                            _reservationManager.SetReservation(txtSpritePalAddr, targetAddress.Value, compressedPalette);
                        }

                        DisplayTrainerSprite();
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
                sfd.FileName = $"trainer_sprite_{(int)nudSprite.Value:D4}";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    ImageManager.ExportIndexedImage((Bitmap)picSprite.Image, sfd.FileName);
                }
            }
        }

        private void SaveCurrentData(int idx)
        {
            var imageRes = _reservationManager.GetReservation(txtSpriteImgAddr);
            if (imageRes != null && imageRes.Data != null)
            {
                Array.Copy(imageRes.Data, 0, _romData, (int)imageRes.Address, imageRes.Data.Length);
                _reservationManager.ClearReservation(txtSpriteImgAddr);
            }

            var paletteRes = _reservationManager.GetReservation(txtSpritePalAddr);
            if (paletteRes != null && paletteRes.Data != null)
            {
                Array.Copy(paletteRes.Data, 0, _romData, (int)paletteRes.Address, paletteRes.Data.Length);
                _reservationManager.ClearReservation(txtSpritePalAddr);
            }

            DataBindingHelper.BindControlsToObject(this, _ImgManager.Working[idx]);
            DataBindingHelper.BindControlsToObject(this, _PalManager.Working[idx]);
            DataBindingHelper.BindControlsToObject(this, _yOffsetManager.Working[idx]);
            DataBindingHelper.BindControlsToObject(this, _animManager.Working[idx]);

            _ImgManager.Save(idx);
            _PalManager.Save(idx);
            _yOffsetManager.Save(idx);
            _animManager.Save(idx);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveCurrentData(_currentSpriteIdx);
            _uiStateManager.UpdateInitialValues();
        }

        private void TrainerSpriteEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (btnSave.Enabled)
            {
                _isUpdatingUI = true;

                ControlHelper.HandleUnsavedChanges(
                    () =>
                    {
                        SaveCurrentData(_currentSpriteIdx);
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
    }
}
