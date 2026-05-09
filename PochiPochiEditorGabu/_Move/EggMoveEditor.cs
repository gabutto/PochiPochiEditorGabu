using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using PochiPochiEditorGabu.Constants;
using PochiPochiEditorGabu.FileReaders;
using PochiPochiEditorGabu.Helpers;
using PochiPochiEditorGabu.Managers;

namespace PochiPochiEditorGabu._Move
{
    public partial class EggMoveEditor : Form
    {
        protected byte[] _romData;
        protected IniFileReader _config;
        protected TblFileReader _tblReader;
        protected ReservationManager _reservationManager;

        private UIStateManager _uiStateManager;

        private EntryManager<EggMoveEntry> _eggMoveManager;

        private EntryManager<PokemonNameEntry> _pokemonNameManager;
        private EntryManager<PokemonSpriteFrontImageEntry> _spriteFrontImgManager;
        private EntryManager<PokemonSpriteNormalPaletteEntry> _spriteNormalPalManager;
        private EntryManager<MoveNameEntry> _moveNameManager;

        private class EggMoveListItem
        {
            public bool IsPokemon { get; }
            public int Index { get; }
            public string DisplayText { get; }

            public EggMoveListItem(bool isPokemon, int index, string displayText)
            {
                IsPokemon = isPokemon;
                Index = index;
                DisplayText = displayText;
            }

            public override string ToString() => DisplayText;
        }

        public EggMoveEditor(
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
            InitializeControls();
            InitializeUIStates();
            InitializeEventHandlers();

            LoadEggMovesToUI();

            if (lstEggMoves.Items.Count > 0)
            {
                lstEggMoves.SelectedIndex = 0;
            }
        }

        private void InitializeManagers()
        {
            // egg move
            _eggMoveManager = new EntryManager<EggMoveEntry>(_romData, _tblReader);

            // name
            _pokemonNameManager = EntryManager<PokemonNameEntry>.Create(
                _romData, _tblReader, _config, "PokemonNameTableAddress", "PokemonNameCount");

            // sprite
            _spriteFrontImgManager = EntryManager<PokemonSpriteFrontImageEntry>.Create(
                _romData, _tblReader, _config, "PokemonSpriteFrontImageTableAddress", "PokemonSpriteCount");
            _spriteNormalPalManager = EntryManager<PokemonSpriteNormalPaletteEntry>.Create(
                _romData, _tblReader, _config, "PokemonSpriteNormalPaletteTableAddress", "PokemonSpriteCount");

            // move name
            _moveNameManager = EntryManager<MoveNameEntry>.Create(
                _romData, _tblReader, _config, "MoveNameTableAddress", "MoveNameCount");
        }

        private void InitializeControls()
        {
            // pokemon name for cmb
            var pokemonNames = _pokemonNameManager.Original
                             .Select(entry => entry._PokemonName)
                             .ToArray();
            cmbPokemon.Items.AddRange(pokemonNames);

            // move for cmb
            var moveNames = _moveNameManager.Original
                             .Select(entry => entry._MoveName)
                             .ToArray();
            cmbMove.Items.AddRange(moveNames);

            ControlHelper.AttachExternalBorder(picPokemon);
        }

        private void InitializeUIStates()
        {
            btnSave.Enabled = false;
            _uiStateManager = new UIStateManager(hasChanges => btnSave.Enabled = hasChanges);
        }

        private void InitializeEventHandlers()
        {
            lstEggMoves.SelectedIndexChanged += LstEggMoves_SelectedIndexChanged;
            cmbPokemon.SelectedIndexChanged += CmbPokemon_SelectedIndexChanged;

            btnPokemonInsert.Click += BtnPokemonInsert_Click;
            btnPokemonReplace.Click += BtnPokemonReplace_Click;
            btnPokemonRemove.Click += BtnRemove_Click;

            btnMoveInsert.Click += BtnMoveInsert_Click;
            btnMoveReplace.Click += BtnMoveReplace_Click;
            btnMoveRemove.Click += BtnRemove_Click;

            btnSave.Click += BtnSave_Click;
            this.FormClosing += EggMoveEditor_FormClosing;
        }

        private void LoadEggMovesToUI()
        {
            uint? tableAddr = _config.GetAddr("EggMoveTableAddress");

            int count = 0;
            int currentOffset = (int)tableAddr;
            while (true)
            {
                ushort val = BitConverter.ToUInt16(_romData, currentOffset);
                if (val == GbaConstants.EggMoveTableTerminator)
                {
                    break;
                }
                count++;
                currentOffset += 2;
            }

            _eggMoveManager.Load(tableAddr, count);

            var tableBytes = new List<byte>();
            foreach (var entry in _eggMoveManager.Working)
            {
                tableBytes.AddRange(BitConverter.GetBytes(entry._MoveIdx));
            }
            tableBytes.AddRange(BitConverter.GetBytes(GbaConstants.EggMoveTableTerminator));
            byte[] data = tableBytes.ToArray();
            _uiStateManager.AddBinaries(("EggMoveTable", data));

            RefreshEggMoveTableDisplay();
        }

