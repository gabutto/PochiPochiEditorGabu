using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using PochiPochiEditorGabu.Constants;
using PochiPochiEditorGabu.FileReaders;
using PochiPochiEditorGabu.Helpers;
using PochiPochiEditorGabu.Managers;

namespace PochiPochiEditorGabu._Pokemon
{
    public partial class PokemonEditor : Form
    {
        protected byte[] _romData;
        protected IniFileReader _config;
        protected TblFileReader _tblReader;
        protected ReservationManager _reservationManager;
        protected UIStateManager _uiStateManager;

        private EntryManager<PokemonNameeEntry> _pokemonNameManager;
        private EntryManager<PokemonSpriteFrontImageEntry> _spriteFrontImgManager;
        private EntryManager<PokemonSpriteBackImageEntry> _spriteBackImgManager;
        private EntryManager<PokemonSpriteNormalPaletteEntry> _spriteNormalPalManager;
        private EntryManager<PokemonSpriteShinyPaletteEntry> _spriteShinyPalManager;

        private bool _isUpdatingUI;
        private int _currentPokemonIdx = 0;

        private Bitmap _battleAllyImage = null;
        private Bitmap _battleEnemyImage = null;

        public PokemonEditor(
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

            LoadAllDataToUI(_currentPokemonIdx);
        }

        private void InitializeManagers()
        {
            uint? pokemonNameTableAddr = _config.GetAddr("PokemonNameTableAddress");
            int pokemonNameEntryLength = _config.GetInt("PokemonNameEntryLength");
            int pokemonNameCount = _config.GetInt("PokemonNameCount");
            var lengths = new Dictionary<string, int> { { "PokemonNameEntryLength", pokemonNameEntryLength } };
            _pokemonNameManager = new EntryManager<PokemonNameeEntry>(_romData, _tblReader, lengths);
            _pokemonNameManager.Load(pokemonNameTableAddr, pokemonNameCount);

            uint? spriteFrontImgTableAddr = _config.GetAddr("PokemonSpriteFrontImageTableAddress");
            uint? spriteBackImgTableAddr = _config.GetAddr("PokemonSpriteBackImageTableAddress");
            uint? SpriteNormalPalTableAddr = _config.GetAddr("PokemonSpriteNormalPaletteTableAddress");
            uint? SpriteShinyPalTableAddr = _config.GetAddr("PokemonSpriteShinyPaletteTableAddress");
            int spriteCount = _config.GetInt("PokemonSpriteCount");
            _spriteFrontImgManager = new EntryManager<PokemonSpriteFrontImageEntry>(_romData, _tblReader);
            _spriteFrontImgManager.Load(spriteFrontImgTableAddr, spriteCount);
            _spriteBackImgManager = new EntryManager<PokemonSpriteBackImageEntry>(_romData, _tblReader);
            _spriteBackImgManager.Load(spriteBackImgTableAddr, spriteCount);
            _spriteNormalPalManager = new EntryManager<PokemonSpriteNormalPaletteEntry>(_romData, _tblReader);
            _spriteNormalPalManager.Load(SpriteNormalPalTableAddr, spriteCount);
            _spriteShinyPalManager = new EntryManager<PokemonSpriteShinyPaletteEntry>(_romData, _tblReader);
            _spriteShinyPalManager.Load(SpriteShinyPalTableAddr, spriteCount);
        }

        private void InitializeEventHandlers()
        {
            btnSave.Click += btnSave_Click;
            this.FormClosing += PokemonEditor_FormClosing;

            cmbPokemonName.SelectedIndexChanged += cmbPokemonName_SelectedIndexChanged;
            txtPokemonRename.TextChanged += txtPokemonRename_TextChanged;
            foreach (var txt in new[] {
                txtSpriteFrontImgAddr, 
                txtSpriteBackImgAddr, 
                txtSpriteNormalPalAddr, 
                txtSpriteShinyPalAddr })
            {
                txt.TextChanged += SpriteAddress_TextChanged;
            }
            btnSpriteImport.Click += btnSpriteImport_Click;
            btnSpriteExport.Click += btnSpriteExport_Click;
        }

