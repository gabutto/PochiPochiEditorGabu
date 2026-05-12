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
    public partial class PokedexSearchEditor : Form
    {
        protected byte[] _romData;
        protected IniFileReader _config;
        protected TblFileReader _tblReader;
        protected ReservationManager _reservationManager;

        private UIStateManager _uiStateManager;

        private EntryManager<PokedexSearchSortEntry> _sortAiueoManager;
        private EntryManager<PokedexSearchSortEntry> _sortTypeManager;
        private EntryManager<PokedexSearchSortEntry> _sortWeightManager;
        private EntryManager<PokedexSearchSortEntry> _sortHeightManager;

        private EntryManager<PokemonNameEntry> _pokemonNameManager;
        private EntryManager<PokedexOrderEntry> _orderManager;
        private EntryManager<PokemonIconImageEntry> _iconImgManager;
        private EntryManager<PokemonIconPaletteIndexEntry> _iconPalIdxManager;
        private EntryManager<PokemonIconPaletteAddressEntry> _iconPalAddrManager;

        private bool _isUpdatingUI = false;
        private bool _isCfruSpeciesIndexed = false;
        private int _currentSortAiueoIdx = 0;
        private int _currentSortTypeIdx = 0;
        private int _currentSortWeightIdx = 0;
        private int _currentSortHeightIdx = 0;

        private Dictionary<int, int> _orderToSpeciesMap = new Dictionary<int, int>();

        private class SortListBoxItem
        {
            public int SpeciesIndex { get; set; }
            public string DisplayName { get; set; }
            public override string ToString() => DisplayName;
        }

        public PokedexSearchEditor(
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

            LoadSortAiueoToUI(_currentSortAiueoIdx);
            LoadSortTypeToUI(_currentSortTypeIdx);
            LoadSortWeightToUI(_currentSortWeightIdx);
            LoadSortHeightToUI(_currentSortHeightIdx);
        }

        private void InitializeManagers()
        {
            // sort
            _sortAiueoManager = EntryManager<PokedexSearchSortEntry>.Create(
                _romData, _tblReader, _config, "PokedexSearchSortAiueoTableAddress", "PokedexSearchSortAiueoCount");
            _sortTypeManager = EntryManager<PokedexSearchSortEntry>.Create(
                _romData, _tblReader, _config, "PokedexSearchSortTypeTableAddress", "PokedexSearchSortTypeCount");
            _sortWeightManager = EntryManager<PokedexSearchSortEntry>.Create(
                _romData, _tblReader, _config, "PokedexSearchSortWeightTableAddress", "PokedexSearchSortWeightCount");
            _sortHeightManager = EntryManager<PokedexSearchSortEntry>.Create(
                _romData, _tblReader, _config, "PokedexSearchSortHeightTableAddress", "PokedexSearchSortHeightCount");

            // name
            _pokemonNameManager = EntryManager<PokemonNameEntry>.Create(
                _romData, _tblReader, _config, "PokemonNameTableAddress", "PokemonNameCount");

            // order
            _orderManager = EntryManager<PokedexOrderEntry>.Create(
                _romData, _tblReader, _config, "PokedexOrderTableAddress", "PokedexOrderCount");

            // icon
            _iconImgManager = EntryManager<PokemonIconImageEntry>.Create(
                _romData, _tblReader, _config, "PokemonIconImageTableAddress", "PokemonIconCount");
            _iconPalIdxManager = EntryManager<PokemonIconPaletteIndexEntry>.Create(
                _romData, _tblReader, _config, "PokemonIconPaletteIndexTableAddress", "PokemonIconCount");
            _iconPalAddrManager = EntryManager<PokemonIconPaletteAddressEntry>.Create(
                _romData, _tblReader, _config, "PokemonIconPaletteAddressTableAddress", "PokemonIconPaletteAddressCount");

            // order to species map
            for (int speciesIdx = 0; speciesIdx < _config.GetInt("PokemonNameCount"); speciesIdx++)
            {
                int orderIdx = (speciesIdx == 0)
                    ? 0
                    : _orderManager.Working[speciesIdx - 1]._OrderIdx;

                if (!_orderToSpeciesMap.ContainsKey(orderIdx))
                {
                    _orderToSpeciesMap.Add(orderIdx, speciesIdx);
                }
            }

            // cfru
            _isCfruSpeciesIndexed = _config.GetBool("IsAppliedCFRU") && _config.GetBool("EnableSpeciesIndexed");
        }

        private void InitializeEventHandlers()
        {
            btnSave.Click += btnSave_Click;
            this.FormClosing += PokedexSearchEditor_FormClosing;

            lstSortAiueo.SelectedIndexChanged +=
                (s, e) => LoadSortAiueoToUI(lstSortAiueo.SelectedIndex);
            lstSortType.SelectedIndexChanged += 
                (s, e) => LoadSortTypeToUI(lstSortType.SelectedIndex);
            lstSortWeight.SelectedIndexChanged += 
                (s, e) => LoadSortWeightToUI(lstSortWeight.SelectedIndex);
            lstSortHeight.SelectedIndexChanged += 
                (s, e) => LoadSortHeightToUI(lstSortHeight.SelectedIndex);

            cmbPokemonName1.SelectedIndexChanged += 
                (s, e) => OnComboBoxSelectedIndexChanged(
                    cmbPokemonName1, 
                    lstSortAiueo,
                    _sortAiueoManager,
                    true, 
                    "SortAiueo",
                    nudPokemonIdx1, 
                    txtPokemonIdx1Hex, 
                    picPokemonIcon1);
            cmbPokemonName2.SelectedIndexChanged += 
                (s, e) => OnComboBoxSelectedIndexChanged(
                    cmbPokemonName2, 
                    lstSortType, 
                    _sortTypeManager,
                    false, "" +
                    "SortType", 
                    nudPokemonIdx2,
                    txtPokemonIdx2Hex,
                    picPokemonIcon2);
            cmbPokemonName3.SelectedIndexChanged += 
                (s, e) => OnComboBoxSelectedIndexChanged(
                    cmbPokemonName3,
                    lstSortWeight,
                    _sortWeightManager,
                    true, 
                    "SortWeight",
                    nudPokemonIdx3, 
                    txtPokemonIdx3Hex, 
                    picPokemonIcon3);
            cmbPokemonName4.SelectedIndexChanged += 
                (s, e) => OnComboBoxSelectedIndexChanged(
                    cmbPokemonName4, 
                    lstSortHeight, 
                    _sortHeightManager, 
                    true,
                    "SortHeight",
                    nudPokemonIdx4, 
                    txtPokemonIdx4Hex, 
                    picPokemonIcon4);
        }

        private void InitializeControls()
        {
            // pokemon name for cmb
            var pokemonNames = _pokemonNameManager.Original
                             .Select(entry => entry._PokemonName)
                             .ToArray();
            cmbPokemonName1.Items.AddRange(pokemonNames);
            cmbPokemonName2.Items.AddRange(pokemonNames);
            cmbPokemonName3.Items.AddRange(pokemonNames);
            cmbPokemonName4.Items.AddRange(pokemonNames);

            ControlHelper.AttachExternalBorder(
                picPokemonIcon1,
                picPokemonIcon2,
                picPokemonIcon3,
                picPokemonIcon4);

            // lstSort
            PopulateSortListBox(lstSortAiueo, _sortAiueoManager, true);
            PopulateSortListBox(lstSortType, _sortTypeManager, false);
            PopulateSortListBox(lstSortWeight, _sortWeightManager, true);
            PopulateSortListBox(lstSortHeight, _sortHeightManager, true);
        }

        private void InitializeUIStates()
        {
            btnSave.Enabled = false;
            _uiStateManager = new UIStateManager(hasChanges => btnSave.Enabled = hasChanges);

            _uiStateManager.AddBinaries(
                ("SortAiueo", SerializeSortManager(_sortAiueoManager)),
                ("SortType", SerializeSortManager(_sortTypeManager)),
                ("SortWeight", SerializeSortManager(_sortWeightManager)),
                ("SortHeight", SerializeSortManager(_sortHeightManager))
            );
        }

        private void PopulateSortListBox(ListBox lst, EntryManager<PokedexSearchSortEntry> manager, bool checkCfru)
        {
            lst.BeginUpdate();
            lst.Items.Clear();

            for (int i = 0; i < manager.Original.Count; i++)
            {
                ushort idxVal = manager.Original[i]._Idx;
                int speciesIdx = 0;

                if (checkCfru && !_isCfruSpeciesIndexed)
                {
                    if (_orderToSpeciesMap.TryGetValue(idxVal, out int mappedVal))
                    {
                        speciesIdx = mappedVal;
                    }
                }
                else
                {
                    speciesIdx = idxVal;
                }

                int orderIdx = (speciesIdx == 0)
                    ? 0
                    : _orderManager.Working[speciesIdx - 1]._OrderIdx;
                string pokemonName = _pokemonNameManager.Original[speciesIdx]._PokemonName;

                lst.Items.Add(new SortListBoxItem
                {
                    SpeciesIndex = speciesIdx,
                    DisplayName = $"No.{orderIdx:D4} {pokemonName}"
                });
            }

            lst.EndUpdate();

            if (lst.Items.Count > 0)
            {
                lst.SelectedIndex = 0;
            }
        }

        private void LoadSortToUI(
            int idx, 
            ListBox lst,
            ComboBox cmb,
            NumericUpDown nud, 
            TextBox txtHex, 
            PictureBox picIcon, 
            ref int currentIdx)
        {
            if (_isUpdatingUI || idx < 0 || idx >= lst.Items.Count) return;

            currentIdx = idx;
            _isUpdatingUI = true;

            if (lst.Items[idx] is SortListBoxItem item)
            {
                cmb.SelectedIndex = item.SpeciesIndex;
                nud.Value = item.SpeciesIndex;
                txtHex.Text = item.SpeciesIndex.ToString("X4");

                picIcon.Image?.Dispose();
                picIcon.Image = GetPokemonIcon(item.SpeciesIndex, true);
            }

            _isUpdatingUI = false;
        }

        private void LoadSortAiueoToUI(int idx) => 
            LoadSortToUI(
                idx, 
                lstSortAiueo,
                cmbPokemonName1, 
                nudPokemonIdx1, 
                txtPokemonIdx1Hex,
                picPokemonIcon1,
                ref _currentSortAiueoIdx);
        private void LoadSortTypeToUI(int idx) => 
            LoadSortToUI(
                idx, 
                lstSortType, 
                cmbPokemonName2,
                nudPokemonIdx2,
                txtPokemonIdx2Hex,
                picPokemonIcon2, 
                ref _currentSortTypeIdx);
        private void LoadSortWeightToUI(int idx) => 
            LoadSortToUI(
                idx, 
                lstSortWeight,
                cmbPokemonName3,
                nudPokemonIdx3, 
                txtPokemonIdx3Hex, 
                picPokemonIcon3, 
                ref _currentSortWeightIdx);
        private void LoadSortHeightToUI(int idx) => 
            LoadSortToUI(
                idx, 
                lstSortHeight,
                cmbPokemonName4, 
                nudPokemonIdx4, 
                txtPokemonIdx4Hex, 
                picPokemonIcon4,
                ref _currentSortHeightIdx);

        private void OnComboBoxSelectedIndexChanged(
            ComboBox cmb,
            ListBox lst, 
            EntryManager<PokedexSearchSortEntry> manager, 
            bool checkCfru,
            string binaryKey, 
            NumericUpDown nud,
            TextBox txtHex, 
            PictureBox picIcon)
        {
            if (_isUpdatingUI || lst.SelectedIndex < 0 || cmb.SelectedIndex < 0) return;

            int speciesIdx = cmb.SelectedIndex;
            ushort newEntryVal;

            if (checkCfru && !_isCfruSpeciesIndexed)
            {
                newEntryVal = (ushort)((speciesIdx == 0) 
                    ? 0 
                    : _orderManager.Original[speciesIdx - 1]._OrderIdx);
            }
            else
            {
                newEntryVal = (ushort)speciesIdx;
            }

            int listIdx = lst.SelectedIndex;
            manager.Working[listIdx]._Idx = newEntryVal;

            int orderIdx = (speciesIdx == 0) 
                ? 0 
                : _orderManager.Original[speciesIdx - 1]._OrderIdx;
            string pokemonName = _pokemonNameManager.Original[speciesIdx]._PokemonName;

            if (lst.Items[listIdx] is SortListBoxItem item)
            {
                item.SpeciesIndex = speciesIdx;
                item.DisplayName = $"No.{orderIdx:D4} {pokemonName}";

                _isUpdatingUI = true;

                lst.Items[listIdx] = item;
                lst.SelectedIndex = listIdx;
                nud.Value = speciesIdx;
                txtHex.Text = speciesIdx.ToString("X4");
                picIcon.Image?.Dispose();
                picIcon.Image = GetPokemonIcon(speciesIdx, true);

                _isUpdatingUI = false;
            }

            _uiStateManager.UpdateBinary(binaryKey, SerializeSortManager(manager));
        }

        private byte[] SerializeSortManager(EntryManager<PokedexSearchSortEntry> manager)
        {
            byte[] result = new byte[manager.Working.Count * 2];
            for (int i = 0; i < manager.Working.Count; i++)
            {
                byte[] bytes = BitConverter.GetBytes(manager.Working[i]._Idx);
                result[i * 2] = bytes[0];
                result[i * 2 + 1] = bytes[1];
            }
            return result;
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

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveCurrentAllSort();
            _uiStateManager.UpdateInitialValues();
        }

        private void PokedexSearchEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (btnSave.Enabled)
            {
                ControlHelper.HandleUnsavedChanges(
                    () =>
                    {
                        SaveCurrentAllSort();
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

        private void SaveCurrentAllSort()
        {
            SaveSortManagerIfChanged("SortAiueo", _sortAiueoManager);
            SaveSortManagerIfChanged("SortType", _sortTypeManager);
            SaveSortManagerIfChanged("SortWeight", _sortWeightManager);
            SaveSortManagerIfChanged("SortHeight", _sortHeightManager);
        }

        private void SaveSortManagerIfChanged(string binaryKey, EntryManager<PokedexSearchSortEntry> manager)
        {
            if (_uiStateManager.HasBinaryChanges(binaryKey))
            {
                for (int i = 0; i < manager.Working.Count; i++)
                {
                    if (manager.Original[i]._Idx != manager.Working[i]._Idx)
                    {
                        manager.Save(i);
                    }
                }
            }
        }
    }
}