        private void RefreshEggMoveTableDisplay()
        {
            int selectedIndex = lstEggMoves.SelectedIndex;
            int topIndex = lstEggMoves.TopIndex;

            lstEggMoves.BeginUpdate();
            lstEggMoves.Items.Clear();

            foreach (var entry in _eggMoveManager.Working)
            {
                ushort value = entry._MoveIdx;

                if (value >= GbaConstants.SpeciesIndexThreshold)
                {
                    int pokemonIndex = value - GbaConstants.SpeciesIndexThreshold;
                    string pokemonName = pokemonIndex < _pokemonNameManager.Original.Count
                        ? _pokemonNameManager.Original[pokemonIndex]._PokemonName
                        : "???";

                    lstEggMoves.Items.Add(new EggMoveListItem(true, pokemonIndex, pokemonName));
                }
                else
                {
                    int moveIndex = value;
                    string moveName = moveIndex < _moveNameManager.Original.Count
                        ? _moveNameManager.Original[moveIndex]._MoveName
                        : "???";

                    lstEggMoves.Items.Add(new EggMoveListItem(false, moveIndex, "  " + moveName));
                }
            }

            lstEggMoves.EndUpdate();

            if (selectedIndex >= 0 && selectedIndex < lstEggMoves.Items.Count)
            {
                lstEggMoves.SelectedIndex = selectedIndex;
            }
            else if (lstEggMoves.Items.Count > 0)
            {
                lstEggMoves.SelectedIndex = lstEggMoves.Items.Count - 1;
            }

            if (topIndex >= 0 && topIndex < lstEggMoves.Items.Count)
            {
                lstEggMoves.TopIndex = topIndex;
            }
        }

        private void UpdateBinaryState()
        {
            var tableBytes = new List<byte>();
            foreach (var entry in _eggMoveManager.Working)
            {
                tableBytes.AddRange(BitConverter.GetBytes(entry._MoveIdx));
            }
            tableBytes.AddRange(BitConverter.GetBytes(GbaConstants.EggMoveTableTerminator));

            byte[] data = tableBytes.ToArray();
            _uiStateManager.UpdateBinary("EggMoveTable", data);
        }

        private void LstEggMoves_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!(lstEggMoves.SelectedItem is EggMoveListItem item)) return;

