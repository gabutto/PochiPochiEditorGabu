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

namespace PochiPochiEditorGabu._Map
{
    public partial class OverworldSpriteEditor : Form
    {
        protected byte[] _romData;
        protected IniFileReader _config;
        protected TblFileReader _tblReader;
        protected ReservationManager _reservationManager;

        private UIStateManager _uiStateManager;
        private TableParsingHelper _tableParsingHelper;

        Dictionary<int, IReadOnlyList<PointerEntry>> _dataPointers;
        private List<OverworldDataEntry> _originalDataEntries;
        private List<OverworldDataEntry> _workingDataEntries;
        private EntryManager<OverworldPaletteEntry> _palManager;
        private EntryManager<OverworldFontEntry> _fontManager;

        private bool _isUpdatingUI = false;
        private bool _isMultipleTable = false;
        private int _currentDataTableIdx = 0;
        private int _currentDataEntryIdx = 0;

        private class PaletteComboItem
        {
            public ushort PalIdx { get; set; }
            public int? TableIdx { get; set; } // dummy = null, temporary = -1
            public bool IsTemporary { get; set; }
            public uint PalAddr { get; set; }
            public string DisplayText => $"{PalIdx:X4}" + (IsTemporary ? "*" : "");
        }

        private class DataSizeComboItem
        {
            public string Key { get; set; }
            public int Width { get; set; }
            public int Height { get; set; }
            public int VramSize { get; set; }
            public string DisplayText => $"{Key} (0x{VramSize:X})";
        }

        private IReadOnlyList<DataSizeComboItem> _dataSizePresets = new List<DataSizeComboItem>
        {
            new DataSizeComboItem { Key = "16x32",  Width = 0x10, Height = 0x20, VramSize = 0x100 },
            new DataSizeComboItem { Key = "32x32",  Width = 0x20, Height = 0x20, VramSize = 0x200 },
            new DataSizeComboItem { Key = "16x16",  Width = 0x10, Height = 0x10, VramSize = 0x80 },
            new DataSizeComboItem { Key = "64x64",  Width = 0x40, Height = 0x40, VramSize = 0x800 },
            new DataSizeComboItem { Key = "128x64", Width = 0x80, Height = 0x40, VramSize = 0x1000 },
            new DataSizeComboItem { Key = "32x16",  Width = 0x20, Height = 0x10, VramSize = 0x100 }
        };

        public OverworldSpriteEditor(
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

            LoadDataEntryToUI(_currentDataEntryIdx);
        }

