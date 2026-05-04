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
        private EntryManager<PokemonEvolutionEntry> _evoManager;
        private EntryManager<PokemonLearnsetEntry> _learnsetManager;
        private EntryManager<TmHmMoveEntry> _tmHmListManager;
        private EntryManager<TutorMoveEntry> _tutorListManager;
        private EntryManager<PokedexOrderEntry> _orderManager;
        private EntryManager<PokedexEntry> _dexManager;

        private EntryManager<AbilityNameEntry> _abilityNameManager;
        private EntryManager<ItemSpriteEntry> _itemSpriteManager;
        private EntryManager<ItemDataEntry> _itemDataManager;
        private EntryManager<TypeNameEntry> _typeNameManager;
        private EntryManager<MoveNameEntry> _moveNameManager;
        private EntryManager<TrainerSpriteImageEntry> _trainerImgManager;
        private EntryManager<TrainerSpritePaletteEntry> _trainerPalManager;

        private bool _isUpdatingUI = false;
        private int _currentPokemonIdx = 0;
        private int _currentdexOrder = -1;

        private ImageManager.PokemonIconAnimator _iconAnimator = null;
        private byte[] _currentFootprintData = null;
        private bool _isDrawingFootprint = false;
        private bool _drawingColorIsBlack = false;
        private Bitmap _battleAllyImage = null;
        private Bitmap _battleEnemyImage = null;
        private Bitmap _currentIconFrame = null;
        private Bitmap _battleBaackgroundImage = null;
        private Bitmap _battleShadowImage = null;
        private Bitmap _battleBubbleImage = null;
        private Bitmap _itemUse1BackgroundImage = null;
        private Bitmap _itemUse2BackgroundImage = null;
        private bool _isItemUseCoordValid = false;
        private int _currentEvoSlotIndex = 0;
        private List<PokemonEvolutionEntry[]> _originalEvoSlots = new List<PokemonEvolutionEntry[]>();
        private List<PokemonEvolutionEntry[]> _workingEvoSlots = new List<PokemonEvolutionEntry[]>();
        private byte[] _currentDexDescData = null;
        private Bitmap _dexSizeCompBackgroundImage = null;

        private class EvolutionMethodInfo
        {
            public string MethodName { get; set; }
            public string Param1 { get; set; }
            public string Param2 { get; set; }
        }
        private List<EvolutionMethodInfo> _evolutionMethodInfos = new List<EvolutionMethodInfo>();

        public class LearnsetList
        {
            public int Level { get; set; }
            public int MoveIdx { get; set; }
            public string MoveName { get; set; }

            public override string ToString() => $"Lv{Level:D2} {MoveName}";
        }
        private List<LearnsetList> _currentLearnsetList = new List<LearnsetList>();

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
            LoadPokedexSizeCompImages();
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
                _statsExpansionManager = EntryManager<PokemonStatsExpansionEntry>.Create(
                    _romData, _tblReader, _config, "PokemonStatsTableAddress", "PokemonStatsCount");
            }
            else
            {
                _statsNormalManager = EntryManager<PokemonStatsNormalEntry>.Create(
                    _romData, _tblReader, _config, "PokemonStatsTableAddress", "PokemonStatsCount");
            }

            // evolution
            uint? evoTableAddr = _config.GetAddr("PokemonEvolutionTableAddress");
            int evoSlotCount = _config.GetInt("PokemonEvolutionSlotCount");
            int evoEntryCount = _config.GetInt("PokemonEvolutionEntryCount");
            _evoManager = new EntryManager<PokemonEvolutionEntry>(_romData, _tblReader);
            _evoManager.Load(evoTableAddr, evoSlotCount * evoEntryCount);
            for (int i = 0; i < evoEntryCount; i++)
            {
                int start = i * evoSlotCount;
                var originalSlots = _evoManager.Original.Skip(start).Take(evoSlotCount).ToArray();
                var workingSlots = _evoManager.Working.Skip(start).Take(evoSlotCount).ToArray();
                _originalEvoSlots.Add(originalSlots);
                _workingEvoSlots.Add(workingSlots);
            }

            // learnset
            _learnsetManager = EntryManager<PokemonLearnsetEntry>.Create(
                _romData, _tblReader, _config, "PokemonLearnsetTableAddress", "PokemonLearnsetEntryCount");

            // tm hm
            _tmHmListManager = EntryManager<TmHmMoveEntry>.Create(
                _romData, _tblReader, _config, "TmHmListTableAddress", "TmHmCount");

            // tutor
            _tutorListManager = EntryManager<TutorMoveEntry>.Create( 
                _romData, _tblReader, _config, "TutorListTableAddress", "TutorCount");

            //order
            _orderManager = EntryManager<PokedexOrderEntry>.Create(
                _romData, _tblReader, _config, "PokedexOrderTableAddress", "PokedexOrderCount");

            //pokedex
            _dexManager = EntryManager<PokedexEntry>.Create(
                _romData, _tblReader, _config, "PokedexTableAddress", "PokedexCount");





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

            lstEvoSlots.SelectedIndexChanged += lstEvoSlots_SelectedIndexChanged;
            cmbEvoToPokemon.SelectedIndexChanged += cmbEvoToPokemon_SelectedIndexChanged;
            cmbEvoCondMethod.SelectedIndexChanged += cmbEvoCondMethod_SelectedIndexChanged;
            cmbEvoToPokemon.SelectedIndexChanged += OnEvolutionUIChanged;
            cmbEvoCondMethod.SelectedIndexChanged += OnEvolutionUIChanged;
            foreach (var nud in new[] {
                nudEvoCondParam1A,
                nudEvoCondParam1B,
                nudEvoCondParam2A,
                nudEvoCondParam2B})
            {
                nud.ValueChanged += OnEvolutionUIChanged;
            }
            rbEvoInputAssistPokemon.CheckedChanged += EvoInputAssist_CheckedChanged;
            rbEvoInputAssistType.CheckedChanged += EvoInputAssist_CheckedChanged;
            rbEvoInputAssistItem.CheckedChanged += EvoInputAssist_CheckedChanged;
            rbEvoInputAssistMove.CheckedChanged += EvoInputAssist_CheckedChanged;
            btnEvoInputAssistParam1.Click += btnEvoInputAssistParam1_Click;
            btnEvoInputAssistParam2.Click += btnEvoInputAssistParam2_Click;

            txtLearnsetAddr.TextChanged += UpdateLearnsetDisplay;
            lstLearnset.SelectedIndexChanged += lstLearnset_SelectedIndexChanged;
            nudLearnsetLevel.ValueChanged += OnLevelMoveUIChanged;
            cmbLearnsetMove.SelectedIndexChanged += OnLevelMoveUIChanged;
            btnCreateNewLearnset.Click += btnCreateNewLearnset_Click;
            clbTmHm.ItemCheck += (s, e) => HandleLearnFlagItemCheck(clbTmHm, "TmHmCount", "TmHmData");
            clbTutor.ItemCheck += (s, e) => HandleLearnFlagItemCheck(clbTutor, "TutorCount", "TutorData");

            txtDexCategory.TextChanged += txtDexCategory_TextChanged;
            nudDexHeight.ValueChanged += PokedexInfoNudUnit_ValueChanged;
            nudDexWeight.ValueChanged += PokedexInfoNudUnit_ValueChanged;
            txtDexDescAddr.TextChanged += txtDexDescAddr_TextChanged;
            txtDexDescString.TextChanged += txtDexDescString_TextChanged;

            foreach (var nud in new[] {
                nudDexSizeCompParam1,
                nudDexSizeCompParam2,
                nudDexSizeCompParam3,
                nudDexSizeCompParam4,
                nudDexSizeCompTrainerSpriteIdx})
            {
                nud.ValueChanged += SizeCompParam_ValueChanged;
            }
        }

        private void InitializeControls()
        {
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

            // nudStatsExp
            if (_config.GetBool("IsAppliedCFRU") && _config.GetBool("EnableStatsExpansion"))
            {
                nudStatsExp.Maximum = GbaConstants.Mask16Bits;
            }

            // lstEvoSlots
            int evoSlotCount = _config.GetInt("PokemonEvolutionSlotCount");
            lstEvoSlots.Items.Clear();
            for (int i = 0; i < evoSlotCount; i++)
            {
                lstEvoSlots.Items.Add($"進化スロット {i + 1}");
            }

            // cmbEvoCondMethod
            InitializePokemonEvolutionMethod();

            // nudLearnsetLevel
            nudLearnsetLevel.Maximum =
                _config.GetBool("IsAppliedCFRU") && _config.GetBool("EnableLearnsetExpansion")
                ? GbaConstants.LearnsetMaxLevel3Byte
                : GbaConstants.LearnsetMaxLevel2Byte;

            // clbTmHm
            clbTmHm.BeginUpdate();
            clbTmHm.Items.Clear();
            for (int i = 0; i < _config.GetInt("TmHmCount"); i++)
            {
                int moveIdx = (int)_tmHmListManager.Working[i]._MoveIdx;
                string moveName = _moveNameManager.Original[moveIdx]._MoveName;
                if (i < _config.GetInt("TmCount"))
                {
                    clbTmHm.Items.Add($"TM{i + 1:00} - {moveName}");
                }
                else
                {
                    clbTmHm.Items.Add($"HM{i - _config.GetInt("TmCount") + 1:00} - {moveName}");
                }
            }
            clbTmHm.EndUpdate();

            // clbTutor
            clbTutor.BeginUpdate();
            clbTutor.Items.Clear();
            for (int i = 0; i < _config.GetInt("TutorCount"); i++)
            {
                int moveIdx = (int)_tutorListManager.Working[i]._MoveIdx;
                string moveName = _moveNameManager.Original[moveIdx]._MoveName;
                clbTutor.Items.Add($"No.{i + 1:00} - {moveName}");

            }
            clbTmHm.EndUpdate();

            // pokemon name for cmb
            var classNames = _pokemonNameManager.Working
                             .Select(entry => entry._PokemonName)
                             .ToArray();
            cmbPokemonName.Items.AddRange(classNames);
            cmbEvoToPokemon.Items.AddRange(classNames);
            cmbEvoInputAssistPokemon.Items.AddRange(classNames);

            // ability for cmb
            var abilityNames = _abilityNameManager.Working
                             .Select(entry => entry._AbilityName)
                             .ToArray();
            cmbStatsAbility1.Items.AddRange(abilityNames);
            cmbStatsAbility2.Items.AddRange(abilityNames);
            cmbStatsAbilityHidden.Items.AddRange(abilityNames);

            // item for cmb
            var itemNames = _itemDataManager.Working
                             .Select(entry => entry._ItemName)
                             .ToArray();
            cmbStatsHoldItem1.Items.AddRange(itemNames);
            cmbStatsHoldItem2.Items.AddRange(itemNames);
            cmbEvoInputAssistItem.Items.AddRange(itemNames);

            // type for cmb
            var typeNames = _typeNameManager.Working
                             .Select(entry => entry._TypeName)
                             .ToArray();
            cmbStatsType1.Items.AddRange(typeNames);
            cmbStatsType2.Items.AddRange(typeNames);
            cmbEvoInputAssistType.Items.AddRange(typeNames);

            // move for cmb
            var moveNames = _moveNameManager.Working
                             .Select(entry => entry._MoveName)
                             .ToArray();
            cmbEvoInputAssistMove.Items.AddRange(moveNames);
            cmbLearnsetMove.Items.AddRange(moveNames);

            // grpEvoInputAssist
            foreach (var cmb in new[] {
                cmbEvoInputAssistPokemon,
                cmbEvoInputAssistType,
                cmbEvoInputAssistItem,
                cmbEvoInputAssistMove })
            {
                cmb.SelectedIndex = 0;
            }
            rbEvoInputAssistPokemon.Checked = true;

            // nudPokedexInfoSizeComparisonTrainerId
            nudDexSizeCompTrainerSpriteIdx.Maximum = _config.GetInt("TrainerSpriteCount") -1;

            ControlHelper.AttachAddressAutoFormat(
                txtSpriteFrontImgAddr, txtSpriteBackImgAddr, txtSpriteNormalPalAddr, txtSpriteShinyPalAddr,
                txtIconImgAddr,
                txtFootprintImgAddr,
                txtLearnsetAddr,
                txtDexDescAddr);
            ControlHelper.AttachExternalBorder(
                picSpriteFrontNormal, picSpriteBackNormal, picSpriteFrontShiny, picSpriteBackShiny,
                picIconPal, picIcon, picIconAnimated,
                picFootprint, pnlFootprintCanvas,
                picCoordBattleDisplay, picCoordItemUse1, picCoordItemUse2,
                picStatsHoldItem1, picStatsHoldItem2,
                picEvoToIcon,
                picDexSizeCompPreview);
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
                cmbStatsType1, cmbStatsType2,
                txtDexCategory, nudDexHeight, nudDexWeight, txtDexDescAddr,
                nudDexSizeCompParam1, nudDexSizeCompParam2, nudDexSizeCompParam3, nudDexSizeCompParam4);
            _uiStateManager.AddBinaries(
                (pnlFootprintCanvas, null),
                (lstEvoSlots, null),
                (lstLearnset, null),
                (txtDexDescString, null));
        }

        private void LoadAllDataToUI(int idx)
        {
            _isUpdatingUI = true;
            _reservationManager.ClearAllReservations();

            _currentPokemonIdx = idx;
            _currentEvoSlotIndex = 0;

            LoadPokemonNameToUI(idx);
            LoadSpritesToUI(idx);
            LoadIconToUI(idx);
            LoadFootprintToUI(idx);
            LoadCoordBattleToUI(idx);
            LoadCoordItemUseToUI(idx);
            LoadStatsToUI(idx);
            LoadEvolutionsToUI(idx);
            LoadLearnsetsToUI(idx);
            LoadPokedexToUI(idx);

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
            cmbEvoToPokemon.Items[idx] = validName;
            cmbEvoInputAssistPokemon.Items[idx] = validName;
            _pokemonNameManager.Working[idx]._PokemonName = validName;

            _isUpdatingUI = false;
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
                        DiscardData(_currentPokemonIdx);
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
            UpdateEvoToIcon();
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

            // cache
            _currentIconFrame?.Dispose();
            _currentIconFrame = null;

            if (!GetCurrentIconData(out byte[] imageData, out Color[] colors)) return;

            Bitmap[] frames = ImageManager.CreatePokemonIconFrames(imageData, colors, true);

            if (frames != null)
            {
                // cache
                _currentIconFrame = (Bitmap)frames[0].Clone();

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

        private void LoadStatsToUI(int idx)
        {
            if (_config.GetBool("IsAppliedCFRU") && _config.GetBool("EnableStatsExpansion"))
            {
                DataBindingHelper.BindObjectToControls(this, _statsExpansionManager.Working[idx]);
                ApplyEvsToControls(_statsExpansionManager.Working[idx]._StatsEvs);
            }
            else
            {
                DataBindingHelper.BindObjectToControls(this, _statsNormalManager.Working[idx]);
                ApplyEvsToControls(_statsNormalManager.Working[idx]._StatsEvs);
            }

            UpdateHoldItemImages();
        }

        private void ApplyEvsToControls(ushort evValue)
        {
            byte[] evs = DecodeEv(evValue);
            nudStatsEvHp.Value = evs[0];
            nudStatsEvAtk.Value = evs[1];
            nudStatsEvDef.Value = evs[2];
            nudStatsEvSpAtk.Value = evs[3];
            nudStatsEvSpDef.Value = evs[4];
            nudStatsEvSpeed.Value = evs[5];
        }

        private byte[] DecodeEv(ushort evValue)
        {
            byte[] evs = new byte[6];
            byte[] bytes = BitConverter.GetBytes(evValue);

            // low byte
            evs[0] = (byte)((bytes[0] >> GbaConstants.EvShiftHp) & GbaConstants.Mask2Bits);         // HP
            evs[1] = (byte)((bytes[0] >> GbaConstants.EvShiftAtk) & GbaConstants.Mask2Bits);        // Attack
            evs[2] = (byte)((bytes[0] >> GbaConstants.EvShiftDefense) & GbaConstants.Mask2Bits);    // Defense
            evs[5] = (byte)((bytes[0] >> GbaConstants.EvShiftSpeed) & GbaConstants.Mask2Bits);      // Speed

            // high byte
            evs[3] = (byte)((bytes[1] >> GbaConstants.EvShiftSpAtk) & GbaConstants.Mask2Bits);      // SpAttack
            evs[4] = (byte)((bytes[1] >> GbaConstants.EvShiftSpDef) & GbaConstants.Mask2Bits);      // SpDefense

            return evs;
        }

        private ushort EncodeEv(byte[] evs)
        {
            byte lowByte = 0;
            byte highByte = 0;

            // low byte
            lowByte = (byte)(lowByte | ((evs[0] & GbaConstants.Mask2Bits) << GbaConstants.EvShiftHp));          // HP
            lowByte = (byte)(lowByte | ((evs[1] & GbaConstants.Mask2Bits) << GbaConstants.EvShiftAtk));         // Attack
            lowByte = (byte)(lowByte | ((evs[2] & GbaConstants.Mask2Bits) << GbaConstants.EvShiftDefense));     // Defense
            lowByte = (byte)(lowByte | ((evs[5] & GbaConstants.Mask2Bits) << GbaConstants.EvShiftSpeed));       // Speed

            // igh byte
            highByte = (byte)(highByte | ((evs[3] & GbaConstants.Mask2Bits) << GbaConstants.EvShiftSpAtk));     // SpAttack
            highByte = (byte)(highByte | ((evs[4] & GbaConstants.Mask2Bits) << GbaConstants.EvShiftSpDef));     // SpDefense

            return BitConverter.ToUInt16(new byte[] { lowByte, highByte }, 0);
        }

        private void InitializePokemonEvolutionMethod()
        {
            _evolutionMethodInfos.Clear();
            cmbEvoCondMethod.BeginUpdate();
            cmbEvoCondMethod.Items.Clear();

            string filePath = Path.Combine(Application.StartupPath, "txt", "PokemonEvolutionMethod.txt");

            if (File.Exists(filePath))
            {
                string[] lines = File.ReadAllLines(filePath, System.Text.Encoding.UTF8);

                foreach (string line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        continue;
                    }

                    string[] parts = line.Split(';');
                    var info = new EvolutionMethodInfo
                    {
                        MethodName = parts[0].Trim(),
                        Param1 = parts[1].Trim(),
                        Param2 = parts[2].Trim()
                    };

                    _evolutionMethodInfos.Add(info);
                    cmbEvoCondMethod.Items.Add(info.MethodName);
                }
            }

            cmbEvoCondMethod.EndUpdate();
        }

        private void cmbEvoCondMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selectedIndex = cmbEvoCondMethod.SelectedIndex;
            if (selectedIndex >= 0 && selectedIndex < _evolutionMethodInfos.Count)
            {
                txtEvoCondParam1Desc.Text = _evolutionMethodInfos[selectedIndex].Param1;
                txtEvoCondParam2Desc.Text = _evolutionMethodInfos[selectedIndex].Param2;
            }
        }

        private void UpdateEvoToIcon()
        {
            Bitmap icon = null;

            if (cmbEvoToPokemon.SelectedIndex >= 0 && cmbEvoToPokemon.SelectedIndex < cmbEvoToPokemon.Items.Count)
            {
                int idx = cmbEvoToPokemon.SelectedIndex;
                icon = GetPokemonIcon(idx, true);
            }

            picEvoToIcon.Image?.Dispose();
            picEvoToIcon.Image = null;
            picEvoToIcon.Image = icon;
        }

        private Bitmap GetPokemonIcon(int idx, bool showBackColor)
        {
            // cache?
            if (idx == _currentPokemonIdx && _currentIconFrame != null)
            {
                return (Bitmap)_currentIconFrame.Clone();
            }

            // normal
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
                return ImageManager.CreateSprite(image, palette, GbaConstants.IconFrameSize, GbaConstants.IconFrameSize, showBackColor);
            }
            catch
            {
                return null;
            }
        }

        private void cmbEvoToPokemon_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_isUpdatingUI) return;
            UpdateEvoToIcon();
        }

        private void EvoInputAssist_CheckedChanged(object sender, EventArgs e)
        {
            cmbEvoInputAssistPokemon.Enabled = rbEvoInputAssistPokemon.Checked;
            cmbEvoInputAssistType.Enabled = rbEvoInputAssistType.Checked;
            cmbEvoInputAssistItem.Enabled = rbEvoInputAssistItem.Checked;
            cmbEvoInputAssistMove.Enabled = rbEvoInputAssistMove.Checked;
        }

        private void LoadEvolutionsToUI(int idx)
        {
            lstEvoSlots.SelectedIndex = _currentEvoSlotIndex;

            var slots = _originalEvoSlots[idx];
            byte[] binary = EvolutionSlotsToBytes(slots);
            _uiStateManager.UpdateBinary(lstEvoSlots, binary);

            DataBindingHelper.BindObjectToControls(this, slots[_currentEvoSlotIndex]);
            UpdateEvoToIcon();
        }

        private byte[] EvolutionSlotsToBytes(PokemonEvolutionEntry[] entries)
        {
            int totalSize = _config.GetInt("PokemonEvolutionSlotLength") * entries.Length;
            byte[] data = new byte[totalSize];
            IoHelper.WriteStructures(data, 0, entries, _tblReader);
            return data;
        }

        private void lstEvoSlots_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_isUpdatingUI) return;
            if (_currentEvoSlotIndex == lstEvoSlots.SelectedIndex) return;

            _isUpdatingUI = true;

            int newIndex = lstEvoSlots.SelectedIndex;
            DataBindingHelper.BindControlsToObject(this, _workingEvoSlots[_currentPokemonIdx][_currentEvoSlotIndex]);
            _currentEvoSlotIndex = newIndex;
            DataBindingHelper.BindObjectToControls(this, _workingEvoSlots[_currentPokemonIdx][_currentEvoSlotIndex]);

            _isUpdatingUI = false;

            UpdateEvoToIcon();
        }

        private void OnEvolutionUIChanged(object sender, EventArgs e)
        {
            if (_isUpdatingUI) return;

            DataBindingHelper.BindControlsToObject(this, _workingEvoSlots[_currentPokemonIdx][_currentEvoSlotIndex]);
            byte[] currentBytes = EvolutionSlotsToBytes(_workingEvoSlots[_currentPokemonIdx]);
            _uiStateManager.UpdateBinary(lstEvoSlots, currentBytes);
        }

        private int GetSelectedEvolutionInputAssistValue()
        {
            if (rbEvoInputAssistPokemon.Checked && cmbEvoInputAssistPokemon.SelectedIndex >= 0)
                return cmbEvoInputAssistPokemon.SelectedIndex;
            if (rbEvoInputAssistType.Checked && cmbEvoInputAssistType.SelectedIndex >= 0)
                return cmbEvoInputAssistType.SelectedIndex;
            if (rbEvoInputAssistItem.Checked && cmbEvoInputAssistItem.SelectedIndex >= 0)
                return cmbEvoInputAssistItem.SelectedIndex;
            if (rbEvoInputAssistMove.Checked && cmbEvoInputAssistMove.SelectedIndex >= 0)
                return cmbEvoInputAssistMove.SelectedIndex;
            return 0;
        }

        private void btnEvoInputAssistParam1_Click(object sender, EventArgs e)
        {
            if (_isUpdatingUI) return;

            int val = GetSelectedEvolutionInputAssistValue();
            nudEvoCondParam1A.Value = val & GbaConstants.Mask8Bits;
            nudEvoCondParam1B.Value = (val >> GbaConstants.BitsPerByte) & GbaConstants.Mask8Bits;
        }

        private void btnEvoInputAssistParam2_Click(object sender, EventArgs e)
        {
            if (_isUpdatingUI) return;

            int val = GetSelectedEvolutionInputAssistValue();
            nudEvoCondParam2A.Value = val & GbaConstants.Mask8Bits;
            nudEvoCondParam2B.Value = (val >> GbaConstants.BitsPerByte) & GbaConstants.Mask8Bits;
        }

        private void LoadLearnsetsToUI(int idx)
        {
            DataBindingHelper.BindObjectToControls(this, _learnsetManager.Working[idx]);
            UpdateLearnsetDisplay();
            LoadLearnFlagData(idx, "TmHmLearnTableAddress", "TmHmCount", clbTmHm, "TmHmData");
            LoadLearnFlagData(idx, "TutorLearnTableAddress", "TutorCount", clbTutor, "TutorData");
        }

        private void UpdateLearnsetDisplay(object sender = null, EventArgs e = null)
        {
            if (_isUpdatingUI && sender != null) return;

            byte[] currentBinary = null;

            if (ControlHelper.TryParseAddress(txtLearnsetAddr.Text, out uint address))
            {
                var res = _reservationManager.GetReservation(txtLearnsetAddr);

                if (res != null)
                {
                    _currentLearnsetList = DecodeLearnsetData(res.Data, 0);
                    currentBinary = res.Data;
                }
                else
                {
                    _currentLearnsetList = DecodeLearnsetData(_romData, address);
                    currentBinary = EncodeLearnsetData(_currentLearnsetList);
                }
            }
            else
            {
                _currentLearnsetList = new List<LearnsetList>();
            }

            _uiStateManager.UpdateBinary(lstLearnset, currentBinary);

            _isUpdatingUI = true;

            lstLearnset.BeginUpdate();
            lstLearnset.Items.Clear();
            foreach (var move in _currentLearnsetList)
            {
                lstLearnset.Items.Add(move.ToString());
            }
            lstLearnset.EndUpdate();

            _isUpdatingUI = false;

            if (lstLearnset.Items.Count > 0)
            {
                lstLearnset.SelectedIndex = 0;
            }
            else
            {
                _isUpdatingUI = true;
                nudLearnsetLevel.Value = 0;
                cmbLearnsetMove.SelectedIndex = -1;
                _isUpdatingUI = false;
            }
        }

        private List<LearnsetList> DecodeLearnsetData(byte[] data, uint offset = 0)
        {
            var moves = new List<LearnsetList>();
            if (data == null || data.Length == 0) return moves;

            int pos = (int)offset;
            while (moves.Count < 256)
            {
                if (_config.GetBool("IsAppliedCFRU") && _config.GetBool("EnableLearnsetExpansion"))
                {
                    ushort moveId = BitConverter.ToUInt16(data, pos);
                    byte level = data[pos + 2];
                    pos += GbaConstants.LearnsetEntryLength3Byte;

                    if (moveId == GbaConstants.LearnsetTerminator2Byte &&
                        level == GbaConstants.LearnsetTerminator3ByteLevel) break;

                    moves.Add(new LearnsetList { Level = level, MoveIdx = moveId });
                }
                else
                {
                    ushort raw = BitConverter.ToUInt16(data, pos);
                    pos += GbaConstants.LearnsetEntryLength2Byte;

                    if (raw == GbaConstants.LearnsetTerminator2Byte) break;

                    int level = (raw >> GbaConstants.BitsPerByte) / 2;
                    int moveId = (raw & GbaConstants.Mask8Bits) |
                                 (((raw >> GbaConstants.BitsPerByte) & 1) << GbaConstants.BitsPerByte);
                    moves.Add(new LearnsetList { Level = level, MoveIdx = moveId });
                }
            }

            foreach (var m in moves)
            {
                if (m.MoveIdx >=0 && m.MoveIdx < _moveNameManager.Original.Count)
                {
                    m.MoveName = _moveNameManager.Original[m.MoveIdx]._MoveName;
                }
                else
                {
                    m.MoveName = $"(Invalid Move {m.MoveIdx})";
                }
            }

            return moves;
        }

        private byte[] EncodeLearnsetData(List<LearnsetList> moves, bool align = true)
        {
            var data = new List<byte>();

            foreach (var move in moves)
            {
                if (_config.GetBool("IsAppliedCFRU") && _config.GetBool("EnableLearnsetExpansion"))
                {
                    data.AddRange(BitConverter.GetBytes((ushort)move.MoveIdx));
                    data.Add((byte)move.Level);
                }
                else
                {
                    int encodedLevel = (move.Level * 2) | ((move.MoveIdx >> GbaConstants.BitsPerByte) & 1);
                    ushort raw = (ushort)((encodedLevel << GbaConstants.BitsPerByte) | (move.MoveIdx & GbaConstants.Mask8Bits));
                    data.AddRange(BitConverter.GetBytes(raw));
                }
            }

            // terminate
            if (_config.GetBool("IsAppliedCFRU") && _config.GetBool("EnableLearnsetExpansion"))
            {
                data.AddRange(BitConverter.GetBytes((ushort)GbaConstants.LearnsetTerminator2Byte));
                data.Add(GbaConstants.LearnsetTerminator3ByteLevel);
            }
            else
            {
                data.AddRange(BitConverter.GetBytes((ushort)GbaConstants.LearnsetTerminator2Byte));
            }

            // alignment
            if (align)
            {
                while (data.Count % GbaConstants.PtrSize != 0)
                {
                    data.Add((byte)GbaConstants.PaddingByte);
                }
            }

            return data.ToArray();
        }

        private void OnLevelMoveUIChanged(object sender, EventArgs e)
        {
            if (_isUpdatingUI || lstLearnset.SelectedIndex < 0) return;

            var move = _currentLearnsetList[lstLearnset.SelectedIndex];
            move.Level = (int)nudLearnsetLevel.Value;
            move.MoveIdx = cmbLearnsetMove.SelectedIndex;

            if (move.MoveIdx >= 0 && move.MoveIdx < _moveNameManager.Original.Count)
            {
                move.MoveName = _moveNameManager.Original[move.MoveIdx]._MoveName;
            }
            else
            {
                move.MoveName = $"(Invalid Move {move.MoveIdx})";
            }

            _isUpdatingUI = true;
            lstLearnset.Items[lstLearnset.SelectedIndex] = move.ToString();
            _isUpdatingUI = false;

            byte[] newBinary = EncodeLearnsetData(_currentLearnsetList);
            _uiStateManager.UpdateBinary(lstLearnset, newBinary);

            var reservation = _reservationManager.GetReservation(txtLearnsetAddr);
            if (reservation != null)
            {
                _reservationManager.SetReservation(txtLearnsetAddr, reservation.Address, newBinary);
            }
        }

        private void lstLearnset_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_isUpdatingUI) return;

            int idx = lstLearnset.SelectedIndex;
            if (idx < 0 || idx >= _currentLearnsetList.Count) return;

            _isUpdatingUI = true;

            var move = _currentLearnsetList[idx];

            if (move.Level >= nudLearnsetLevel.Minimum && move.Level <= nudLearnsetLevel.Maximum)
            {
                nudLearnsetLevel.Value = move.Level;
            }

            if (move.MoveIdx >= 0 && move.MoveIdx < cmbLearnsetMove.Items.Count)
            {
                cmbLearnsetMove.SelectedIndex = move.MoveIdx;
            }
            else
            {
                cmbLearnsetMove.SelectedIndex = -1;
            }

            _isUpdatingUI = false;
        }

        private void btnCreateNewLearnset_Click(object sender, EventArgs e)
        {
            using (var popup = new QuickInputPopup())
            {
                popup.Setup(txtLearnsetAddr.Text, nudMin: 1, nudMax: 256, defaultNudValue: 1);

                if (popup.ShowDialog(this) == DialogResult.OK)
                {
                    string targetAddressStr = popup.ResultAddress;
                    int entryCount = popup.ResultEntryCount;

                    if (targetAddressStr != "null" && ControlHelper.TryParseAddress(targetAddressStr, out uint targetAddress))
                    {
                        var newMoves = new List<LearnsetList>();
                        string moveName = string.Empty;

                        if (cmbLearnsetMove.Items.Count > 1)
                        {
                            moveName = cmbLearnsetMove.Items[1].ToString();
                        }

                        for (int i = 0; i < entryCount; i++)
                        {
                            newMoves.Add(new LearnsetList
                            {
                                Level = 0,
                                MoveIdx = 0,
                                MoveName = moveName
                            });
                        }

                        byte[] newDataBytes = EncodeLearnsetData(newMoves);
                        _reservationManager.SetReservation(txtLearnsetAddr, targetAddress, newDataBytes);

                        _isUpdatingUI = true;
                        _currentLearnsetList = newMoves;
                        lstLearnset.Items.Clear();
                        foreach (var move in _currentLearnsetList)
                        {
                            lstLearnset.Items.Add(move.ToString());
                        }
                        _isUpdatingUI = false;

                        txtLearnsetAddr.Text = targetAddressStr;
                        UpdateLearnsetDisplay();
                    }
                }
            }
        }

        private void LoadLearnFlagData(int pokemonIndex, string addressKey, string countKey, CheckedListBox clb, string uiStateKey)
        {
            uint baseAddress = (uint)_config.GetAddr(addressKey);
            int count = _config.GetInt(countKey);
            int dataLength = (count + (GbaConstants.BitsPerByte - 1)) / GbaConstants.BitsPerByte;

            uint address = baseAddress + (uint)(pokemonIndex * dataLength);
            byte[] data = new byte[dataLength];
            Array.Copy(_romData, address, data, 0, dataLength);

            for (int i = 0; i < count; i++)
            {
                int byteIndex = i / GbaConstants.BitsPerByte;
                int bitIndex = i % GbaConstants.BitsPerByte;
                bool isLearned = (data[byteIndex] & (1 << bitIndex)) != 0;
                clb.SetItemChecked(i, isLearned);
            }
            
            _uiStateManager.AddBinaries((uiStateKey, data));
            _uiStateManager.UpdateBinary(uiStateKey, data);
        }

        private byte[] GetCurrentLearnFlagData(CheckedListBox clb, int count)
        {
            int dataLength = (count + (GbaConstants.BitsPerByte - 1)) / GbaConstants.BitsPerByte;
            byte[] data = new byte[dataLength];

            for (int i = 0; i < count; i++)
            {
                if (clb.GetItemChecked(i))
                {
                    int byteIndex = i / GbaConstants.BitsPerByte;
                    int bitIndex = i % GbaConstants.BitsPerByte;
                    data[byteIndex] |= (byte)(1 << bitIndex);
                }
            }
            return data;
        }

        private void HandleLearnFlagItemCheck(CheckedListBox clb, string countKey, string uiStateKey)
        {
            if (_isUpdatingUI) return;

            this.BeginInvoke((MethodInvoker)delegate
            {
                int count = _config.GetInt(countKey);
                byte[] data = GetCurrentLearnFlagData(clb, count);
                _uiStateManager?.UpdateBinary(uiStateKey, data);
            });
        }

        private void LoadPokedexSizeCompImages()
        {
            _dexSizeCompBackgroundImage = (Bitmap)Image.FromFile("img/PokedexSizeComparisonBackGround.png");
        }

        private void LoadPokedexToUI(int idx)
        {
            _currentdexOrder = (idx == 0) 
                ? 0 
                : _orderManager.Working[idx - 1]._OrderIdx;

            nudPokedexOrder.Value = _currentdexOrder;

            if (_currentdexOrder >= 0 && _currentdexOrder < _dexManager.Working.Count)
            {
                ControlHelper.SetControlsEnabled(tabPagePokedex, true);
                DataBindingHelper.BindObjectToControls(this, _dexManager.Working[_currentdexOrder]);

                // category
                txtDexCategory.Text = _dexManager.Working[_currentdexOrder]._DexCategory;
                PokedexCategoryTrimming();

                UpdatePokedexInfoUnitLabel(nudDexHeight, lblDexHeight);
                UpdatePokedexInfoUnitLabel(nudDexWeight, lblDexWeight);
                DisplayPokedexDesc();
            }
            else
            {
                ControlHelper.SetControlsEnabled(tabPagePokedex, false);
                ControlHelper.ResetControls(tabPagePokedex, new[] { "nudDexSizeCompTrainerSpriteIdx" });
                txtDexDescString.Text = string.Empty;
                _uiStateManager.UpdateBinary(txtDexDescString, null);
            }

            UpdateSizeCompDisplay();
        }

        private void PokedexCategoryTrimming()
        {
            int maxAllowedBytes = _config.GetInt("PokedexCategoryMaxLength");
            string currentText = txtDexCategory.Text;
            byte[] currentBytes = _tblReader.StringToBytes(currentText, false);

            if (currentBytes.Length > maxAllowedBytes)
            {
                while (currentText.Length > 0)
                {
                    currentBytes = _tblReader.StringToBytes(currentText, false);
                    if (currentBytes.Length <= maxAllowedBytes) break;

                    currentText = currentText.Substring(0, currentText.Length - 1);
                }

                int savedSelectionStart = txtDexCategory.SelectionStart;
                txtDexCategory.Text = currentText;
                txtDexCategory.SelectionStart = Math.Min(savedSelectionStart, currentText.Length);
            }

            if (currentBytes.Length < maxAllowedBytes)
            {
                byte[] paddedBytes = new byte[maxAllowedBytes];
                Array.Copy(currentBytes, paddedBytes, currentBytes.Length);
                currentBytes = paddedBytes;
            }

            string validName = _tblReader.BytesToString(currentBytes, 0, currentBytes.Length);

            if (_currentdexOrder >= 0 && _currentdexOrder < _dexManager.Working.Count)
            {
                _dexManager.Working[_currentdexOrder]._DexCategory = validName;
                txtDexCategory.Text = validName;
            }
        }

        private void txtDexCategory_TextChanged(object sender, EventArgs e)
        {
            if (_isUpdatingUI) return;
            PokedexCategoryTrimming();
        }

        private void UpdatePokedexInfoUnitLabel(NumericUpDown nud, Label lbl)
        {
            lbl.Text = (nud.Value / 10m).ToString("0.0");
        }

        private void PokedexInfoNudUnit_ValueChanged(object sender, EventArgs e)
        {
            if (_isUpdatingUI) return;
            var nud = (NumericUpDown)sender;

            if (nud == nudDexHeight)
            {
                UpdatePokedexInfoUnitLabel(nudDexHeight, lblDexHeight);
            }

            if (nud == nudDexWeight)
            {
                UpdatePokedexInfoUnitLabel(nudDexWeight, lblDexWeight);
            }
        }

        private void DisplayPokedexDesc()
        {
            _isUpdatingUI = true;

            if (ControlHelper.TryParseAddress(txtDexDescAddr.Text, out uint address))
            {
                List<byte> descriptionBytes = new List<byte>();
                uint i = 0;

                while (address + i < _romData.Length)
                {
                    byte b = _romData[(int)(address + i)];
                    descriptionBytes.Add(b);
                    if (b == 0xFF)
                    {
                        break;
                    }

                    i++;
                }

                byte[] byteArr = descriptionBytes.ToArray();
                _currentDexDescData = byteArr;
                txtDexDescString.Text = _tblReader.BytesToString(byteArr, 0, 256);
                _uiStateManager.UpdateBinary(txtDexDescString, byteArr);
            }
            else
            {
                _currentDexDescData = null;
                txtDexDescString.Text = string.Empty;
                _uiStateManager.UpdateBinary(txtDexDescString, null);
            }

            _isUpdatingUI = false;
        }

        private void txtDexDescAddr_TextChanged(object sender, EventArgs e)
        {
            if (_isUpdatingUI) return;
            DisplayPokedexDesc();
        }

        private void txtDexDescString_TextChanged(object sender, EventArgs e)
        {
            if (_isUpdatingUI) return;

            byte[] bytes = _tblReader.StringToBytes(txtDexDescString.Text, true);
            _currentDexDescData = bytes;
            _uiStateManager.UpdateBinary(txtDexDescString, bytes);
        }

        private void SizeCompParam_ValueChanged(object sender, EventArgs e)
        {
            UpdateSizeCompDisplay(sender, e);
        }

        private void UpdateSizeCompDisplay(object sender = null, EventArgs e = null)
        {
            if (_isUpdatingUI && sender != null) return;

            Bitmap baseCanvas = new Bitmap(GbaConstants.PokedexSizeComparisonBaseWidth, GbaConstants.PokedexSizeComparisonBaseHeight);
            using (Graphics g = Graphics.FromImage(baseCanvas))
            {
                g.SmoothingMode = SmoothingMode.None;
                g.PixelOffsetMode = PixelOffsetMode.Half;
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                g.DrawImage(_dexSizeCompBackgroundImage, 0, 0,
                            GbaConstants.PokedexSizeComparisonBaseWidth, GbaConstants.PokedexSizeComparisonBaseHeight);

                // pokemon
                if (_battleEnemyImage != null)
                {
                    try
                    {
                        using (Bitmap pokemonSprite = (Bitmap)_battleEnemyImage.Clone())
                        {
                            ColorPalette pal = pokemonSprite.Palette;
                            for (int i = 1; i < pal.Entries.Length; i++)
                            {
                                pal.Entries[i] = Color.Black;
                            }
                            pokemonSprite.Palette = pal;

                            float param1 = (float)nudDexSizeCompParam1.Value;
                            if (param1 > 0)
                            {
                                float scaleA = GbaConstants.PokedexSizeComparisonScaleBase / param1;
                                int newWidthA = (int)(GbaConstants.SpriteSize * scaleA);
                                int newHeightA = (int)(GbaConstants.SpriteSize * scaleA);
                                int offsetYA = (int)nudDexSizeCompParam2.Value;

                                Rectangle destRectA = new Rectangle(
                                    GbaConstants.PokedexSizeComparisonPokemonBaseX + (GbaConstants.SpriteSize - newWidthA) / 2,
                                    GbaConstants.PokedexSizeComparisonPokemonBaseY + (GbaConstants.SpriteSize - newHeightA) / 2 + offsetYA,
                                    newWidthA, newHeightA);

                                g.DrawImage(pokemonSprite, destRectA);
                            }
                        }
                    }
                    catch 
                    { 
                        //
                    }
                }

                // trainer
                try
                {
                    int trainerId = (int)nudDexSizeCompTrainerSpriteIdx.Value;
                    using (Bitmap trainerSprite = GetTrainerSprite(trainerId, false))
                    {
                        if (trainerSprite != null)
                        {
                            ColorPalette pal = trainerSprite.Palette;
                            for (int i = 1; i < pal.Entries.Length; i++)
                            {
                                pal.Entries[i] = Color.Black;
                            }
                            trainerSprite.Palette = pal;

                            float param3 = (float)nudDexSizeCompParam3.Value;
                            if (param3 > 0)
                            {
                                float scaleB = GbaConstants.PokedexSizeComparisonScaleBase / param3;
                                int newWidthB = (int)(GbaConstants.SpriteSize * scaleB);
                                int newHeightB = (int)(GbaConstants.SpriteSize * scaleB);
                                int offsetYB = (int)nudDexSizeCompParam4.Value;

                                Rectangle destRectB = new Rectangle(
                                    GbaConstants.PokedexSizeComparisonTrainerBaseX + (GbaConstants.SpriteSize - newWidthB) / 2,
                                    GbaConstants.PokedexSizeComparisonTrainerBaseY + (GbaConstants.SpriteSize - newHeightB) / 2 + offsetYB,
                                    newWidthB, newHeightB);

                                g.DrawImage(trainerSprite, destRectB);
                            }
                        }
                    }
                }
                catch
                { 
                    //
                }
            }

            Bitmap scaledCanvas = ImageManager.ScalePixelArt(baseCanvas, GbaConstants.DefaultScale);
            picDexSizeCompPreview.Image?.Dispose();
            picDexSizeCompPreview.Image = null;
            picDexSizeCompPreview.Image = scaledCanvas;
            baseCanvas.Dispose();
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





        private void ResetControls()
        {
            txtSpriteImportAddr.Text = String.Empty;
            cmbSpriteExport.SelectedIndex = 0;
            txtIconImportAddr.Text = String.Empty;
            txtFootprintImportAddr.Text = String.Empty;
        }

        private void DiscardData(int idx)
        {
            _pokemonNameManager.Discard(idx);
            string originalName = _pokemonNameManager.Original[idx]._PokemonName;
            cmbPokemonName.Items[idx] = originalName;
            cmbEvoToPokemon.Items[idx] = originalName;
            cmbEvoInputAssistPokemon.Items[idx] = originalName;

            _workingEvoSlots[idx] = _originalEvoSlots[idx].Select(e => CloneHelper.Clone(e)).ToArray();

            _dexManager.Discard(idx);
        }

        private void SaveCurrentAllData(int idx)
        {
            SaveCurrentPokemonName(idx);
            SaveCurrentSprites(idx);
            SaveCurrentIcon(idx);
            SaveCurrentFootprint(idx);
            SaveCurrentCoordBattle(idx);
            SaveCurrentCoordItemUse(idx);
            SaveCurrentStats(idx);
            SaveCurrentEvolutions(idx);
            SaveCurrentLearnsets(idx);
            SaveCurrentPokedex();
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

        private void SaveCurrentStats(int idx)
        {
            // evs
            byte[] evs = new byte[6];
            evs[0] = (byte)nudStatsEvHp.Value;
            evs[1] = (byte)nudStatsEvAtk.Value;
            evs[2] = (byte)nudStatsEvDef.Value;
            evs[3] = (byte)nudStatsEvSpAtk.Value;
            evs[4] = (byte)nudStatsEvSpDef.Value;
            evs[5] = (byte)nudStatsEvSpeed.Value;

            if (_config.GetBool("IsAppliedCFRU") && _config.GetBool("EnableStatsExpansion"))
            {
                _statsExpansionManager.Working[idx]._StatsEvs = EncodeEv(evs);
                DataBindingHelper.BindControlsToObject(this, _statsExpansionManager.Working[idx]);
                _statsExpansionManager.Save(idx);
            }
            else
            {
                _statsNormalManager.Working[idx]._StatsEvs = EncodeEv(evs);
                DataBindingHelper.BindControlsToObject(this, _statsNormalManager.Working[idx]);
                _statsNormalManager.Save(idx);
            }
        }

        private void SaveCurrentEvolutions(int idx)
        {
            int entrySize = _config.GetInt("PokemonEvolutionSlotLength");
            int slotCount = _config.GetInt("PokemonEvolutionSlotCount");
            uint? baseAddress = _config.GetAddr("PokemonEvolutionTableAddress");
            uint address = (uint)(baseAddress + idx * slotCount * entrySize);
            IoHelper.WriteStructures(_romData, (int)address, _workingEvoSlots[idx], _tblReader);
            _originalEvoSlots[idx] = _workingEvoSlots[idx].Select(e => CloneHelper.Clone(e)).ToArray();
        }

        private void SaveCurrentLearnsets(int idx)
        {
            var res = _reservationManager.GetReservation(txtLearnsetAddr);
            if (res != null && res.Data != null)
            {
                Array.Copy(res.Data, 0, _romData, (int)res.Address, res.Data.Length);
                _reservationManager.ClearReservation(txtLearnsetAddr);
            }
            else if (_uiStateManager.HasBinaryChanges(lstLearnset) && _currentLearnsetList != null)
            {
                if (ControlHelper.TryParseAddress(txtLearnsetAddr.Text, out uint address))
                {
                    byte[] dataToSave = EncodeLearnsetData(_currentLearnsetList, false);
                    Array.Copy(dataToSave, 0, _romData, (int)address, dataToSave.Length);
                }
            }

            DataBindingHelper.BindControlsToObject(this, _learnsetManager.Working[idx]);
            _learnsetManager.Save(idx);

            SaveLearnFlagData(idx, "TmHmLearnTableAddress", "TmHmCount", clbTmHm, "TmHmData");
            SaveLearnFlagData(idx, "TutorLearnTableAddress", "TutorCount", clbTutor, "TutorData");
        }

        private void SaveLearnFlagData(int pokemonIndex, string addressKey, string countKey, CheckedListBox clb, string uiStateKey)
        {
            if (!_uiStateManager.HasBinaryChanges(uiStateKey)) return;

            int count = _config.GetInt(countKey);
            byte[] data = GetCurrentLearnFlagData(clb, count); 

            uint baseAddress = (uint)_config.GetAddr(addressKey);
            uint address = baseAddress + (uint)(pokemonIndex * data.Length);
            Array.Copy(data, 0, _romData, (int)address, data.Length);
        }

        private void SaveCurrentPokedex()
        {
            if (_currentdexOrder >= 0 && _currentdexOrder < _dexManager.Working.Count)
            {
                DataBindingHelper.BindControlsToObject(this, _dexManager.Working[_currentdexOrder]);
                _dexManager.Save(_currentdexOrder, false, GbaConstants.PaddingByte, GbaConstants.PaddingByte);

                // desc
                if (!ControlHelper.TryParseAddress(txtDexDescAddr.Text, out uint address)) return;
                if (_uiStateManager.HasBinaryChanges(txtDexDescString) && _currentDexDescData != null)
                {
                    _tblReader.WriteToRom(_romData, address, _currentDexDescData);
                }
            }
        }
    }
}