            if (item.IsPokemon)
            {
                if (item.Index >= 0 && item.Index < cmbPokemon.Items.Count)
                {
                    cmbPokemon.SelectedIndex = item.Index;
                }

                if (cmbMove.Items.Count > 0)
                {
                    cmbMove.SelectedIndex = 0;
                }

                SetControlsEnabled(pokemonControlsEnabled: true, moveControlsEnabled: false);
            }
            else
            {
                int parentPokemonIndex = -1;
                for (int i = lstEggMoves.SelectedIndex - 1; i >= 0; i--)
                {
                    if (lstEggMoves.Items[i] is EggMoveListItem pItem && pItem.IsPokemon)
                    {
                        parentPokemonIndex = pItem.Index;
                        break;
                    }
                }

                if (parentPokemonIndex != -1 && parentPokemonIndex < cmbPokemon.Items.Count)
                {
                    cmbPokemon.SelectedIndex = parentPokemonIndex;
                }

                if (item.Index >= 0 && item.Index < cmbMove.Items.Count)
                {
                    cmbMove.SelectedIndex = item.Index;
                }

                SetControlsEnabled(pokemonControlsEnabled: false, moveControlsEnabled: true);
            }
        }

        private void SetControlsEnabled(bool pokemonControlsEnabled, bool moveControlsEnabled)
        {
            btnPokemonReplace.Enabled = pokemonControlsEnabled;
            btnPokemonRemove.Enabled = pokemonControlsEnabled;

            btnMoveReplace.Enabled = moveControlsEnabled;
            btnMoveRemove.Enabled = moveControlsEnabled;
        }

        private void CmbPokemon_SelectedIndexChanged(object sender, EventArgs e)
        {
            int pokemonIndex = cmbPokemon.SelectedIndex;
            if (pokemonIndex < 0) return;

            picPokemon.Image?.Dispose();
            picPokemon.Image = GetPokemonSprite(pokemonIndex, true);
        }

        private void BtnPokemonInsert_Click(object sender, EventArgs e)
        {
            int pokemonIndex = cmbPokemon.SelectedIndex;
            if (pokemonIndex < 0) return;

            ushort val = (ushort)(GbaConstants.SpeciesIndexThreshold + pokemonIndex);
            InsertIntoTable(val);
        }

        private void BtnMoveInsert_Click(object sender, EventArgs e)
        {
            int moveIndex = cmbMove.SelectedIndex;
            if (moveIndex < 0) return;

            ushort val = (ushort)moveIndex;
            InsertIntoTable(val);
        }

        private void InsertIntoTable(ushort value)
        {
            int insertIndex = lstEggMoves.SelectedIndex == -1 ? _eggMoveManager.Working.Count : lstEggMoves.SelectedIndex;
            _eggMoveManager.Working.Insert(insertIndex, new EggMoveEntry { _MoveIdx = value });

            UpdateBinaryState();
            RefreshEggMoveTableDisplay();
            lstEggMoves.SelectedIndex = insertIndex;
        }

        private void BtnPokemonReplace_Click(object sender, EventArgs e)
        {
            if (!(lstEggMoves.SelectedItem is EggMoveListItem item) || !item.IsPokemon) return;

            int pokemonIndex = cmbPokemon.SelectedIndex;
            if (pokemonIndex < 0) return;

            ushort val = (ushort)(GbaConstants.SpeciesIndexThreshold + pokemonIndex);
            ReplaceInTable(val);
        }

        private void BtnMoveReplace_Click(object sender, EventArgs e)
        {
            if (!(lstEggMoves.SelectedItem is EggMoveListItem item) || item.IsPokemon) return;

            int moveIndex = cmbMove.SelectedIndex;
            if (moveIndex < 0) return;

            ushort val = (ushort)moveIndex;
            ReplaceInTable(val);
        }

        private void ReplaceInTable(ushort value)
        {
            int replaceIndex = lstEggMoves.SelectedIndex;
            if (replaceIndex < 0 || replaceIndex >= _eggMoveManager.Working.Count) return;

            if (_eggMoveManager.Working[replaceIndex]._MoveIdx == value) return;

            _eggMoveManager.Working[replaceIndex]._MoveIdx = value;

            UpdateBinaryState();
            RefreshEggMoveTableDisplay();
        }

        private void BtnRemove_Click(object sender, EventArgs e)
        {
            int deleteIndex = lstEggMoves.SelectedIndex;
            if (deleteIndex < 0 || deleteIndex >= _eggMoveManager.Working.Count) return;

            _eggMoveManager.Working.RemoveAt(deleteIndex);

            UpdateBinaryState();
            RefreshEggMoveTableDisplay();

            if (lstEggMoves.Items.Count > 0)
            {
                lstEggMoves.SelectedIndex = Math.Min(deleteIndex, lstEggMoves.Items.Count - 1);
            }
        }

        private void SaveEggMoveTable()
        {
            int tableAddr = (int)_config.GetAddr("EggMoveTableAddress");
            var saveList = new List<EggMoveEntry>(_eggMoveManager.Working)
            {
                new EggMoveEntry { _MoveIdx = GbaConstants.EggMoveTableTerminator }
            };

            IoHelper.WriteStructures(
                _romData,
                tableAddr,
                saveList,
                _tblReader,
                null,
                false
            );

            if (_config.GetBool("EnableEggMoveSizeCalc"))
            {
                uint? sizeAddr = _config.GetAddr("EggMoveTableSizeAddress");
                if (sizeAddr != null)
                {
                    int addr = (int)sizeAddr.Value;
                    ushort sizeValue = (ushort)(_eggMoveManager.Working.Count - 1);
                    byte[] sizeBytes = BitConverter.GetBytes(sizeValue);
                    Array.Copy(sizeBytes, 0, _romData, addr, 2);
                }
            }

            _eggMoveManager.Original.Clear();
            _eggMoveManager.Original.AddRange(_eggMoveManager.Working.Select(x => CloneHelper.Clone(x)));
            _uiStateManager.UpdateInitialValues();
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            SaveEggMoveTable();
        }

        private void EggMoveEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (btnSave.Enabled)
            {
                DialogResult result = ControlHelper.HandleUnsavedChanges(
                    () =>
                    {
                        SaveEggMoveTable();
                    },
                    () =>
                    {
                        //
                    },
                    () =>
                    {
                        e.Cancel = true;
                    }
                );
            }
        }

        private Bitmap GetPokemonSprite(int idx, bool showBackColor)
        {
            uint imageAddr = _spriteFrontImgManager.Original[idx].pSpriteFrontImgAddr - GbaConstants.BaseAddr;
            uint palAddr = _spriteNormalPalManager.Original[idx].pSpriteNormalPalAddr - GbaConstants.BaseAddr;

            try
            {
                Color[] palette;
                byte[] imageData;

                // palette
                palette = ImageManager.DecompressPalette(_romData, palAddr, showBackColor);

                // image
                imageData = ImageManager.DecompressLZ77(_romData, imageAddr);

                return ImageManager.CreateSprite(
                    imageData,
                    palette,
                    GbaConstants.SpriteSize,
                    GbaConstants.SpriteSize,
                    true);
            }
            catch
            {
                return null;
            }
        }
    }
}