        private void InitializeManagers()
        {
            _isMultipleTable = _config.GetBool("IsAppliedCFRU") &&
                _config.GetBool("EnableMultipleOverworldSpriteDataTable");
            _tableParsingHelper = new TableParsingHelper(_romData);
            _dataPointers = new Dictionary<int, IReadOnlyList<PointerEntry>>();
            string validationPattern = 
                "FF FF ?? 11 ?? 11 ?? ?? ?? 00 ?? 00 ?? ?? ?? 00 PP PP PP PP PP PP PP PP PP PP PP PP PP PP PP PP PP PP PP PP";

            // data
            if (_isMultipleTable) // multiple
            {
                uint? dataGroupsAddr = _config.GetAddr("MultipleOverworldSpriteDataTableAddress");

                // calc data groups count
                IReadOnlyList<PointerEntry> dataGroupPointer = _tableParsingHelper.ParsePointerEntries(
                    (int)dataGroupsAddr,
                    "PP PP PP PP",
                    _config.GetInt("MultipleOverworldSpriteDataTableCount"));

                // calc data pointer count
                foreach (var dataGroupAddr in dataGroupPointer)
                {
                    IReadOnlyList<PointerEntry> dataPointer = _tableParsingHelper.ParsePointerEntries(
                        (int)dataGroupAddr.TargetOffset,
                        "PP PP PP PP",
                        _config.GetInt("MultipleOverworldSpriteDataEntryMaxCount"));

                    // valid?
                    var validPointers = new List<PointerEntry>();
                    foreach (var ptr in dataPointer)
                    {
                        var validationResult = 
                            _tableParsingHelper.ParseDataEntries(
                                (int)ptr.TargetOffset, 
                                validationPattern, 
                                1);

                        if (validationResult.Count == 0)
                        {
                            break;
                        }

                        validPointers.Add(ptr);
                    }

                    _dataPointers.Add(dataGroupAddr.Index, validPointers);
                }
            }
            else // single
            {
                uint? dataPointersAddr = _config.GetAddr("OverworldSpriteDataTableAddress");
                uint? entryCountAddr = _config.GetAddr("OverworldSpriteDataLastIndex");
                int entryCount = _romData[(int)entryCountAddr] + 1; // plus 1
                IReadOnlyList<PointerEntry> dataPointer = _tableParsingHelper.ParsePointerEntries(
                    (int)dataPointersAddr,
                    "PP PP PP PP",
                    entryCount,
                    knownCount: entryCount);

                // to index 0
                _dataPointers.Add(0, dataPointer);
            }

            // _currentDataTableIdx = 0
            LoadCurrentTableData();

            // pal
            uint? palTableAddr = _config.GetAddr("OverworldSpritePaletteTableAddress");
            IReadOnlyList<DataEntry> palEntries = _tableParsingHelper.ParseDataEntries(
                (int)palTableAddr,
                "?? ?? ?? ?? ?? 11 00 00"); // 1100 - 11FF
            int palentryCount = palEntries.Count;
            _palManager = new EntryManager<OverworldPaletteEntry>(_romData, _tblReader);
            _palManager.Load(palTableAddr, palentryCount);

            uint? fontTableAddr = _config.GetAddr("OverworldSpriteFontTableAddress");
            int fontEntryCount = (_dataPointers[_currentDataTableIdx].Count + 1) / 2;
            _fontManager = new EntryManager<OverworldFontEntry>(_romData, _tblReader);
            _fontManager.Load(fontTableAddr, fontEntryCount);
        }

        private void InitializeEventHandlers()
        {
            nudDataTableIdx.ValueChanged += nudDataTableIdx_ValueChanged;
            lstDataEntry.SelectedIndexChanged += lstDataEntry_SelectedIndexChanged;
        }

        private void InitializeControls()
        {
            // nudDataTableIdx
            nudDataTableIdx.Maximum = _dataPointers.Count - 1;
            nudDataEntryCount.Value = _dataPointers[_currentDataTableIdx].Count;

            // data size cmb
            cmbDataSize.DataSource = new List<DataSizeComboItem>(_dataSizePresets);
            cmbDataSize.DisplayMember = nameof(DataSizeComboItem.DisplayText);
            cmbCreateNewImgTableSize.DataSource = new List<DataSizeComboItem>(_dataSizePresets);
            cmbCreateNewImgTableSize.DisplayMember = nameof(DataSizeComboItem.DisplayText);

            ControlHelper.AttachAddressAutoFormat(
                txtDataEntryAddr,
                txtDataLoadAddr, txtDataSizeAddr, txtDataAnimAddr, txtDataImgTableAddr, txtDataMemoryAddr,
                txtSpriteFrameAddr);
            ControlHelper.AttachExternalBorder(
                picSpritePreviewFrame,
                picPalPreview);
            ControlHelper.AttachNumericUpDownNavigators(
                nudDataTableIdx, btnDataTablePrev, btnDataTableNext);
            ControlHelper.AttachNumericUpDownNavigators(
                nudSpriteFrameCount, btnSpriteFrameCountPrev, btnSpriteFrameCountNext);
            ControlHelper.LoadComboBoxFromTextFile(cmbDataFootprint, "txt/OverworldSpriteFootprint.txt");
            ControlHelper.LoadComboBoxFromTextFile(cmbFontIdx, "txt/OverworldSpriteFont.txt");

            UpdateDataEntryListBox();
            UpdatePaletteComboBox();
        }

        private void InitializeUIStates()
        {
            btnSave.Enabled = false;
            _uiStateManager = new UIStateManager(hasChanges => btnSave.Enabled = hasChanges);
        }

