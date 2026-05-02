using System;
using System.Drawing;
using System.Linq;
using System.Reflection;
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

        private EntryManager<PokemonNameEntry> _pokemonNameManager;
        private EntryManager<PokemonSpriteFrontImageEntry> _spriteFrontImgManager;
        private EntryManager<PokemonSpriteBackImageEntry> _spriteBackImgManager;
        private EntryManager<PokemonSpriteNormalPaletteEntry> _spriteNormalPalManager;
        private EntryManager<PokemonSpriteShinyPaletteEntry> _spriteShinyPalManager;
        private EntryManager<PokemonIconImageEntry> _iconImgManager;
        private EntryManager<PokemonIconPaletteIndexEntry> _iconPalIdxManager;
        private EntryManager<PokemonIconPaletteAddressEntry> _iconPalAddrManager;
        private EntryManager<PokemonFootPrintImageEntry> _footprintImgManager;
        private EntryManager<PokemonCoordBattleAllyEntry> _coordBattleAllyManager;
        private EntryManager<PokemonCoordBattleEnemyEntry> _coordBattleEnemyManager;
        private EntryManager<PokemonCoordBattleEnemyShaowEntry> _coordBattleEnemyShadowManager;
        private EntryManager<PokemonCoordItemUseEntry> _coordItemUseManager;
        private EntryManager<PokemonStatsNormalEntry> _statsNormalManager;
        private EntryManager<PokemonStatsExpansionEntry> _statsExpansionManager;

        private EntryManager<AbilityNameEntry> _abilityNameManager;
        private EntryManager<ItemSpriteEntry> _itemSpriteManager;
        private EntryManager<ItemDataEntry> _itemDataManager;
        private EntryManager<TypeNameEntry> _typeNameManager;
        private EntryManager<MoveNameEntry> _moveNameManager;

        private bool _isUpdatingUI = false;
        private int _currentPokemonIdx = 0;

        private ImageManager.PokemonIconAnimator _iconAnimator = null;
        private byte[] _currentFootprintData = null;
        private bool _isDrawingFootprint = false;
        private bool _drawingColorIsBlack = false;
        private Bitmap _battleAllyImage = null;
        private Bitmap _battleEnemyImage = null;
        private Bitmap _battleBaackgroundImage = null;
        private Bitmap _battleShadowImage = null;
        private Bitmap _battleBubbleImage = null;
        private Bitmap _itemUse1BackgroundImage = null;
        private Bitmap _itemUse2BackgroundImage = null;
        private bool _isItemUseCoordValid = false;

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

            LoadCoordBattleImages();
            LoadCoordItemUseImages();
            LoadAllDataToUI(_currentPokemonIdx);
        }

        private void InitializeManagers()
        {
            // name
            _pokemonNameManager = EntryManager<PokemonNameEntry>.Create(
                _romData, _tblReader, _config, "PokemonNameTableAddress", "PokemonNameCount");

            // sprite
            _spriteFrontImgManager = EntryManager<PokemonSpriteFrontImageEntry>.Create(
                _romData, _tblReader, _config, "PokemonSpriteFrontImageTableAddress", "PokemonSpriteCount");
            _spriteBackImgManager = EntryManager<PokemonSpriteBackImageEntry>.Create(
                _romData, _tblReader, _config, "PokemonSpriteBackImageTableAddress", "PokemonSpriteCount");
            _spriteNormalPalManager = EntryManager<PokemonSpriteNormalPaletteEntry>.Create(
                _romData, _tblReader, _config, "PokemonSpriteNormalPaletteTableAddress", "PokemonSpriteCount");
            _spriteShinyPalManager = EntryManager<PokemonSpriteShinyPaletteEntry>.Create(
                _romData, _tblReader, _config, "PokemonSpriteShinyPaletteTableAddress", "PokemonSpriteCount");

            // icon
            _iconImgManager = EntryManager<PokemonIconImageEntry>.Create(
                _romData, _tblReader, _config, "PokemonIconImageTableAddress", "PokemonIconCount");
            _iconPalIdxManager = EntryManager<PokemonIconPaletteIndexEntry>.Create(
                _romData, _tblReader, _config, "PokemonIconPaletteIndexTableAddress", "PokemonIconCount");
            _iconPalAddrManager = EntryManager<PokemonIconPaletteAddressEntry>.Create(
                _romData, _tblReader, _config, "PokemonIconPaletteAddressTableAddress", "PokemonIconPaletteAddressCount");

            // footprint
            _footprintImgManager = EntryManager<PokemonFootPrintImageEntry>.Create(
                _romData, _tblReader, _config, "PokemonFootprintTableAddress", "PokemonFootprintCount");

            // coordinate
            _coordBattleAllyManager = EntryManager<PokemonCoordBattleAllyEntry>.Create(
                _romData, _tblReader, _config, "PokemonCoordinateBattleAllyTableAddress", "PokemonCoordinateBattleCount");
            _coordBattleEnemyManager = EntryManager<PokemonCoordBattleEnemyEntry>.Create(
                _romData, _tblReader, _config, "PokemonCoordinateBattleEnemyTableAddress", "PokemonCoordinateBattleCount");
            _coordBattleEnemyShadowManager = EntryManager<PokemonCoordBattleEnemyShaowEntry>.Create(
                _romData, _tblReader, _config, "PokemonCoordinateBattleEnemyShadowTableAddress", "PokemonCoordinateBattleEnemyShadowCount");
            _coordItemUseManager = EntryManager<PokemonCoordItemUseEntry>.Create(
                _romData, _tblReader, _config, "PokemonCoordinateItemUseTableAddress", "PokemonCoordinateItemUseCount");

            // stats
            if (_config.GetBool("IsAppliedCFRU") && _config.GetBool("EnableStatsExpansion"))
            {
                _statsNormalManager = EntryManager<PokemonStatsNormalEntry>.Create(
                    _romData, _tblReader, _config, "PokemonStatsTableAddress", "PokemonStatsCount");
            }
            else
            {
                _statsExpansionManager = EntryManager<PokemonStatsExpansionEntry>.Create(
                    _romData, _tblReader, _config, "PokemonStatsTableAddress", "PokemonStatsCount");
            }






            // ability name
            _abilityNameManager = EntryManager<AbilityNameEntry>.Create(
                _romData, _tblReader, _config, "AbilityNameTableAddress", "AbilityNameCount");

            // item sprite
            _itemSpriteManager = EntryManager<ItemSpriteEntry>.Create(
                _romData, _tblReader, _config, "ItemSpriteTableAddress", "ItemDataCount");

            // item name
            _itemDataManager = EntryManager<ItemDataEntry>.Create(
                _romData, _tblReader, _config, "ItemDataTableAddress", "ItemDataCount");

            // type name
            _typeNameManager = EntryManager<TypeNameEntry>.Create(
                _romData, _tblReader, _config, "TypeNameTableAddress", "TypeNameCount");

            // move name
            _moveNameManager = EntryManager<MoveNameEntry>.Create(
                _romData, _tblReader, _config, "MoveNameTableAddress", "MoveNameCount");
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

            txtIconImgAddr.TextChanged += txtIconImgAddr_TextChanged;
            cmbIconPalIdx.SelectedIndexChanged += cmbIconPalIdx_SelectedIndexChanged;
            btnIconImport.Click += btnIconImport_Click;
            btnIconExport.Click += btnIconExport_Click;

            txtFootprintImgAddr.TextChanged += txtFootprintImgAddr_TextChanged;
            pnlFootprintCanvas.Paint += pnlFootprintCanvas_Paint;
            pnlFootprintCanvas.MouseDown += pnlFootprintCanvas_MouseDown;
            pnlFootprintCanvas.MouseMove += pnlFootprintCanvas_MouseMove;
            pnlFootprintCanvas.MouseUp += pnlFootprintCanvas_MouseUp;
            btnFootprintImport.Click += btnFootprintImport_Click;
            btnFootprintExport.Click += btnFootprintExport_Click;

            foreach (var nud in new[] {
                nudCoordBattleAllyBubbleX,
                nudCoordBattleAllyBubbleY,
                nudCoordBattleAllyPokemon,
                nudCoordBattleEnemyBubbleX,
                nudCoordBattleEnemyBubbleY,
                nudCoordBattleEnemyPokemon,
                nudCoordBattleEnemyShadowY})
            {
                nud.ValueChanged += UpdateCoordBattleDisplay;
            }
            chkShowBattleBubble.CheckedChanged += chkShowBattleBubble_CheckedChanged;

            nudCoordItemUse1X.ValueChanged += UpdateCoordItemUse1Preview;
            nudCoordItemUse1Y.ValueChanged += UpdateCoordItemUse1Preview;
            nudCoordItemUse2X.ValueChanged += UpdateCoordItemUse2Preview;
            nudCoordItemUse2Y.ValueChanged += UpdateCoordItemUse2Preview;
            nudCoordItemUse2Zoom.ValueChanged += UpdateCoordItemUse2Preview;
            rbCoordItemUse2Normal.CheckedChanged += UpdateCoordItemUse2Preview;
            rbCoordItemUse2Zoom.CheckedChanged += UpdateCoordItemUse2Preview;
            rbCoordItemUse2Normal.CheckedChanged += CoordItemUse2Mode_CheckedChanged;
            rbCoordItemUse2Zoom.CheckedChanged += CoordItemUse2Mode_CheckedChanged;




            cmbStatsHoldItem1.SelectedIndexChanged += HoldItemComboBox_SelectedIndexChanged;
            cmbStatsHoldItem2.SelectedIndexChanged += HoldItemComboBox_SelectedIndexChanged;
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

            // cmbIconPalIdx
            int iconPaletteAddressCount = _config.GetInt("PokemonIconPaletteAddressCount");
            cmbIconPalIdx.Items.Clear();
            for (int i = 0; i < iconPaletteAddressCount; i++)
            {
                cmbIconPalIdx.Items.Add($"パレット {i}");
            }
            cmbIconPalIdx.SelectedIndex = 0;

            // pnlFootprintCanvas
            typeof(Panel).GetProperty(
                "DoubleBuffered",
                BindingFlags.Instance | BindingFlags.NonPublic)
                ?.SetValue(pnlFootprintCanvas, true);

            // cmbStatsAbility
            var abilityNames = _abilityNameManager.Working
                             .Select(entry => entry._AbilityName)
                             .ToArray();
            cmbStatsAbility1.Items.AddRange(abilityNames);
            cmbStatsAbility2.Items.AddRange(abilityNames);
            cmbStatsAbilityHidden.Items.AddRange(abilityNames);

            // cmbStatsHoldItem
            var itemNames = _itemDataManager.Working
                             .Select(entry => entry._ItemName)
                             .ToArray();
            cmbStatsHoldItem1.Items.AddRange(itemNames);
            cmbStatsHoldItem2.Items.AddRange(itemNames);

            //cmbStatsType
            var typeNames = _typeNameManager.Working
                             .Select(entry => entry._TypeName)
                             .ToArray();
            cmbStatsType1.Items.AddRange(typeNames);
            cmbStatsType2.Items.AddRange(typeNames);

            ControlHelper.AttachAddressAutoFormat(
                txtSpriteFrontImgAddr, txtSpriteBackImgAddr, txtSpriteNormalPalAddr, txtSpriteShinyPalAddr,
                txtIconImgAddr,
                txtFootprintImgAddr);
            ControlHelper.AttachExternalBorder(
                picSpriteFrontNormal, picSpriteBackNormal, picSpriteFrontShiny, picSpriteBackShiny,
                picIconPal, picIcon, picIconAnimated,
                picFootprint, pnlFootprintCanvas,
                picCoordBattleDisplay, picCoordItemUse1, picCoordItemUse2,
                picStatsHoldItem1, picStatsHoldItem2);
            ControlHelper.AttachRadioButtonToTextBoxFocus(rbSpriteFrontImgAddr, txtSpriteFrontImgAddr);
            ControlHelper.AttachRadioButtonToTextBoxFocus(rbSpriteBackImgAddr, txtSpriteBackImgAddr);
            ControlHelper.AttachRadioButtonToTextBoxFocus(rbSpriteNormalPalAddr, txtSpriteNormalPalAddr);
            ControlHelper.AttachRadioButtonToTextBoxFocus(rbSpriteShinyPalAddr, txtSpriteShinyPalAddr);
            ControlHelper.LoadComboBoxFromTextFile(cmbStatsGrowthRate, "txt/PokemonStatsGrowthRate.txt");
            ControlHelper.LoadComboBoxFromTextFile(cmbStatsColor, "txt/PokemonStatsColor.txt");
            ControlHelper.LoadComboBoxFromTextFile(cmbStatsFlip, "txt/PokemonStatsFlip.txt");
            ControlHelper.LoadComboBoxFromTextFile(cmbStatsGender, "txt/PokemonStatsGender.txt");
            ControlHelper.LoadComboBoxFromTextFile(cmbStatsEggStep, "txt/PokemonStatsEggStep.txt");
            ControlHelper.LoadComboBoxFromTextFile(cmbStatsEggGroup1, "txt/PokemonStatsEggGroup.txt");
            ControlHelper.LoadComboBoxFromTextFile(cmbStatsEggGroup2, "txt/PokemonStatsEggGroup.txt");
        }

        private void InitializeUIStates()
        {
            _uiStateManager = new UIStateManager(hasChanges => btnSave.Enabled = hasChanges);
            btnSave.Enabled = false;
            _uiStateManager.AddControls(
                txtPokemonRename,
                txtSpriteFrontImgAddr, txtSpriteBackImgAddr, txtSpriteNormalPalAddr, txtSpriteShinyPalAddr,
                txtIconImgAddr, cmbIconPalIdx,
                txtFootprintImgAddr,
                nudCoordBattleAllyBubbleX, nudCoordBattleAllyBubbleY, nudCoordBattleAllyPokemon,
                nudCoordBattleEnemyBubbleX, nudCoordBattleEnemyBubbleY, nudCoordBattleEnemyPokemon, nudCoordBattleEnemyShadowY,
                nudCoordItemUse1X, nudCoordItemUse1Y, nudCoordItemUse2X, nudCoordItemUse2Y, nudCoordItemUse2Zoom,
                nudStatsHp, nudStatsAtk, nudStatsDef, nudStatsSpAtk, nudStatsSpDef, nudStatsSpeed,
                nudStatsEvHp, nudStatsEvAtk, nudStatsEvDef, nudStatsEvSpAtk, nudStatsEvSpDef, nudStatsEvSpeed,
                nudStatsCatchRate, nudStatsHappiness, nudStatsExp, cmbStatsGrowthRate, cmbStatsColor, cmbStatsFlip, nudStatsRunRate,
                cmbStatsGender, cmbStatsEggStep, cmbStatsEggGroup1, cmbStatsEggGroup2,
                cmbStatsAbility1, cmbStatsAbility2, cmbStatsAbilityHidden,
                cmbStatsHoldItem1, cmbStatsHoldItem2,
                cmbStatsType1, cmbStatsType2);
            _uiStateManager.AddBinaries(
                (pnlFootprintCanvas, null));
        }

        private void LoadAllDataToUI(int idx)
        {
            _isUpdatingUI = true;
            _reservationManager.ClearAllReservations();

            _currentPokemonIdx = idx;
            LoadPokemonNameToUI(idx);
            LoadSpritesToUI(idx);
            LoadIconToUI(idx);
            LoadFootprintToUI(idx);
            LoadCoordBattleToUI(idx);
            LoadCoordItemUseToUI(idx);

            _isUpdatingUI = false;
            _uiStateManager.UpdateInitialValues();
        }

        private void LoadPokemonNameToUI(int idx)
        {
            cmbPokemonName.SelectedIndex = idx;
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

            UpdateCoordBattleDisplay();
            UpdateCoordItemUseDisplay();
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

        private void LoadIconToUI(int idx)
        {
            DataBindingHelper.BindObjectToControls(this, _iconImgManager.Working[idx]);
            DataBindingHelper.BindObjectToControls(this, _iconPalIdxManager.Working[idx]);

            DisplayIconPalette();
            DisplayIcon();
        }

        private void txtIconImgAddr_TextChanged(object sender, EventArgs e)
        {
            if (_isUpdatingUI) return;
            DisplayIcon();
        }

        private void cmbIconPalIdx_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_isUpdatingUI) return;
            DisplayIconPalette();
            DisplayIcon();
        }

        private void DisplayIconPalette()
        {
            int paletteIndex = cmbIconPalIdx.SelectedIndex;
            var entry = _iconPalAddrManager.Working[paletteIndex];
            uint palettePtr = entry._IconPaletteAddr;

            if (palettePtr == 0) return;

            uint paletteAddress = palettePtr - GbaConstants.BaseAddr;
            Color[] colors = ImageManager.DecompressPalette(_romData, paletteAddress, false);

            var bmp = new Bitmap(picIconPal.Width, picIconPal.Height);
            int size = 10;
            using (Graphics g = Graphics.FromImage(bmp))
            {
                for (int i = 0; i < GbaConstants.PalColorCount; i++)
                {
                    if (i < colors.Length)
                    {
                        int x = (i % (GbaConstants.PalColorCount / 2)) * size;
                        int y = (i / (GbaConstants.PalColorCount / 2)) * size;
                        using (var b = new SolidBrush(colors[i]))
                        {
                            g.FillRectangle(b, x, y, size, size);
                        }
                    }
                }
            }

            picIconPal.Image?.Dispose();
            picIconPal.Image = bmp;
            picIconPal.Refresh();
        }

        private void DisplayIcon()
        {
            _iconAnimator?.StopAnimation();
            picIcon.Image?.Dispose();
            picIcon.Image = null;

            if (!GetCurrentIconData(out byte[] imageData, out Color[] colors)) return;

            Bitmap[] frames = ImageManager.CreatePokemonIconFrames(imageData, colors, true);

            if (frames != null)
            {
                // preview
                var fullIcon = new Bitmap(GbaConstants.IconFrameSize, GbaConstants.IconFrameSize * GbaConstants.IconFrameCounts);
                using (Graphics g = Graphics.FromImage(fullIcon))
                {
                    g.DrawImage(frames[0], 0, 0);
                    g.DrawImage(frames[1], 0, GbaConstants.IconFrameSize);
                }
                picIcon.Image = fullIcon;

                // animation
                Bitmap[] scaledFrames = new Bitmap[GbaConstants.IconFrameCounts];
                scaledFrames[0] = ImageManager.ScalePixelArt(frames[0]);
                scaledFrames[1] = ImageManager.ScalePixelArt(frames[1]);

                _iconAnimator = new ImageManager.PokemonIconAnimator(picIconAnimated);
                _iconAnimator.SetFrames(scaledFrames);
                _iconAnimator.StartAnimation();
            }
        }

        private bool GetCurrentIconData(out byte[] imageData, out Color[] colors)
        {
            imageData = null;
            colors = null;

            if (!ControlHelper.TryParseAddress(txtIconImgAddr.Text, out uint imageAddress))
            {
                return false;
            }

            // image data
            int dataSize = GbaConstants.IconBytesPerFrame * GbaConstants.IconFrameCounts;
            var res = _reservationManager.GetReservation(txtIconImgAddr);

            if (res != null)
            {
                imageData = res.Data;
            }
            else
            {
                imageData = new byte[dataSize];
                Array.Copy(_romData, imageAddress, imageData, 0, dataSize);
            }

            // palette data
            int paletteIndex = cmbIconPalIdx.SelectedIndex;
            var entry = _iconPalAddrManager.Working[paletteIndex];
            uint palettePtr = entry._IconPaletteAddr;

            if (palettePtr != 0)
            {
                uint paletteAddress = palettePtr - GbaConstants.BaseAddr;
                colors = ImageManager.DecompressPalette(_romData, paletteAddress, false);
            }

            return imageData != null && colors != null;
        }

        private void btnIconImport_Click(object sender, EventArgs e)
        {
            if (!ControlHelper.ValidateAndFormatInputTextBox(txtIconImportAddr, out uint? targetAddress)) return;

            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = GbaConstants.ImageImportFilter;
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    using (var bmp = new Bitmap(ofd.FileName))
                    {
                        if (!ImageManager.ExtractImageAndPalette(
                            bmp, 
                            GbaConstants.IconFrameSize,
                            GbaConstants.IconFrameSize * GbaConstants.IconFrameCounts,
                            out byte[] imageData, 
                            out Color[] palette))
                            return;

                        _reservationManager.SetReservation(txtIconImgAddr, (uint)targetAddress, imageData);
                        DisplayIcon();
                    }
                }
            }
        }

        private void btnIconExport_Click(object sender, EventArgs e)
        {
            if (!GetCurrentIconData(out byte[] imageData, out Color[] colors)) return;

            using (var exportBmp = ImageManager.CreateSprite(
                imageData, 
                colors,
                GbaConstants.IconFrameSize,
                GbaConstants.IconFrameSize * GbaConstants.IconFrameCounts, 
                true))
            {
                using (var sfd = new SaveFileDialog())
                {
                    sfd.Filter = GbaConstants.ImageExportFilter;
                    sfd.FileName = $"pokemon_icon_{(int)nudSpecies.Value:D4}";

                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        ImageManager.ExportIndexedImage(exportBmp, sfd.FileName);
                    }
                }
            }
        }

        private void LoadFootprintToUI(int idx)
        {
            if (idx >= _config.GetInt("NoFootprintStartIndex"))
            {
                ControlHelper.SetControlsEnabled(grpFootprint, false);
                ControlHelper.ResetControls(grpFootprint);

                picFootprint.Image?.Dispose();
                picFootprint.Image = null;
                _currentFootprintData = null;
                pnlFootprintCanvas.Invalidate();
            }

            else
            {
                ControlHelper.SetControlsEnabled(grpFootprint, true);
                DataBindingHelper.BindObjectToControls(this, _footprintImgManager.Working[idx]);
                DisplayFootprint();
            }
        }

        private void txtFootprintImgAddr_TextChanged(object sender, EventArgs e)
        {
            if (_isUpdatingUI) return;
            DisplayFootprint();
        }

        private void DisplayFootprint()
        {
            picFootprint.Image?.Dispose();
            picFootprint.Image = null;

            if (!ControlHelper.TryParseAddress(txtFootprintImgAddr.Text, out uint imageAddress)) return;

            var res = _reservationManager.GetReservation(txtFootprintImgAddr);
            if (res != null)
            {
                _currentFootprintData = (byte[])res.Data.Clone();
            }
            else
            {
                _currentFootprintData = new byte[GbaConstants.FootprintDataSize];
                Array.Copy(_romData, (int)imageAddress, _currentFootprintData, 0, GbaConstants.FootprintDataSize);
            }

            Color[] palette = { Color.White, Color.Black };
            using (var bmp = ImageManager.DecodeFootprint(_currentFootprintData, palette))
            {
                picFootprint.Image = ImageManager.ScalePixelArt(bmp, GbaConstants.DefaultScale);
            }

            pnlFootprintCanvas.Invalidate();
            _uiStateManager.UpdateBinary(pnlFootprintCanvas, _currentFootprintData);
        }

        private void pnlFootprintCanvas_Paint(object sender, PaintEventArgs e)
        {
            if (_currentFootprintData == null) return;

            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            e.Graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;

            Color[] palette = { Color.White, Color.Black };
            using (var bmp = ImageManager.DecodeFootprint(_currentFootprintData, palette))
            {
                e.Graphics.DrawImage(bmp, 0, 0, pnlFootprintCanvas.Width, pnlFootprintCanvas.Height);
            }

            // 16x16 grid
            int cellSize = GbaConstants.FootprintCanvasScale;
            for (int i = 0; i <= GbaConstants.FootprintSize; i++)
            {
                Color penColor = (i == GbaConstants.FootprintSize / 2) ? Color.Red : Color.Gray;
                using (var p = new Pen(penColor))
                {
                    e.Graphics.DrawLine(p, i * cellSize, 0, i * cellSize, pnlFootprintCanvas.Height);
                    e.Graphics.DrawLine(p, 0, i * cellSize, pnlFootprintCanvas.Width, i * cellSize);
                }
            }
        }

        private void pnlFootprintCanvas_MouseDown(object sender, MouseEventArgs e)
        {
            if (_currentFootprintData == null) return;

            if (e.Button == MouseButtons.Left)
            {
                _drawingColorIsBlack = true;
                _isDrawingFootprint = true;
            }
            else if (e.Button == MouseButtons.Right)
            {
                _drawingColorIsBlack = false;
                _isDrawingFootprint = true;
            }
            else
            {
                return;
            }

            ApplyFootprintDraw(e.X, e.Y);
        }

        private void pnlFootprintCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_isDrawingFootprint)
                return;

            ApplyFootprintDraw(e.X, e.Y);
        }

        private void pnlFootprintCanvas_MouseUp(object sender, MouseEventArgs e)
        {
            if (_isDrawingFootprint)
            {
                _isDrawingFootprint = false;
            }
          
        }

        private void ApplyFootprintDraw(int mouseX, int mouseY)
        {
            int x = mouseX / GbaConstants.FootprintCanvasScale;
            int y = mouseY / GbaConstants.FootprintCanvasScale;

            if (x < 0 || x >= GbaConstants.FootprintSize || y < 0 || y >= GbaConstants.FootprintSize) return;

            UpdateFootprintPixel(x, y,_drawingColorIsBlack);
            pnlFootprintCanvas.Invalidate();

            Color[] palette = { Color.White, Color.Black };
            using (var bmp = ImageManager.DecodeFootprint(_currentFootprintData, palette))
            {
                var oldImage = picFootprint.Image;
                picFootprint.Image = ImageManager.ScalePixelArt(bmp, GbaConstants.DefaultScale);
                oldImage?.Dispose();
            }

            _uiStateManager.UpdateBinary(pnlFootprintCanvas, _currentFootprintData);
        }

        private void UpdateFootprintPixel(int x, int y, bool isBlack)
        {
            int blockX = x / GbaConstants.FootprintTileSize;
            int blockY = y / GbaConstants.FootprintTileSize;
            int blockIndex = blockY * GbaConstants.FootprintBlockDim + blockX;

            int localX = x % GbaConstants.FootprintTileSize;
            int localY = y % GbaConstants.FootprintTileSize;

            int byteIndex = blockIndex * GbaConstants.FootprintTileSize + localY;

            if (isBlack)
            {
                _currentFootprintData[byteIndex] = (byte)(_currentFootprintData[byteIndex] | (1 << localX));
            }
            else
            {
                _currentFootprintData[byteIndex] = (byte)(_currentFootprintData[byteIndex] & ~(1 << localX));
            }
        }

        private void btnFootprintImport_Click(object sender, EventArgs e)
        {
            if (!ControlHelper.ValidateAndFormatInputTextBox(txtFootprintImportAddr, out uint? targetAddress)) return;

            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = GbaConstants.ImageImportFilter;

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    using (var bmp = new Bitmap(ofd.FileName))
                    {
                        if (!ImageManager.ExtractFootprint(bmp, out byte[] footprintData)) return;

                        _reservationManager.SetReservation(txtFootprintImgAddr, (uint)targetAddress, footprintData);
                        DisplayFootprint();
                    }
                }
            }
        }

        private void btnFootprintExport_Click(object sender, EventArgs e)
        {
            if (_currentFootprintData == null) return;

            using (var bmp = ImageManager.ConvertFootprintToBitmap(_currentFootprintData))
            {
                if (bmp == null) return;

                using (var sfd = new SaveFileDialog())
                {
                    sfd.Filter = GbaConstants.ImageExportFilter;
                    sfd.FileName = $"pokemon_footprint_{(int)nudSpecies.Value:D4}";

                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        ImageManager.ExportIndexedImage(bmp, sfd.FileName);
                    }
                }
            }
        }

        private void LoadCoordBattleImages()
        {
            _battleBaackgroundImage = (Bitmap)Image.FromFile("img/PokemonCoordBattleBackground.png");

            var shadowBmp = new Bitmap("img/PokemonCoordBattleShadow.png");
            shadowBmp.MakeTransparent();
            _battleShadowImage = shadowBmp;

            var bubbleBmp = new Bitmap("img/PokemonCoordBattleBubble.png");
            bubbleBmp.MakeTransparent();
            _battleBubbleImage = bubbleBmp;
        }

        private void LoadCoordBattleToUI(int idx)
        {
            DataBindingHelper.BindObjectToControls(this, _coordBattleAllyManager.Working[idx]);
            DataBindingHelper.BindObjectToControls(this, _coordBattleEnemyManager.Working[idx]);
            DataBindingHelper.BindObjectToControls(this, _coordBattleEnemyShadowManager.Working[idx]);

            UpdateCoordBattleDisplay();
        }

        private void UpdateCoordBattleDisplay(object sender = null, EventArgs e = null)
        {
            if (_isUpdatingUI && sender != null) return;

            var canvas = new Bitmap(picCoordBattleDisplay.Width, picCoordBattleDisplay.Height);
            using (Graphics g = Graphics.FromImage(canvas))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;

                // background
                g.DrawImage(_battleBaackgroundImage, 0, 0, picCoordBattleDisplay.Width, picCoordBattleDisplay.Height);

                // shadow
                if (nudCoordBattleEnemyShadowY.Value != 0)
                {
                    g.DrawImage(_battleShadowImage, GbaConstants.BattleEnemyShadowX, GbaConstants.BattleEnemyShadowY,
                                _battleShadowImage.Width, _battleShadowImage.Height);
                }

                // ally
                if (_battleAllyImage != null)
                {
                    int yPosition = GbaConstants.BattleAllyY + (int)nudCoordBattleAllyPokemon.Value;
                    g.DrawImage(_battleAllyImage, GbaConstants.BattleAllyX, yPosition,
                                _battleAllyImage.Width, _battleAllyImage.Height);
                }

                // enemy
                if (_battleEnemyImage != null)
                {
                    int yPosition = GbaConstants.BattleEnemyY + (int)nudCoordBattleEnemyPokemon.Value;

                    if (nudCoordBattleEnemyShadowY.Value != 0)
                    {
                        yPosition -= (int)nudCoordBattleEnemyShadowY.Value;
                    }
                    g.DrawImage(_battleEnemyImage, GbaConstants.BattleEnemyX, yPosition,
                                _battleEnemyImage.Width, _battleEnemyImage.Height);
                }

                // bubble
                if (chkShowBattleBubble.Checked)
                {
                    // ally
                    int allyX = GbaConstants.BattleAllyBubbleX + ((int)nudCoordBattleAllyBubbleX.Value * GbaConstants.BattleBubbleMultiplier);
                    int allyY = GbaConstants.BattleAllyBubbleY - ((int)nudCoordBattleAllyBubbleY.Value * GbaConstants.BattleBubbleMultiplier)
                                + (int)nudCoordBattleAllyPokemon.Value;
                    g.DrawImage(_battleBubbleImage, allyX, allyY, _battleBubbleImage.Width, _battleBubbleImage.Height);

                    // enemy
                    int enemyX = GbaConstants.BattleEnemyBubbleX - ((int)nudCoordBattleEnemyBubbleX.Value * GbaConstants.BattleBubbleMultiplier);
                    int enemyY = GbaConstants.BattleEnemyBubbleY - ((int)nudCoordBattleEnemyBubbleY.Value * GbaConstants.BattleBubbleMultiplier)
                                 + (int)nudCoordBattleEnemyPokemon.Value - (int)nudCoordBattleEnemyShadowY.Value;
                    g.DrawImage(_battleBubbleImage, enemyX, enemyY, _battleBubbleImage.Width, _battleBubbleImage.Height);
                }
            }

            // discard
            picCoordBattleDisplay.Image?.Dispose();
            picCoordBattleDisplay.Image = null;

            picCoordBattleDisplay.Image = canvas;
        }

        private void chkShowBattleBubble_CheckedChanged(object sender, EventArgs e)
        {
            UpdateCoordBattleDisplay(sender, e);
        }

        private void LoadCoordItemUseImages()
        {
            _itemUse1BackgroundImage = (Bitmap)Image.FromFile("img/PokemonCoordItemUse1Background.png");
            _itemUse2BackgroundImage = (Bitmap)Image.FromFile("img/PokemonCoordItemUse2Background.png");
        }

        private void LoadCoordItemUseToUI(int idx)
        {
            int coordinateIndex = idx - _config.GetInt("PokemonCoordinateItemUseStartIndex");
            bool isValid = coordinateIndex >= 0 && coordinateIndex < _coordItemUseManager.Count;
            _isItemUseCoordValid = isValid;

            if (isValid)
            {
                ControlHelper.SetControlsEnabled(grpCoordItemUse, true);
                DataBindingHelper.BindObjectToControls(this, _coordItemUseManager.Working[coordinateIndex]);
            }
            else
            {
                ControlHelper.SetControlsEnabled(grpCoordItemUse, false);
                ControlHelper.ResetControls(grpCoordItemUse, excludeTypes: new[] { typeof(RadioButton) });
            }

            UpdateCoordItemUseDisplay();
            CoordItemUse2Mode_CheckedChanged(null, null);
        }

        private void UpdateCoordItemUseDisplay()
        {
            UpdateCoordItemUse1Preview();
            UpdateCoordItemUse2Preview();
        }

        private void UpdateCoordItemUse1Preview(object sender = null, EventArgs e = null)
        {
            if (_isUpdatingUI && sender != null) return;

            if (!_isItemUseCoordValid)
            {
                picCoordItemUse1.Image?.Dispose();
                picCoordItemUse1.Image = null;
                return;
            }

            if (_battleEnemyImage == null) return;

            Bitmap canvas = new Bitmap(picCoordItemUse1.Width, picCoordItemUse1.Height);
            using (Graphics g = Graphics.FromImage(canvas))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                g.DrawImage(_itemUse1BackgroundImage, 0, 0);
                g.DrawImage(_battleEnemyImage, GbaConstants.ItemUseAnimPokeX, GbaConstants.ItemUseAnimPokeY);

                using (Bitmap itemImage = GetItemSprite(GbaConstants.ItemUse1PreviewItemIdx, false))
                {
                    if (itemImage != null)
                    {
                        int itemX = GbaConstants.ItemUseAnimItemX + (int)nudCoordItemUse1X.Value;
                        int itemY = GbaConstants.ItemUseAnimItemY + (int)nudCoordItemUse1Y.Value;
                        g.DrawImage(itemImage, itemX, itemY);
                    }
                }
            }

            picCoordItemUse1.Image?.Dispose();
            picCoordItemUse1.Image = null;

            picCoordItemUse1.Image = canvas;
        }

        private void UpdateCoordItemUse2Preview(object sender = null, EventArgs e = null)
        {
            if (_isUpdatingUI && sender != null) return;

            if (!_isItemUseCoordValid)
            {
                picCoordItemUse2.Image?.Dispose();
                picCoordItemUse2.Image = null;
                return;
            }

            if (_battleEnemyImage == null) return;

            Bitmap canvas = new Bitmap(picCoordItemUse2.Width, picCoordItemUse2.Height);
            using (Graphics g = Graphics.FromImage(canvas))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                g.DrawImage(_itemUse2BackgroundImage, 0, 0);

                if (rbCoordItemUse2Normal.Checked)
                {
                    // normal
                    g.DrawImage(_battleEnemyImage, GbaConstants.ItemUseAnimPokeX, GbaConstants.ItemUseAnimPokeY);

                    using (Bitmap itemImage = GetItemSprite(GbaConstants.ItemUse2PreviewItemIdx, false))
                    {
                        if (itemImage != null)
                        {
                            int itemX = GbaConstants.ItemUseAnimItemX + (int)nudCoordItemUse2X.Value;
                            int itemY = GbaConstants.ItemUseAnimItemY + (int)nudCoordItemUse2Y.Value;
                            g.DrawImage(itemImage, itemX, itemY);
                        }
                    }
                }
                else
                {
                    // zoom, magic number
                    using (Bitmap scaledPokemon = ImageManager.ScalePixelArt(_battleEnemyImage, 2))
                    using (Bitmap scaledItem = GetItemSprite(GbaConstants.ItemUse2PreviewItemIdx, false))
                    {
                        Bitmap itemScaled = null;
                        if (scaledItem != null)
                        {
                            itemScaled = ImageManager.ScalePixelArt(scaledItem, 2);
                        }

                        int pokemonCenterX = GbaConstants.ItemUseAnimPokeX + 32;
                        int pokemonCenterY = GbaConstants.ItemUseAnimPokeY + 32;
                        int zoomedCenterY = pokemonCenterY + (int)nudCoordItemUse2Zoom.Value;

                        int scaledWidth = _itemUse2BackgroundImage.Width * 2;
                        int scaledHeight = _itemUse2BackgroundImage.Height * 2;

                        int drawX = pokemonCenterX - (scaledWidth / 2);
                        int drawY = zoomedCenterY - (scaledHeight / 2) - 8;

                        g.DrawImage(scaledPokemon, drawX + GbaConstants.ItemUseAnimPokeX * 2, drawY + GbaConstants.ItemUseAnimPokeY * 2);

                        if (itemScaled != null)
                        {
                            int itemX = drawX + (GbaConstants.ItemUseAnimItemX + (int)nudCoordItemUse2X.Value - 2) * 2;
                            int itemY = drawY + (GbaConstants.ItemUseAnimItemY + (int)nudCoordItemUse2Y.Value) * 2;
                            g.DrawImage(itemScaled, itemX, itemY);
                            itemScaled.Dispose();
                        }
                    }
                }
            }

            picCoordItemUse2.Image?.Dispose();
            picCoordItemUse2.Image = null;

            picCoordItemUse2.Image = canvas;
        }

        private void CoordItemUse2Mode_CheckedChanged(object sender, EventArgs e)
        {
            if (!_isItemUseCoordValid) return;

            bool isNormal = rbCoordItemUse2Normal.Checked;
            nudCoordItemUse2X.Enabled = isNormal;
            nudCoordItemUse2Y.Enabled = isNormal;
            nudCoordItemUse2Zoom.Enabled = !isNormal;
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

        private void UpdateHoldItemImages()
        {
            var itemControls = new[]
            {
                (Combo: cmbStatsHoldItem1, Pic: picStatsHoldItem1),
                (Combo: cmbStatsHoldItem2, Pic: picStatsHoldItem2) 
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















        private void HoldItemComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_isUpdatingUI) return;
            UpdateHoldItemImages();
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
            SaveCurrentIcon(idx);
            SaveCurrentFootprint(idx);
            SaveCurrentCoordBattle(idx);
            SaveCurrentCoordItemUse(idx);
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
            if (_config.GetBool("IsAppliedCFRU")) // FF FF FF ...
            {
                _pokemonNameManager.Save(idx, true, GbaConstants.FreeSpaceByte, GbaConstants.FreeSpaceByte);
            }
            else
            {
                _pokemonNameManager.Save(idx); // ... FF 00 00
            }
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

        private void SaveCurrentIcon(int idx)
        {
            var res = _reservationManager.GetReservation(txtIconImgAddr);
            if (res != null && res.Data != null)
            {
                Array.Copy(res.Data, 0, _romData, (int)res.Address, res.Data.Length);
                _reservationManager.ClearReservation(txtIconImgAddr);
            }

            DataBindingHelper.BindControlsToObject(this, _iconImgManager.Working[idx]);
            DataBindingHelper.BindControlsToObject(this, _iconPalIdxManager.Working[idx]);
            _iconImgManager.Save(idx);
            _iconPalIdxManager.Save(idx);
        }

        private void SaveCurrentFootprint(int idx)
        {
            if (idx >= _config.GetInt("NoFootprintStartIndex")) return;
            if (!ControlHelper.TryParseAddress(txtFootprintImgAddr.Text, out uint imageAddress)) return;

            var res = _reservationManager.GetReservation(txtFootprintImgAddr);
            if (res != null && res.Data != null)
            {
                Array.Copy(res.Data, 0, _romData, (int)res.Address, res.Data.Length);
                _reservationManager.ClearReservation(txtFootprintImgAddr);
            }

            if (_uiStateManager.HasBinaryChanges(pnlFootprintCanvas) && _currentFootprintData != null)
            {
                Array.Copy(_currentFootprintData, 0, _romData, (int)imageAddress, GbaConstants.FootprintDataSize);
            }

            DataBindingHelper.BindControlsToObject(this, _footprintImgManager.Working[idx]);
            _footprintImgManager.Save(idx);
        }

        private void SaveCurrentCoordBattle(int idx)
        {
            DataBindingHelper.BindControlsToObject(this, _coordBattleAllyManager.Working[idx]);
            DataBindingHelper.BindControlsToObject(this, _coordBattleEnemyManager.Working[idx]);
            DataBindingHelper.BindControlsToObject(this, _coordBattleEnemyShadowManager.Working[idx]);
            _coordBattleAllyManager.Save(idx);
            _coordBattleEnemyManager.Save(idx);
            _coordBattleEnemyShadowManager.Save(idx);
        }

        private void SaveCurrentCoordItemUse(int idx)
        {
            int coordinateIndex = idx - _config.GetInt("PokemonCoordinateItemUseStartIndex");
            if (coordinateIndex < 0 || coordinateIndex >= _coordItemUseManager.Count) return;

            DataBindingHelper.BindControlsToObject(this, _coordItemUseManager.Working[coordinateIndex]);
            _coordItemUseManager.Save(coordinateIndex);
        }
    }
}
