using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

using PochiPochiEditorGabu.Constants;
using PochiPochiEditorGabu.FileReaders;
using PochiPochiEditorGabu.Helpers;
using PochiPochiEditorGabu.Managers;

namespace PochiPochiEditorGabu._Pokemon
{
    public partial class PokedexHabitatEditor : Form
    {
        protected byte[] _romData;
        protected IniFileReader _config;
        protected TblFileReader _tblReader;
        protected ReservationManager _reservationManager;

        private UIStateManager _uiStateManager;

        private EntryManager<PokedexHabitatAreaEntry> _areaManager;
        private List<PokedexHabitatPageEntry> _currentAreaPages = null;
        private List<ushort[]> _currentAreaPokemonData = null;

        private EntryManager<PokemonNameEntry> _pokemonNameManager;
        private EntryManager<PokemonSpriteFrontImageEntry> _spriteFrontImgManager;
        private EntryManager<PokemonSpriteNormalPaletteEntry> _spriteNormalPalManager;

        private bool _isUpdatingUI = false;
        private int _currentAreaIdx = 0;
        private int _currentPageIdx = 0;

        private PictureBox[] _picPokemons;
        private int _selectedPicIndex = -1;

        public PokedexHabitatEditor(
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

            InitializeAll();
            LoadAreaToUI(_currentAreaIdx);
        }

        private void InitializeAll()
        {
            ControlHelper.AttachAddressAutoFormat(txtAreaAddr, txtPageAddr);
            ControlHelper.AttachExternalBorder(picPokemon1, picPokemon2, picPokemon3, picPokemon4);
            ControlHelper.LoadComboBoxFromTextFile(cmbArea, "txt/PokedexAreaName.txt");

            // cmbPokemonName
            _pokemonNameManager = EntryManager<PokemonNameEntry>.Create(
                _romData, _tblReader, _config, "PokemonNameTableAddress", "PokemonNameCount");
            var pokemonNames = _pokemonNameManager.Original
                 .Select(entry => entry._PokemonName)
                 .ToArray();
            cmbPokemonName.Items.AddRange(pokemonNames);

            // sprite
            _spriteFrontImgManager = EntryManager<PokemonSpriteFrontImageEntry>.Create(
                _romData, _tblReader, _config, "PokemonSpriteFrontImageTableAddress", "PokemonSpriteCount");
            _spriteNormalPalManager = EntryManager<PokemonSpriteNormalPaletteEntry>.Create(
                _romData, _tblReader, _config, "PokemonSpriteNormalPaletteTableAddress", "PokemonSpriteCount");

            // all area
            uint? habitatTableAddr = _config.GetAddr("PokedexHabitatTableAddress");
            int habitatCount = cmbArea.Items.Count;
            _areaManager = new EntryManager<PokedexHabitatAreaEntry>(_romData, _tblReader);
            _areaManager.Load(habitatTableAddr, habitatCount);

            // pic
            _picPokemons = new PictureBox[] { picPokemon1, picPokemon2, picPokemon3, picPokemon4 };
            foreach (var pic in _picPokemons)
            {
                pic.Click += PicPokemon_Click;
                pic.Paint += PicPokemon_Paint;
            }
            cmbPokemonName.SelectedIndexChanged += CmbPokemonName_SelectedIndexChanged;

            // ui state
            btnSave.Enabled = false;
            _uiStateManager = new UIStateManager(hasChanges => btnSave.Enabled = hasChanges);
            _uiStateManager.AddBinaries((lstPage, null));

            // event handler
            cmbArea.SelectedIndexChanged += CmbArea_SelectedIndexChanged;
            lstPage.SelectedIndexChanged += (s, e) => LoadPageToUI(lstPage.SelectedIndex);
        }

        private void LoadAreaToUI(int areaIdx)
        {
            _currentAreaIdx = areaIdx;
            var areaEntry = _areaManager.Original[areaIdx];
            uint actualAddr = areaEntry.pAreaAddr - GbaConstants.BaseAddr;
            int pageCount = areaEntry.PageCount;
            var pages = IoHelper.ReadStructures<PokedexHabitatPageEntry>(
                _romData,
                actualAddr,
                pageCount,
                _tblReader);
            _currentAreaPages = pages.Select(p => CloneHelper.Clone(p)).ToList();

            // laod pokemons
            _currentAreaPokemonData = new List<ushort[]>();
            foreach (var page in pages)
            {
                ushort[] pokemonData = new ushort[page.PokemonCount];
                if (page.pPageAddr != 0 && page.PokemonCount > 0)
                {
                    uint pageActualAddr = page.pPageAddr - GbaConstants.BaseAddr;
                    for (int p = 0; p < page.PokemonCount; p++)
                    {
                        pokemonData[p] = BitConverter.ToUInt16(_romData, (int)(pageActualAddr + p * 2));
                    }
                }
                _currentAreaPokemonData.Add(pokemonData);
            }

            _isUpdatingUI = true;

            // toUI
            DataBindingHelper.BindObjectToControls(grpSelectArea, _areaManager.Original[areaIdx]);

            // uistate
            _uiStateManager.UpdateBinary(lstPage, ConvertAreaDataToBytes(_currentAreaPokemonData));
            _uiStateManager.UpdateInitialValues();

            // lstPage
            lstPage.Items.Clear();
            for (int i = 0; i < _currentAreaPages.Count; i++)
            {
                lstPage.Items.Add($"ページ {i + 1}");
            }

            if (lstPage.Items.Count > 0)
            {
                lstPage.SelectedIndex = 0;
            }

            _isUpdatingUI = false;
        }