        private void UpdateDataEntryListBox()
        {
            _isUpdatingUI = true;
            lstDataEntry.BeginUpdate();
            lstDataEntry.Items.Clear();

            int entryCount = _dataPointers[_currentDataTableIdx].Count;

            for (int i = 0; i < entryCount; i++)
            {
                lstDataEntry.Items.Add($"No. {i:D4}");
            }

            lstDataEntry.EndUpdate();
            _isUpdatingUI = false;

            if (lstDataEntry.Items.Count > 0)
            {
                lstDataEntry.SelectedIndex = 0;
            }
        }

        private void UpdatePaletteComboBox()
        {
            var items = new List<PaletteComboItem>();
            for (int i = 0; i < _palManager.Working.Count; i++)
            {
                items.Add(new PaletteComboItem
                {
                    PalIdx = _palManager.Working[i]._Idx,
                    TableIdx = i,
                    IsTemporary = false,
                    PalAddr = _palManager.Working[i].pPalAddr
                });
            }

            // 11FF dummy
            if (!items.Any(x => x.PalIdx == 0x11FF))
            {
                items.Add(new PaletteComboItem
                {
                    PalIdx = 0x11FF,
                    TableIdx = null,
                    IsTemporary = true,
                    PalAddr = 0 // null pointer
                });
            }

            /*
            
            // ② 一時的に追加されたパレットをリストに追加（将来の実装向け）
            // ※ReservationManagerを使ってtxtCreateNewPalIdxが予約された際に、
            // _temporaryPalettesにそのIDを追加し、このメソッドを呼ぶ設計にします。
            foreach (var tempIdx in _temporaryPalettes)
            {
                // 既に同名のIDが存在しないかチェックするのも良いでしょう
                if (!items.Any(x => x.PalIdx == tempIdx))
                {
                    items.Add(new PaletteComboItem
                    {
                        PalIdx = tempIdx,
                        OriginalTableIndex = -1, // 元テーブルには存在しないため -1
                        IsTemporary = true
                    });
                }
            }

            */

            var sortedItems = items.OrderBy(x => x.PalIdx).ToList();

            foreach (var cmb in new[] {
                cmbPalIdx,
                cmbDataPalIdx1,
                cmbDataPalIdx2 })
            {
                cmb.BeginUpdate();

                ushort? currentSelectedIdx = (cmb.SelectedItem as PaletteComboItem)?.PalIdx;
                cmb.DisplayMember = nameof(PaletteComboItem.DisplayText);
                cmb.ValueMember = nameof(PaletteComboItem.TableIdx);
                cmb.DataSource = sortedItems.ToList();

                // cursor
                if (currentSelectedIdx.HasValue)
                {
                    var itemToSelect = (cmb.DataSource as List<PaletteComboItem>)
                        .FirstOrDefault(x => x.PalIdx == currentSelectedIdx.Value);
                    if (itemToSelect != null)
                    {
                        cmb.SelectedItem = itemToSelect;
                    }
                }

                cmb.EndUpdate();
            }
        }