        private void InitializeControls()
        {
            // cmbPokemonName
            var classNames = _pokemonNameManager.Working
                             .Select(entry => entry._PokemonName)
                             .ToArray();
            cmbPokemonName.Items.AddRange(classNames);

            // cmbSpriteExport
            ControlHelper.SetupComboBoxItems(
                cmbSpriteExport, 
                0,
                "正面・通常", "背面・通常", "正面・色違い", "背面・色違い");

            ControlHelper.AttachAddressAutoFormat(
                txtSpriteFrontImgAddr, txtSpriteBackImgAddr,
                txtSpriteNormalPalAddr, txtSpriteShinyPalAddr);
            ControlHelper.AttachExternalBorder
                (picSpriteFrontNormal, picSpriteBackNormal,
                picSpriteFrontShiny, picSpriteBackShiny);
            ControlHelper.AttachRadioButtonToTextBoxFocus(rbSpriteFrontImgAddr, txtSpriteFrontImgAddr);
            ControlHelper.AttachRadioButtonToTextBoxFocus(rbSpriteBackImgAddr, txtSpriteBackImgAddr);
            ControlHelper.AttachRadioButtonToTextBoxFocus(rbSpriteNormalPalAddr, txtSpriteNormalPalAddr);
            ControlHelper.AttachRadioButtonToTextBoxFocus(rbSpriteShinyPalAddr, txtSpriteShinyPalAddr);
        }

        private void InitializeUIStates()
        {
            _uiStateManager = new UIStateManager(hasChanges => btnSave.Enabled = hasChanges);
            btnSave.Enabled = false;
            _uiStateManager.AddControls(
                txtPokemonRename,
                txtSpriteFrontImgAddr, txtSpriteBackImgAddr, txtSpriteNormalPalAddr, txtSpriteShinyPalAddr);
        }

        private void LoadAllDataToUI(int idx)
        {
            _isUpdatingUI = true;
            _reservationManager.ClearAllReservations();

            _currentPokemonIdx = idx;
            LoadPokemonNameToUI(idx);
            LoadSpritesToUI(idx);

            _isUpdatingUI = false;
            _uiStateManager.UpdateInitialValues();
        }

        private void LoadPokemonNameToUI(int idx)
        {
            if (cmbPokemonName.SelectedIndex != idx)
            {
                cmbPokemonName.SelectedIndex = idx;
            }
            nudSpecies.Value = idx;
            txtSpeciesHex.Text = idx.ToString("X4");

            // Load pokemon name
            txtPokemonRename.Text = _pokemonNameManager.Working[idx]._PokemonName;
        }

        private void txtPokemonRename_TextChanged(object sender, EventArgs e)
        {
            if (_isUpdatingUI) return;

            int pokemonNameEntryLength = _config.GetInt("PokemonNameEntryLength");
            int maxAllowedBytes = pokemonNameEntryLength - 1;
            string currentText = txtPokemonRename.Text;
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

                int savedSelectionStart = txtPokemonRename.SelectionStart;
                txtPokemonRename.Text = currentText;
                txtPokemonRename.SelectionStart = Math.Min(savedSelectionStart, currentText.Length);

                _isUpdatingUI = false;
            }

            string validName = _tblReader.BytesToString(currentBytes, 0, currentBytes.Length);

            _isUpdatingUI = true;

            int idx = _currentPokemonIdx;
            cmbPokemonName.Items[idx] = validName;
            _pokemonNameManager.Working[idx]._PokemonName = validName;

            _isUpdatingUI = false;
        }

        private void RestorePokemonName(int idx)
        {
            var originalEntry = _pokemonNameManager.Original[idx];
            var workingEntry = _pokemonNameManager.Working[idx];

            var restoredEntry = CloneHelper.Clone(originalEntry);
            workingEntry._PokemonName = restoredEntry._PokemonName;

            cmbPokemonName.Items[idx] = originalEntry._PokemonName;
        }