        private void LoadPageToUI(int pageIdx)
        {
            if (pageIdx < 0) return;
            _currentPageIdx = pageIdx;
            _isUpdatingUI = true;

            var pageEntry = _currentAreaPages[pageIdx];
            DataBindingHelper.BindObjectToControls(grpSelectPage, pageEntry);

            // case null
            bool isNullPointer = (pageEntry.pPageAddr == 0);
            bool noPokemon = (pageEntry.PokemonCount == 0);

            if (isNullPointer || noPokemon)
            {
                cmbPokemonName.Enabled = false;
                cmbPokemonName.SelectedIndex = -1;

                foreach (var pic in _picPokemons)
                {
                    pic.Enabled = false;
                    pic.Image = null;
                    pic.Refresh();
                }

                _selectedPicIndex = -1;
                _isUpdatingUI = false;
                return;
            }

            cmbPokemonName.Enabled = true;
            var pokemonData = _currentAreaPokemonData[pageIdx];
            _selectedPicIndex = -1;

            for (int i = 0; i < _picPokemons.Length; i++)
            {
                if (i < pageEntry.PokemonCount)
                {
                    _picPokemons[i].Enabled = true;
                    _picPokemons[i].Image = GetPokemonSprite(pokemonData[i], true);

                    if (_selectedPicIndex == -1)
                    {
                        _selectedPicIndex = i;
                    }
                }
                else
                {
                    _picPokemons[i].Enabled = false;
                    _picPokemons[i].Image = null;
                }
                _picPokemons[i].Refresh();
            }

            UpdateComboBoxSelection();
            _isUpdatingUI = false;
        }

        private void CmbArea_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_isUpdatingUI) return;

            int newIndex = cmbArea.SelectedIndex;
            if (newIndex == _currentAreaIdx) return;

            if (btnSave.Enabled)
            {
                _isUpdatingUI = true;

                ControlHelper.HandleUnsavedChanges(
                    saveAction: () =>
                    {
                        /* 必要に応じてエリア保存処理を実装（現在のエリアの変更をROMに書き込む） */
                        LoadAreaToUI(cmbArea.SelectedIndex);
                    },
                    proceedAction: () =>
                    {
                        LoadAreaToUI(cmbArea.SelectedIndex);
                    },
                    cancelAction: () =>
                    {
                        cmbArea.SelectedIndex = _currentAreaIdx;
                    });

                _isUpdatingUI = false;
            }
            else
            {
                LoadAreaToUI(cmbArea.SelectedIndex);
            }
        }

        private void PicPokemon_Click(object sender, EventArgs e)
        {
            var pic = sender as PictureBox;
            if (pic == null || !pic.Enabled || pic.Image == null) return;

            int index = Array.IndexOf(_picPokemons, pic);
            if (index == -1 || index == _selectedPicIndex) return;

            _selectedPicIndex = index;

            foreach (var p in _picPokemons) p.Refresh();

            _isUpdatingUI = true;
            UpdateComboBoxSelection();
            _isUpdatingUI = false;
        }

        private void PicPokemon_Paint(object sender, PaintEventArgs e)
        {
            var pic = sender as PictureBox;
            if (pic == null) return;

            int index = Array.IndexOf(_picPokemons, pic);
            if (index == _selectedPicIndex)
            {
                using (var pen = new Pen(Color.Red, 2))
                {
                    e.Graphics.DrawRectangle(pen, 1, 1, pic.Width - 2, pic.Height - 2);
                }
            }
        }

        private void CmbPokemonName_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_isUpdatingUI || _selectedPicIndex == -1) return;

            int selectedPokemonIdx = cmbPokemonName.SelectedIndex;
            if (selectedPokemonIdx < 0) return;

            var pokemonData = _currentAreaPokemonData[_currentPageIdx];
            if (_selectedPicIndex < pokemonData.Length)
            {
                pokemonData[_selectedPicIndex] = (ushort)selectedPokemonIdx;
                _picPokemons[_selectedPicIndex].Image = GetPokemonSprite(selectedPokemonIdx, true);

                // uistate
                byte[] areaDataBytes = ConvertAreaDataToBytes(_currentAreaPokemonData);
                _uiStateManager.UpdateBinary(lstPage, areaDataBytes);
            }
        }

        private void UpdateComboBoxSelection()
        {
            if (_selectedPicIndex != -1)
            {
                var pokemonData = _currentAreaPokemonData[_currentPageIdx];
                if (_selectedPicIndex < pokemonData.Length)
                {
                    cmbPokemonName.SelectedIndex = pokemonData[_selectedPicIndex];
                }
            }
            else
            {
                cmbPokemonName.SelectedIndex = -1;
            }
        }

        private byte[] ConvertAreaDataToBytes(List<ushort[]> pageDataList)
        {
            using (var ms = new MemoryStream())
            using (var writer = new BinaryWriter(ms))
            {
                foreach (var pageData in pageDataList)
                {
                    foreach (var pokemonIdx in pageData)
                    {
                        writer.Write(pokemonIdx);
                    }
                }
                return ms.ToArray();
            }
        }

        private Bitmap GetPokemonSprite(int idx, bool showBackColor)
        {
            uint? imgAddr = _spriteFrontImgManager.Original[idx].pSpriteFrontImgAddr - GbaConstants.BaseAddr;
            uint? palAddr = _spriteNormalPalManager.Original[idx].pSpriteNormalPalAddr - GbaConstants.BaseAddr;

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