        private void LoadDataEntryToUI(int idx)
        {
            _isUpdatingUI = true;
            _reservationManager.ClearAllReservations();

            _currentDataEntryIdx = idx;
            var currentEntry = _workingDataEntries[idx];
            DataBindingHelper.BindObjectToControls(this, currentEntry);

            // pal
            LoadToPalComboBox(cmbDataPalIdx1, currentEntry._PalIdx1);
            LoadToPalComboBox(cmbDataPalIdx2, currentEntry._PalIdx2);

            // size cmb
            ushort width = currentEntry._ImgWidth;
            ushort height = currentEntry._ImgHeight;
            LoadToSizeComboBox(width, height);

            // pal slot and unknown flags
            byte paletteSlotAndUnknown = currentEntry._PalSlotAndUnknownFlags;
            nudDataPalSlot.Value = paletteSlotAndUnknown & GbaConstants.NibbleMask;
            chkUnknownFlag1.Checked = (paletteSlotAndUnknown & GbaConstants.OverworldSpriteUnknownFlag1Mask) != 0;
            chkUnknownFlag2.Checked = (paletteSlotAndUnknown & GbaConstants.OverworldSpriteUnknownFlag2Mask) != 0;
            chkUnknownFlag3.Checked = (paletteSlotAndUnknown & GbaConstants.OverworldSpriteUnknownFlag3Mask) != 0;

            // font
            int fontByteIndex = idx / 2;
            if (fontByteIndex < _fontManager.Working.Count)
            {
                byte fontData = _fontManager.Working[fontByteIndex]._Idx;
                int fontId = (idx % 2 == 0)
                    ? (fontData & GbaConstants.NibbleMask)
                    : ((fontData & (GbaConstants.NibbleMask << 4)) >> GbaConstants.NibbleShift);
                cmbFontIdx.SelectedValue = fontId;
            }

            // ui
            uint entryAddr = _dataPointers[_currentDataTableIdx][idx].TargetOffset;
            txtDataEntryAddr.Text = entryAddr.ToString("X8");

            _isUpdatingUI = false;
            _uiStateManager.UpdateInitialValues();
        }

        private void LoadCurrentTableData()
        {
            _originalDataEntries = new List<OverworldDataEntry>();
            _workingDataEntries = new List<OverworldDataEntry>();

            for (int i = 0; i < _dataPointers[_currentDataTableIdx].Count; i++)
            {
                uint entryAddr = _dataPointers[_currentDataTableIdx][i].TargetOffset;
                var manager = new EntryManager<OverworldDataEntry>(_romData, _tblReader);
                manager.Load(entryAddr, 1);

                _originalDataEntries.Add(manager.Original[0]);
                _workingDataEntries.Add(manager.Working[0]);
            }
        }

        private void LoadToPalComboBox(ComboBox cmb, ushort palIdx)
        {
            bool isFound = false;

            for (int i = 0; i < cmb.Items.Count; i++)
            {
                var item = (PaletteComboItem)cmb.Items[i];

                if (item.PalIdx == palIdx)
                {
                    cmb.SelectedIndex = i;
                    isFound = true;
                    break;
                }
            }

            if (!isFound)
            {
                cmb.SelectedIndex = -1;
            }
        }

        private void LoadToSizeComboBox(ushort width, ushort height)
        {
            bool isFound = false;

            for (int i = 0; i < cmbDataSize.Items.Count; i++)
            {
                var item = (DataSizeComboItem)cmbDataSize.Items[i];

                if (item.Width == width && item.Height == height)
                {
                    cmbDataSize.SelectedIndex = i;
                    isFound = true;
                    break;
                }
            }

            if (!isFound)
            {
                cmbDataSize.SelectedIndex = -1;
            }
        }






























        private void nudDataTableIdx_ValueChanged(object sender, EventArgs e)
        {
            if (_isUpdatingUI) return;

            _currentDataTableIdx = (int)nudDataTableIdx.Value;
            nudDataEntryCount.Value = _dataPointers[_currentDataTableIdx].Count;

            LoadCurrentTableData();
            UpdateDataEntryListBox();
        }

        private void lstDataEntry_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_isUpdatingUI) return;

            int newentryIndex = lstDataEntry.SelectedIndex;
            if (newentryIndex == _currentDataEntryIdx) return;

            if (btnSave.Enabled)
            {
                ControlHelper.HandleUnsavedChanges(
                    () =>
                    {
                        SaveCurrentAllData(_currentDataEntryIdx);
                        ResetControls();
                        LoadDataEntryToUI(newentryIndex);
                    },
                    () =>
                    {
                        RestoreData(_currentDataEntryIdx);
                        ResetControls();
                        LoadDataEntryToUI(newentryIndex);
                    },
                    () =>
                    {
                        lstDataEntry.SelectedIndex = _currentDataEntryIdx;
                    }
                );
            }
            else
            {
                ResetControls();
                LoadDataEntryToUI(newentryIndex);
            }
        }

        private void ResetControls()
        {

        }

        private void RestoreData(int idx)
        {

        }

        private void SaveCurrentAllData(int idx)
        {

        }
    }
}