        private void cmbPokemonName_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_isUpdatingUI) return;

            int newIndex = cmbPokemonName.SelectedIndex;
            if (newIndex == _currentPokemonIdx) return;

            if (btnSave.Enabled)
            {
                _isUpdatingUI = true;

                ControlHelper.HandleUnsavedChanges(
                    () =>
                    {
                        SaveCurrentAllData(_currentPokemonIdx);
                        ResetControls();
                        LoadAllDataToUI(newIndex);
                    },
                    () =>
                    {
                        RestorePokemonName(_currentPokemonIdx);
                        ResetControls();
                        LoadAllDataToUI(newIndex);
                    },
                    () =>
                    {
                        cmbPokemonName.SelectedIndex = _currentPokemonIdx;
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

        private void LoadSpritesToUI(int idx)
        {
            DataBindingHelper.BindObjectToControls(this, _spriteFrontImgManager.Working[idx]);
            DataBindingHelper.BindObjectToControls(this, _spriteBackImgManager.Working[idx]);
            DataBindingHelper.BindObjectToControls(this, _spriteNormalPalManager.Working[idx]);
            DataBindingHelper.BindObjectToControls(this, _spriteShinyPalManager.Working[idx]);

            DisplaySprites();
        }


        private void SpriteAddress_TextChanged(object sender, EventArgs e)
        {
            if (_isUpdatingUI) return;
            DisplaySprites();
        }

        private void DisplaySprites()
        {
            bool isImageFrontValid = ControlHelper.TryParseAddress(txtSpriteFrontImgAddr.Text, out uint imageFrontOffset);
            bool isImageBackValid = ControlHelper.TryParseAddress(txtSpriteBackImgAddr.Text, out uint imageBackOffset);
            bool isPaletteNormalValid = ControlHelper.TryParseAddress(txtSpriteNormalPalAddr.Text, out uint paletteNormalOffset);
            bool isPaletteShinyValid = ControlHelper.TryParseAddress(txtSpriteShinyPalAddr.Text, out uint paletteShinyOffset);

            foreach (var pic in new[] {
                picSpriteFrontNormal,
                picSpriteBackNormal,
                picSpriteFrontShiny,
                picSpriteBackShiny })
            {
                pic.Image?.Dispose();
                pic.Image = null;
            }

            // palette
            Color[] paletteNormal = null;
            Color[] paletteShiny = null;

            try
            {
                if (isPaletteNormalValid)
                {
                    var res = _reservationManager.GetReservation(txtSpriteNormalPalAddr);
                    paletteNormal = res != null
                        ? ImageManager.DecompressPalette(res.Data, 0, true)
                        : ImageManager.DecompressPalette(_romData, paletteNormalOffset, true);
                }
            }
            catch (Exception)
            {
            }

            try
            {
                if (isPaletteShinyValid)
                {
                    var res = _reservationManager.GetReservation(txtSpriteShinyPalAddr);
                    paletteShiny = res != null
                        ? ImageManager.DecompressPalette(res.Data, 0, true)
                        : ImageManager.DecompressPalette(_romData, paletteShinyOffset, true);
                }
            }
            catch (Exception)
            {
            }

            // front image
            if (isImageFrontValid)
            {
                try
                {
                    byte[] imageFrontData;
                    var res = _reservationManager.GetReservation(txtSpriteFrontImgAddr);
                    if (res != null)
                    {
                        imageFrontData = ImageManager.DecompressLZ77(res.Data, 0);
                    }
                    else
                    {
                        imageFrontData = ImageManager.DecompressLZ77(_romData, imageFrontOffset);
                    }

                    if (paletteNormal != null)
                    {
                        picSpriteFrontNormal.Image = ImageManager.CreateSprite(
                            imageFrontData, paletteNormal, GbaConstants.SpriteSize, GbaConstants.SpriteSize, true);
                        picSpriteFrontNormal.Refresh();

                        // for coodinate preview
                        _battleEnemyImage?.Dispose();
                        _battleEnemyImage = ImageManager.CreateSprite(
                            imageFrontData, paletteNormal, GbaConstants.SpriteSize, GbaConstants.SpriteSize, false);
                    }

                    if (paletteShiny != null)
                    {
                        picSpriteFrontShiny.Image = ImageManager.CreateSprite(
                            imageFrontData, paletteShiny, GbaConstants.SpriteSize, GbaConstants.SpriteSize, true);
                        picSpriteFrontShiny.Refresh();
                    }
                }
                catch (Exception)
                {
                }
            }

            // back image
            if (isImageBackValid)
            {
                try
                {
                    byte[] imageBackData;
                    var res = _reservationManager.GetReservation(txtSpriteBackImgAddr);
                    if (res != null)
                    {
                        imageBackData = ImageManager.DecompressLZ77(res.Data, 0);
                    }
                    else
                    {
                        imageBackData = ImageManager.DecompressLZ77(_romData, imageBackOffset);
                    }

                    if (paletteNormal != null)
                    {
                        picSpriteBackNormal.Image = ImageManager.CreateSprite(
                            imageBackData, paletteNormal, GbaConstants.SpriteSize, GbaConstants.SpriteSize, true);
                        picSpriteBackNormal.Refresh();

                        // for coodinate preview
                        _battleAllyImage?.Dispose();
                        _battleAllyImage = ImageManager.CreateSprite(
                            imageBackData, paletteNormal, GbaConstants.SpriteSize, GbaConstants.SpriteSize, false);
                    }

                    if (paletteShiny != null)
                    {
                        picSpriteBackShiny.Image = ImageManager.CreateSprite(
                            imageBackData, paletteShiny, GbaConstants.SpriteSize, GbaConstants.SpriteSize, true);
                        picSpriteBackShiny.Refresh();
                    }
                }
                catch (Exception)
                {
                }
            }

            //UpdatePokemonBattleDisplay();
            //UpdatePokemonItemUseDisplay();
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

                        if (!ImageManager.ExtractImageAndPalette(bmp, GbaConstants.SpriteSize, GbaConstants.SpriteSize, out imageData, out palette)) return;

                        if (rbSpriteFrontImgAddr.Checked)
                        {
                            var compressedData = ImageManager.CompressLZ77(imageData);
                            _reservationManager.SetReservation(txtSpriteFrontImgAddr, (uint)targetAddress, compressedData);
                        }
                        else if (rbSpriteBackImgAddr.Checked)
                        {
                            var compressedData = ImageManager.CompressLZ77(imageData);
                            _reservationManager.SetReservation(txtSpriteBackImgAddr, (uint)targetAddress, compressedData);
                        }
                        else if (rbSpriteNormalPalAddr.Checked)
                        {
                            var compressedPalette = ImageManager.CompressPalette(palette, true);
                            _reservationManager.SetReservation(txtSpriteNormalPalAddr, (uint)targetAddress, compressedPalette);
                        }
                        else if (rbSpriteShinyPalAddr.Checked)
                        {
                            var compressedPalette = ImageManager.CompressPalette(palette, true);
                            _reservationManager.SetReservation(txtSpriteShinyPalAddr, (uint)targetAddress, compressedPalette);
                        }

                        DisplaySprites();
                    }
                }
            }
        }

        private void btnSpriteExport_Click(object sender, EventArgs e)
        {
            var exportSettings = new (PictureBox Pic, string Suffix)[]
            {
                (picSpriteFrontNormal, "front_normal"),
                (picSpriteBackNormal, "back_normal"),
                (picSpriteFrontShiny, "front_shiny"),
                (picSpriteBackShiny, "back_shiny")
            };

            int selectedIndex = cmbSpriteExport.SelectedIndex;
            var target = exportSettings[selectedIndex];
            var bmp = target.Pic.Image as Bitmap;
            if (bmp == null) return;

            using (var sfd = new SaveFileDialog())
            {
                sfd.Filter = GbaConstants.ImageExportFilter;
                sfd.FileName = $"pokemon_sprite_{((int)nudSpecies.Value):D4}_{target.Suffix}";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    ImageManager.ExportIndexedImage(bmp, sfd.FileName);
                }
            }
        }













        private void ResetControls()
        {
            txtSpriteImportAddr.Text = String.Empty;
            cmbSpriteExport.SelectedIndex = 0;
            txtIconImportAddr.Text = String.Empty;
            txtFootprintImportAddr.Text = String.Empty;
        }

        private void SaveCurrentAllData(int idx)
        {
            SaveCurrentPokemonName(idx);
            SaveCurrentSprites(idx);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveCurrentAllData(_currentPokemonIdx);
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
                        SaveCurrentAllData(_currentPokemonIdx);
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

        private void SaveCurrentPokemonName(int idx)
        {
            _pokemonNameManager.Save(idx);
        }

        private void SaveCurrentSprites(int idx)
        {
            var textboxes = new[]
            {
                txtSpriteFrontImgAddr,
                txtSpriteBackImgAddr,
                txtSpriteNormalPalAddr,
                txtSpriteShinyPalAddr
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

            DataBindingHelper.BindControlsToObject(this, _spriteFrontImgManager.Working[idx]);
            DataBindingHelper.BindControlsToObject(this, _spriteBackImgManager.Working[idx]);
            DataBindingHelper.BindControlsToObject(this, _spriteNormalPalManager.Working[idx]);
            DataBindingHelper.BindControlsToObject(this, _spriteShinyPalManager.Working[idx]);
            _spriteFrontImgManager.Save(idx);
            _spriteBackImgManager.Save(idx);
            _spriteNormalPalManager.Save(idx);
            _spriteShinyPalManager.Save(idx);
        }
    }
}
