using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
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

        private BindingList<PaletteComboItem> _paletteComboSource;
        private PaletteComboItem _temporaryPalette = new PaletteComboItem();
        private List<Bitmap> _loadedSpriteFrames = new List<Bitmap>();
        private List<uint> _loadedSpriteAddresses = new List<uint>();
        private Dictionary<uint, byte[]> _temporarySpriteFrames = new Dictionary<uint, byte[]>();

        private bool _isUpdatingUI = false;
        private bool _isMultipleTable = false;
        private int _currentDataTableIdx = 0;
        private int _currentDataEntryIdx = 0;

        private class PaletteComboItem
        {
            public ushort PalIdx { get; set; }
            public int? TableIdx { get; set; } // dummy = null, temporary = -1
            public bool IsTemporary { get; set; }
            public byte[] TemporaryData { get; set; }
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

            BackUpCurrentTableData();
            LoadDataEntryListBox();
            LoadDataEntryToUI(_currentDataEntryIdx);
        }

        private void InitializeManagers()
        {
            _isMultipleTable = _config.GetBool("IsAppliedCFRU") &&
                _config.GetBool("EnableMultipleOverworldSpriteDataTable");
            _tableParsingHelper = new TableParsingHelper(_romData);
            _dataPointers = new Dictionary<int, IReadOnlyList<PointerEntry>>();

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
                        null,
                        null,
                        true);

                    // valid?
                    var validPointers = new List<PointerEntry>();
                    string validationPattern =
                        "FF FF ?? 11 ?? 11 ?? ?? ?? 00 ?? 00 ?? ?? ?? 00 PP PP PP PP PP PP PP PP PP PP PP PP PP PP PP PP PP PP PP PP";
                    foreach (var ptr in dataPointer)
                    {
                        if (ptr.TargetOffset != 0)
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
                    null,
                    true);

                // to index 0
                _dataPointers.Add(0, dataPointer);
            }

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
            btnSave.Click += btnSave_Click;
            this.FormClosing += OverworldSpriteEditor_FormClosing;

            nudDataTableIdx.ValueChanged += nudDataTableIdx_ValueChanged;
            lstDataEntry.SelectedIndexChanged += lstDataEntry_SelectedIndexChanged;

            cmbDataPalIdx1.SelectedIndexChanged += cmbDataPalIdx1_SelectedIndexChanged;
            nudSpriteFrameCount.ValueChanged += nudSpriteFrameCount_ValueChanged;

            cmbPalIdx.SelectedIndexChanged += cmbPalIdx_SelectedIndexChanged;
            btnCreateNewPalIdx.Click += btnCreateNewPalIdx_Click;

            btnImportDataEntry.Click += btnImportDataEntry_Click;
            btnExportDataEntry.Click += btnExportDataEntry_Click;
            btnCreateNewDataEntry.Click += btnCreateNewDataEntry_Click;

            btnCreateNewImgTable.Click += btnCreateNewImgTable_Click;

            btnImportSpriteFrames.Click += btnImportSpriteFrames_Click;
            btnExportSpriteFrames.Click += btnExportSpriteFrames_Click;
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

            // picSpritePreviewFrame
            picSpritePreviewFrame.SizeMode = PictureBoxSizeMode.CenterImage;
            picSpritePreviewFrame.BackColor = Color.White;

            ControlHelper.AttachAddressAutoFormat(
                txtDataEntryAddr,
                txtDataLoadAddr, txtDataSizeAddr, txtDataAnimAddr, txtDataImgTableAddr, txtDataMemoryAddr,
                txtSpriteFrameAddr, txtCreateNewDataEntryAddr);
            ControlHelper.AttachExternalBorder(
                picSpritePreviewFrame,
                picPalPreview);
            ControlHelper.AttachNumericUpDownNavigators(
                nudDataTableIdx, btnDataTablePrev, btnDataTableNext);
            ControlHelper.AttachNumericUpDownNavigators(
                nudSpriteFrameCount, btnSpriteFrameCountPrev, btnSpriteFrameCountNext);
            ControlHelper.LoadComboBoxFromTextFile(cmbDataFootprint, "txt/OverworldSpriteFootprint.txt");
            ControlHelper.LoadComboBoxFromTextFile(cmbFontIdx, "txt/OverworldSpriteFont.txt");

            InitializePaletteComboBox();
        }

        private void InitializeUIStates()
        {
            btnSave.Enabled = false;
            _uiStateManager = new UIStateManager(hasChanges => btnSave.Enabled = hasChanges);

            _uiStateManager.AddControls(
                txtDataEntryAddr);
            _uiStateManager.AddControlsRecursive(
                grpDataEntry);
            _uiStateManager.AddBinaries(
                (_temporaryPalette, null),
                ("NewDataEntry", null),
                ("ImportedSprites", null));
        }

        private void InitializePaletteComboBox()
        {
            var items = new List<PaletteComboItem>();

            for (int i = 0; i < _palManager.Original.Count; i++)
            {
                var palEntry = _palManager.Original[i];

                items.Add(new PaletteComboItem
                {
                    PalIdx = _palManager.Original[i]._Idx,
                    TableIdx = i,
                    IsTemporary = false,
                    TemporaryData = null,
                    PalAddr = palEntry.pPalAddr == 0
                        ? 0
                        : palEntry.pPalAddr - GbaConstants.BaseAddr
                });
            }

            // 11FF 
            if (!items.Any(x => x.PalIdx == 0x11FF))
            {
                items.Add(new PaletteComboItem
                {
                    PalIdx = 0x11FF,
                    TableIdx = null,
                    IsTemporary = true,
                    TemporaryData = null,
                    PalAddr = 0 // null pointer
                });
            }

            // sort
            var sortedItems = items.OrderBy(x => x.PalIdx).ToList();
            _paletteComboSource = new BindingList<PaletteComboItem>(sortedItems);

            foreach (var cmb in new[] {
                cmbDataPalIdx1,
                cmbDataPalIdx2,
                cmbPalIdx })
            {
                var bindingSource = new BindingSource();
                bindingSource.DataSource = _paletteComboSource;

                cmb.DisplayMember = nameof(PaletteComboItem.DisplayText);
                cmb.ValueMember = nameof(PaletteComboItem.TableIdx);
                cmb.DataSource = bindingSource;
            }
        }

        private void BackUpCurrentTableData()
        {
            _originalDataEntries = new List<OverworldDataEntry>();
            _workingDataEntries = new List<OverworldDataEntry>();

            for (int i = 0; i < _dataPointers[_currentDataTableIdx].Count; i++)
            {
                uint entryAddr = _dataPointers[_currentDataTableIdx][i].TargetOffset;

                if (entryAddr == 0) // null pointer
                {
                    _originalDataEntries.Add(new OverworldDataEntry());
                    _workingDataEntries.Add(new OverworldDataEntry());
                }
                else
                {
                    var manager = new EntryManager<OverworldDataEntry>(_romData, _tblReader);
                    manager.Load(entryAddr, 1);

                    _originalDataEntries.Add(manager.Original[0]);
                    _workingDataEntries.Add(manager.Working[0]);
                }
            }
        }

        private void LoadDataEntryListBox()
        {
            _isUpdatingUI = true;
            lstDataEntry.BeginUpdate();
            lstDataEntry.Items.Clear();

            int entryCount = _originalDataEntries.Count;

            for (int i = 0; i < entryCount; i++)
            {
                lstDataEntry.Items.Add($"No. {i:D4}");
            }

            lstDataEntry.EndUpdate();

            if (lstDataEntry.Items.Count > 0)
            {
                lstDataEntry.SelectedIndex = 0;
            }

            _isUpdatingUI = false;
        }

        private void LoadDataEntryToUI(int idx)
        {
            _isUpdatingUI = true;
            _reservationManager.ClearAllReservations();

            _temporarySpriteFrames.Clear();
            _uiStateManager.UpdateBinary("ImportedSprites", null);

            _currentDataEntryIdx = idx;
            uint entryAddr = _dataPointers[_currentDataTableIdx][idx].TargetOffset;

            var excludeControls = new[] { "cmbDataSize" };

            if (entryAddr == 0)
            {
                grpDataEntry.SetControlsEnabled(false, excludeControls);
                grpDataEntry.ResetControls(excludeControls);

                grpSpritePreview.SetControlsEnabled(false);
                grpSpritePreview.ResetControls();

                grpCreateNewImgTable.SetControlsEnabled(false);
                grpCreateNewImgTable.ResetControls();

                // ui
                txtDataEntryAddr.Text = "null";
                btnImportDataEntry.Enabled = false;
                btnExportDataEntry.Enabled = false;

                txtDataImgTableAddr.Text = string.Empty;
                LoadSpriteFrames();

                _isUpdatingUI = false;
                _uiStateManager.UpdateInitialValues();
                return;
            }

            // valid
            grpDataEntry.SetControlsEnabled(true, excludeControls);
            grpCreateNewImgTable.SetControlsEnabled(true);

            btnImportDataEntry.Enabled = true;
            btnExportDataEntry.Enabled = true;

            var currentEntry = _originalDataEntries[idx];
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
            txtDataEntryAddr.Text = entryAddr.ToString("X8");

            // frame sprite
            LoadSpriteFrames();

            _isUpdatingUI = false;
            _uiStateManager.UpdateInitialValues();
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

        private void LoadSpriteFrames()
        {
            _loadedSpriteFrames.Clear();
            _loadedSpriteAddresses.Clear();
            picSpritePreviewFrame.Image = null;

            bool isImgTableValid = !string.IsNullOrWhiteSpace(txtDataImgTableAddr.Text) &&
                                   !txtDataImgTableAddr.Text.Equals("null");

            grpSpritePreview.SetControlsEnabled(isImgTableValid);

            if (!isImgTableValid)
            {
                grpSpritePreview.ResetControls();
                _isUpdatingUI = true;
                nudSpriteFrameCount.Maximum = 0;
                nudSpriteFrameMaxCount.Value = 0;
                _isUpdatingUI = false;
                return;
            }

            if (!ControlHelper.TryParseAddress(txtDataImgTableAddr.Text, out uint imgTableOffset)) return;
            var selectedSize = cmbDataSize.SelectedItem as DataSizeComboItem;
            if (selectedSize == null) return;

            int expectedVramSize = selectedSize.VramSize;
            int expectedWidth = selectedSize.Width;
            int expectedHeight = selectedSize.Height;
            Color[] currentPalette = GetCurrentPalette(cmbDataPalIdx1);

            // is reserved?
            var reservedImgTable = _reservationManager.GetReservation(txtDataImgTableAddr);

            if (reservedImgTable != null && reservedImgTable.Address == imgTableOffset)
            {
                byte[] tempTableData = reservedImgTable.Data;
                int calculatedFrameCount = tempTableData.Length / (8 + expectedVramSize);

                for (int i = 0; i < calculatedFrameCount; i++)
                {
                    uint ptr = BitConverter.ToUInt32(tempTableData, i * 8);
                    ushort size = BitConverter.ToUInt16(tempTableData, i * 8 + 4);

                    if (size != expectedVramSize) break;

                    uint imgOffset = ptr - GbaConstants.BaseAddr;
                    int relativeOffset = (int)(imgOffset - reservedImgTable.Address);

                    byte[] imageData = new byte[expectedVramSize];
                    Array.Copy(tempTableData, relativeOffset, imageData, 0, expectedVramSize);

                    Bitmap bmp = ImageManager.CreateSprite(
                        imageData,
                        currentPalette,
                        expectedWidth,
                        expectedHeight,
                        true);
                    Bitmap scaledBmp = ImageManager.ScalePixelArt(bmp, 2);

                    _loadedSpriteFrames.Add(scaledBmp);
                    _loadedSpriteAddresses.Add(imgOffset);
                }
            }
            else
            {
                var pointerTargets = new HashSet<uint>();
                foreach (var entry in _workingDataEntries)
                {
                    if (entry.pDataImgTableAddr >= GbaConstants.BaseAddr)
                    {
                        pointerTargets.Add(entry.pDataImgTableAddr - GbaConstants.BaseAddr);
                    }
                }

                var tableEntries = _tableParsingHelper.ParsePointerEntries(
                    (int)imgTableOffset,
                    "PP PP PP PP sX sX 00 00",
                    null,
                    pointerTargets);

                foreach (var entry in tableEntries)
                {
                    if (entry.ParamX != expectedVramSize)
                    {
                        break;
                    }

                    byte[] imageData = new byte[expectedVramSize];

                    // temporary image data
                    if (_temporarySpriteFrames.TryGetValue(entry.TargetOffset, out byte[] tempData))
                    {
                        Array.Copy(tempData, imageData, expectedVramSize);
                    }
                    else
                    {
                        Array.Copy(_romData, entry.TargetOffset, imageData, 0, expectedVramSize);
                    }

                    Bitmap bmp = ImageManager.CreateSprite(
                        imageData,
                        currentPalette,
                        expectedWidth,
                        expectedHeight,
                        true);
                    Bitmap scaledBmp = ImageManager.ScalePixelArt(bmp, 2);

                    _loadedSpriteFrames.Add(scaledBmp);
                    _loadedSpriteAddresses.Add(entry.TargetOffset);
                }
            }

            _isUpdatingUI = true;
            nudSpriteFrameCount.Maximum = _loadedSpriteFrames.Count > 0 ? _loadedSpriteFrames.Count - 1 : 0;
            nudSpriteFrameMaxCount.Value = _loadedSpriteFrames.Count;
            _isUpdatingUI = false;

            UpdateSpritePreview();
        }

        private Color[] GetCurrentPalette(ComboBox cmb)
        {
            var selectedPal = cmb.SelectedItem as PaletteComboItem;
            if (selectedPal == null) return new Color[GbaConstants.PalColorCount];

            // temporary
            if (selectedPal.IsTemporary && selectedPal.TemporaryData != null)
            {
                return ImageManager.DecompressPalette(selectedPal.TemporaryData, 0, false);
            }

            if (selectedPal.PalAddr == 0)
            {
                return new Color[GbaConstants.PalColorCount];
            }

            return ImageManager.DecompressPalette(_romData, selectedPal.PalAddr, false);
        }

        private void UpdateSpritePreview()
        {
            if (_loadedSpriteFrames.Count == 0) return;
            int frameIdx = (int)nudSpriteFrameCount.Value;
            picSpritePreviewFrame.Image = _loadedSpriteFrames[frameIdx];
            txtSpriteFrameAddr.Text = _loadedSpriteAddresses[frameIdx].ToString("X8");
        }

        private void nudSpriteFrameCount_ValueChanged(object sender, EventArgs e)
        {
            if (_isUpdatingUI) return;
            UpdateSpritePreview();
        }

        private void cmbDataPalIdx1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_isUpdatingUI) return;
            LoadSpriteFrames();
        }

        private void UpdatePalPreview()
        {
            var item = cmbPalIdx.SelectedItem as PaletteComboItem;

            if (item == null)
            {
                picPalPreview.Image = null;
                txtPalAddr.Text = string.Empty;
                return;
            }

            uint palAddr = item.PalAddr;
            txtPalAddr.Text = palAddr == 0
                ? "null"
                : palAddr.ToString("X8");

            Color[] colors = null;

            if (item.IsTemporary && item.TemporaryData != null)
            {
                colors = ImageManager.DecompressPalette(item.TemporaryData, 0, false);
            }
            else if (palAddr != 0)
            {
                try
                {
                    colors = ImageManager.DecompressPalette(_romData, palAddr, false);
                }
                catch
                {
                    //
                }
            }

            if (colors == null)
            {
                picPalPreview.Image = null;
                return;
            }

            int boxSize = 10;
            Bitmap bmp = new Bitmap(picPalPreview.Width, picPalPreview.Height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                for (int i = 0; i < GbaConstants.PalColorCount; i++)
                {
                    int x = (i % (GbaConstants.PalColorCount / 2)) * boxSize;
                    int y = (i / (GbaConstants.PalColorCount / 2)) * boxSize;
                    using (var b = new SolidBrush(colors[i]))
                    {
                        g.FillRectangle(b, x, y, boxSize, boxSize);
                    }
                }
            }

            picPalPreview.Image?.Dispose();
            picPalPreview.Image = bmp;
            picPalPreview.Refresh();
        }

        private void cmbPalIdx_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_isUpdatingUI) return;
            UpdatePalPreview();
        }

        private void btnCreateNewPalIdx_Click(object sender, EventArgs e)
        {
            DiscardTemporaryPalette();
            if (!ControlHelper.ValidateAndFormatInputTextBox(txtCreateNewPalAddr, out uint? palAddr)) return;

            if (!ushort.TryParse(
                txtCreateNewPalIdx.Text,
                System.Globalization.NumberStyles.HexNumber,
                null,
                out ushort targetPalIdx) ||
                targetPalIdx < 0x1100 || targetPalIdx > 0x11FF)
            {
                MessageBox.Show(
                    "パレットIDは0x1100 から0x11FFの範囲である必要があります。",
                    "",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            if (_palManager.Working.Any(x => x._Idx == targetPalIdx))
            {
                MessageBox.Show(
                    "指定されたパレットIDは既に存在します。",
                    "",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = GbaConstants.ImageImportFilter;
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    using (Bitmap bmp = new Bitmap(ofd.FileName))
                    {
                        if (bmp.PixelFormat != PixelFormat.Format4bppIndexed)
                        {
                            MessageBox.Show(
                                "4bpp(16色)のインデックスカラー画像を使用してください。",
                                "",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                            return;
                        }

                        Color[] colors = new Color[GbaConstants.PalColorCount];
                        var pal = bmp.Palette;
                        for (int i = 0; i < GbaConstants.PalColorCount; i++)
                        {
                            if (i < pal.Entries.Length)
                                colors[i] = pal.Entries[i];
                            else
                                colors[i] = Color.Black;
                        }

                        byte[] palData = ImageManager.CompressPalette(colors, false);

                        _temporaryPalette.PalIdx = targetPalIdx;
                        _temporaryPalette.TableIdx = -1;
                        _temporaryPalette.IsTemporary = true;
                        _temporaryPalette.TemporaryData = palData;
                        _temporaryPalette.PalAddr = (uint)palAddr;

                        _paletteComboSource.Add(_temporaryPalette);
                        cmbPalIdx.SelectedItem = _temporaryPalette;

                        _uiStateManager.UpdateBinary(_temporaryPalette, palData);
                        _reservationManager.SetReservation(txtCreateNewPalAddr, palAddr.Value, palData);
                    }
                }
            }
        }

        private void btnImportDataEntry_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = GbaConstants.BinImportExportFilter;
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    byte[] buffer = File.ReadAllBytes(ofd.FileName);
                    var entryManager = new EntryManager<OverworldDataEntry>(_romData, _tblReader);
                    int expectedSize = entryManager.GetEntrySize();

                    var importedEntries = IoHelper.ReadStructures<OverworldDataEntry>(buffer, 0, 1, _tblReader);
                    if (importedEntries.Count > 0)
                    {
                        var importedEntry = importedEntries[0];

                        _isUpdatingUI = true;

                        DataBindingHelper.BindObjectToControls(this, importedEntry);

                        LoadToPalComboBox(cmbDataPalIdx1, importedEntry._PalIdx1);
                        LoadToPalComboBox(cmbDataPalIdx2, importedEntry._PalIdx2);

                        LoadToSizeComboBox(importedEntry._ImgWidth, importedEntry._ImgHeight);

                        byte paletteSlotAndUnknown = importedEntry._PalSlotAndUnknownFlags;
                        nudDataPalSlot.Value = paletteSlotAndUnknown & GbaConstants.NibbleMask;
                        chkUnknownFlag1.Checked = (paletteSlotAndUnknown & GbaConstants.OverworldSpriteUnknownFlag1Mask) != 0;
                        chkUnknownFlag2.Checked = (paletteSlotAndUnknown & GbaConstants.OverworldSpriteUnknownFlag2Mask) != 0;
                        chkUnknownFlag3.Checked = (paletteSlotAndUnknown & GbaConstants.OverworldSpriteUnknownFlag3Mask) != 0;

                        _isUpdatingUI = false;

                        LoadSpriteFrames();
                    }
                }
            }
        }

        private void btnExportDataEntry_Click(object sender, EventArgs e)
        {
            var currentEntry = CloneHelper.Clone(_workingDataEntries[_currentDataEntryIdx]);
            DataBindingHelper.BindControlsToObject(this, currentEntry);

            var pal1Item = cmbDataPalIdx1.SelectedItem as PaletteComboItem;
            var pal2Item = cmbDataPalIdx2.SelectedItem as PaletteComboItem;
            currentEntry._PalIdx1 = pal1Item?.PalIdx ?? 0;
            currentEntry._PalIdx2 = pal2Item?.PalIdx ?? 0;

            var sizeItem = cmbDataSize.SelectedItem as DataSizeComboItem;
            if (sizeItem != null)
            {
                currentEntry._ImgWidth = (ushort)sizeItem.Width;
                currentEntry._ImgHeight = (ushort)sizeItem.Height;
            }

            byte palSlot = (byte)((int)nudDataPalSlot.Value & GbaConstants.NibbleMask);
            byte unknownFlags = 0;
            if (chkUnknownFlag1.Checked) unknownFlags |= GbaConstants.OverworldSpriteUnknownFlag1Mask;
            if (chkUnknownFlag2.Checked) unknownFlags |= GbaConstants.OverworldSpriteUnknownFlag2Mask;
            if (chkUnknownFlag3.Checked) unknownFlags |= GbaConstants.OverworldSpriteUnknownFlag3Mask;
            currentEntry._PalSlotAndUnknownFlags = (byte)(palSlot | unknownFlags);

            currentEntry._Padding1 = 0xFFFF;
            currentEntry._Padding2 = 0x00;

            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = GbaConstants.BinImportExportFilter;
                sfd.FileName = $"overworld_{_currentDataEntryIdx:D4}.bin";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    var entryManager = new EntryManager<OverworldDataEntry>(_romData, _tblReader);
                    int size = entryManager.GetEntrySize();
                    byte[] buffer = new byte[size];

                    IoHelper.WriteStructures(
                        buffer,
                        0,
                        new List<OverworldDataEntry> { currentEntry },
                        _tblReader,
                        null,
                        false);

                    File.WriteAllBytes(sfd.FileName, buffer);
                }
            }
        }

        private void btnCreateNewDataEntry_Click(object sender, EventArgs e)
        {
            if (!ControlHelper.ValidateAndFormatInputTextBox(txtCreateNewDataEntryAddr, out uint? addr)) return;

            // get size
            var entryManager = new EntryManager<OverworldDataEntry>(_romData, _tblReader);
            byte[] entryData = new byte[entryManager.GetEntrySize()];

            _uiStateManager.UpdateBinary(txtDataEntryAddr, entryData);
            _reservationManager.SetReservation(txtDataEntryAddr, addr.Value, entryData);

            _isUpdatingUI = true;

            var excludeControls = new[] { "cmbDataSize" };
            grpDataEntry.SetControlsEnabled(true, excludeControls);
            grpDataEntry.ResetControls(excludeControls);

            grpCreateNewImgTable.SetControlsEnabled(true);
            grpCreateNewImgTable.ResetControls();

            if (cmbDataPalIdx1.Items.Count > 0) cmbDataPalIdx1.SelectedIndex = 0;
            if (cmbDataPalIdx2.Items.Count > 0) cmbDataPalIdx2.SelectedIndex = 0;
            if (cmbDataFootprint.Items.Count > 0) cmbDataFootprint.SelectedIndex = 0;
            if (cmbFontIdx.Items.Count > 0) cmbFontIdx.SelectedIndex = 0;
            if (cmbDataSize.Items.Count > 0) cmbDataSize.SelectedIndex = 0;

            txtDataLoadAddr.Text = "null";
            txtDataSizeAddr.Text = "null";
            txtDataAnimAddr.Text = "null";
            txtDataImgTableAddr.Text = "null";
            txtDataMemoryAddr.Text = "null";

            btnImportDataEntry.Enabled = true;
            btnExportDataEntry.Enabled = true;

            _isUpdatingUI = false;

            LoadSpriteFrames();
        }

        private void btnCreateNewImgTable_Click(object sender, EventArgs e)
        {
            if (!ControlHelper.ValidateAndFormatInputTextBox(txtCreateNewImgTableAddr, out uint? addr)) return;

            var sizeItem = cmbCreateNewImgTableSize.SelectedItem as DataSizeComboItem;
            int frameCount = (int)nudCreateNewImgTableCount.Value;
            var imgEntryManager = new EntryManager<OverworldSpriteImageEntry>(_romData, _tblReader);
            int entrySize = imgEntryManager.GetEntrySize();

            // calc size
            int vramSize = sizeItem.VramSize;
            int pointerTableSize = frameCount * entrySize; 
            int totalSize = pointerTableSize + (frameCount * vramSize);

            byte[] combinedData = new byte[totalSize];
            uint currentImageOffset = addr.Value + (uint)pointerTableSize;

            for (int i = 0; i < frameCount; i++)
            {
                uint gbaPointer = currentImageOffset + GbaConstants.BaseAddr;
                int currentEntryOffset = i * entrySize;

                Array.Copy(BitConverter.GetBytes(gbaPointer), 0, combinedData, currentEntryOffset, GbaConstants.PtrSize);
                Array.Copy(BitConverter.GetBytes((ushort)vramSize), 0, combinedData, currentEntryOffset + GbaConstants.PtrSize, sizeof(ushort));

                currentImageOffset += (uint)vramSize;
            }

            _reservationManager.SetReservation(txtDataImgTableAddr, addr.Value, combinedData);

            for (int i = 0; i < cmbDataSize.Items.Count; i++)
            {
                var item = cmbDataSize.Items[i] as DataSizeComboItem;
                if (item != null && item.Key == sizeItem.Key)
                {
                    cmbDataSize.SelectedIndex = i;
                    break;
                }
            }

            _temporarySpriteFrames.Clear();
            _uiStateManager.UpdateBinary("ImportedSprites", null);

            LoadSpriteFrames();
        }

        private void btnImportSpriteFrames_Click(object sender, EventArgs e)
        {
            if (!(cmbDataSize.SelectedItem is DataSizeComboItem sizeItem)) return;

            int expectedWidth = sizeItem.Width;
            int expectedHeight = sizeItem.Height;
            int frameCount = (int)nudSpriteFrameMaxCount.Value;
            if (frameCount <= 0) return;

            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = GbaConstants.ImageImportFilter;

                if (ofd.ShowDialog() != DialogResult.OK) return;

                using (Bitmap fullBmp = new Bitmap(ofd.FileName))
                {
                    List<byte[]> framesData = new List<byte[]>();

                    try
                    {
                        for (int i = 0; i < frameCount; i++)
                        {
                            Rectangle rect = new Rectangle(i * expectedWidth, 0, expectedWidth, expectedHeight);
                            using (Bitmap frameBmp = fullBmp.Clone(rect, fullBmp.PixelFormat))
                            {
                                if (ImageManager.ExtractImageAndPalette(frameBmp, expectedWidth, expectedHeight, out byte[] frameData, out _))
                                {
                                    framesData.Add(frameData);
                                }
                                else
                                {
                                    return;
                                }
                            }
                        }
                    }
                    catch
                    {
                        return;
                    }

                    int vramSize = sizeItem.VramSize;
                    var reservedImgTable = _reservationManager.GetReservation(txtDataImgTableAddr);

                    if (reservedImgTable != null)
                    {
                        var imgEntryManager = new EntryManager<OverworldSpriteImageEntry>(_romData, _tblReader);
                        int headerSize = frameCount * imgEntryManager.GetEntrySize();

                        for (int i = 0; i < frameCount; i++)
                        {
                            int offset = headerSize + (i * vramSize);
                            Array.Copy(framesData[i], 0, reservedImgTable.Data, offset, vramSize);
                        }

                        _reservationManager.SetReservation(txtDataImgTableAddr, reservedImgTable.Address, reservedImgTable.Data);
                    }
                    else
                    {
                        for (int i = 0; i < frameCount; i++)
                        {
                            if (i < _loadedSpriteAddresses.Count)
                            {
                                _temporarySpriteFrames[_loadedSpriteAddresses[i]] = framesData[i];
                            }
                        }

                        _uiStateManager.UpdateBinary("ImportedSprites", new byte[] { 1 });
                    }

                    LoadSpriteFrames();
                }
            }
        }

        private void btnExportSpriteFrames_Click(object sender, EventArgs e)
        {
            if (!(cmbDataSize.SelectedItem is DataSizeComboItem sizeItem)) return;

            int expectedWidth = sizeItem.Width;
            int expectedHeight = sizeItem.Height;
            int frameCount = (int)nudSpriteFrameMaxCount.Value;
            if (frameCount <= 0) return;

            int vramSize = sizeItem.VramSize;
            List<byte[]> framesData = new List<byte[]>();
            var reservedImgTable = _reservationManager.GetReservation(txtDataImgTableAddr);

            if (reservedImgTable != null)
            {
                var imgEntryManager = new EntryManager<OverworldSpriteImageEntry>(_romData, _tblReader);
                int headerSize = frameCount * imgEntryManager.GetEntrySize();

                for (int i = 0; i < frameCount; i++)
                {
                    byte[] frameData = new byte[vramSize];
                    Array.Copy(reservedImgTable.Data, headerSize + (i * vramSize), frameData, 0, vramSize);
                    framesData.Add(frameData);
                }
            }
            else
            {
                for (int i = 0; i < frameCount; i++)
                {
                    if (i >= _loadedSpriteAddresses.Count) continue;

                    uint addr = _loadedSpriteAddresses[i];
                    byte[] frameData = new byte[vramSize];

                    if (_temporarySpriteFrames.TryGetValue(addr, out byte[] tempData))
                    {
                        Array.Copy(tempData, frameData, vramSize);
                    }
                    else
                    {
                        Array.Copy(_romData, addr, frameData, 0, vramSize);
                    }
                    framesData.Add(frameData);
                }
            }

            if (framesData.Count != frameCount) return;

            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = GbaConstants.ImageExportFilter;
                sfd.FileName = $"overworld_sprite_{_currentDataEntryIdx:D4}.png";

                if (sfd.ShowDialog() != DialogResult.OK) return;

                int totalWidth = expectedWidth * frameCount;
                Color[] currentPalette = GetCurrentPalette(cmbDataPalIdx1);

                using (Bitmap exportBmp = new Bitmap(totalWidth, expectedHeight, PixelFormat.Format4bppIndexed))
                {
                    ColorPalette pal = exportBmp.Palette;
                    int palCount = Math.Min(currentPalette.Length, GbaConstants.PalColorCount);
                    for (int i = 0; i < palCount; i++)
                    {
                        pal.Entries[i] = currentPalette[i];
                    }
                    exportBmp.Palette = pal;

                    BitmapData bmpData = exportBmp.LockBits(
                        new Rectangle(0, 0, totalWidth, expectedHeight),
                        ImageLockMode.WriteOnly,
                        PixelFormat.Format4bppIndexed);

                    byte[] pixels = new byte[bmpData.Stride * expectedHeight];

                    for (int f = 0; f < frameCount; f++)
                    {
                        byte[] frameData = framesData[f];
                        int frameOffsetX = f * expectedWidth;
                        int dataIndex = 0;

                        for (int yTile = 0; yTile < expectedHeight; yTile += GbaConstants.TileSize)
                        {
                            for (int xTile = 0; xTile < expectedWidth; xTile += GbaConstants.TileSize)
                            {
                                for (int yPixel = 0; yPixel < GbaConstants.TileSize; yPixel++)
                                {
                                    for (int xPixel = 0; xPixel < GbaConstants.TileSize; xPixel += GbaConstants.PixelsPerByte4Bpp)
                                    {
                                        if (dataIndex >= frameData.Length) break;

                                        byte temp = frameData[dataIndex++];
                                        int leftIndex = temp & GbaConstants.NibbleMask;
                                        int rightIndex = (temp >> GbaConstants.NibbleShift) & GbaConstants.NibbleMask;

                                        int byteIndex = (yTile + yPixel) * bmpData.Stride + ((frameOffsetX + xTile + xPixel) / GbaConstants.PixelsPerByte4Bpp);
                                        pixels[byteIndex] = (byte)((leftIndex << GbaConstants.Bpp4) | rightIndex);
                                    }
                                }
                            }
                        }
                    }

                    System.Runtime.InteropServices.Marshal.Copy(pixels, 0, bmpData.Scan0, pixels.Length);
                    exportBmp.UnlockBits(bmpData);

                    ImageManager.ExportIndexedImage(exportBmp, sfd.FileName);
                }
            }
        }

        private void nudDataTableIdx_ValueChanged(object sender, EventArgs e)
        {
            if (_isUpdatingUI) return;

            if (btnSave.Enabled)
            {
                ControlHelper.HandleUnsavedChanges(
                    () =>
                    {
                        SaveCurrentData(_currentDataEntryIdx);
                        ChangeDataTable();
                    },
                    () =>
                    {
                        DiscardTemporaryPalette();
                        ChangeDataTable();
                    },
                    () =>
                    {
                        _isUpdatingUI = true;
                        nudDataTableIdx.Value = _currentDataTableIdx;
                        _isUpdatingUI = false;
                    }
                );
            }
            else
            {
                ChangeDataTable();
            }
        }

        private void ChangeDataTable()
        {
            _currentDataTableIdx = (int)nudDataTableIdx.Value;
            nudDataEntryCount.Value = _dataPointers[_currentDataTableIdx].Count;

            BackUpCurrentTableData();
            LoadDataEntryListBox();
            ResetControls();
            LoadDataEntryToUI(0);
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
                        SaveCurrentData(_currentDataEntryIdx);
                        ResetControls();
                        LoadDataEntryToUI(newentryIndex);
                    },
                    () =>
                    {
                        DiscardTemporaryPalette();
                        ResetControls();
                        LoadDataEntryToUI(newentryIndex);
                    },
                    () =>
                    {
                        _isUpdatingUI = true;
                        lstDataEntry.SelectedIndex = _currentDataEntryIdx;
                        _isUpdatingUI = false;
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
            txtCreateNewDataEntryAddr.Text = string.Empty;

            nudSpriteFrameCount.Value = nudSpriteFrameCount.Minimum;

            txtCreateNewPalIdx.Text = string.Empty;
            txtCreateNewPalAddr.Text = string.Empty;

            txtCreateNewImgTableAddr.Text = string.Empty;
            nudCreateNewImgTableCount.Value = nudCreateNewImgTableCount.Minimum;
            if (cmbCreateNewImgTableSize.SelectedIndex > 0)
            {
                cmbCreateNewImgTableSize.SelectedIndex = 0;
            }
        }

        private void SaveCurrentData(int idx)
        {
            uint entryAddr = _dataPointers[_currentDataTableIdx][idx].TargetOffset;

            // new area
            var reservedDataEntry = _reservationManager.GetReservation(txtDataEntryAddr);
            if (reservedDataEntry != null)
            {
                entryAddr = reservedDataEntry.Address;
                uint gbaAddr = entryAddr + GbaConstants.BaseAddr;
                uint ptrOffset = _dataPointers[_currentDataTableIdx][idx].EntryOffset;
                Array.Copy(BitConverter.GetBytes(gbaAddr), 0, _romData, ptrOffset, GbaConstants.PtrSize);

                _dataPointers[_currentDataTableIdx][idx].TargetOffset = entryAddr;
                _reservationManager.ClearReservation(txtDataEntryAddr);
            }

            // new image table
            var reservedImgTable = _reservationManager.GetReservation(txtDataImgTableAddr);
            if (reservedImgTable != null)
            {
                Array.Copy(reservedImgTable.Data, 0, _romData, reservedImgTable.Address, reservedImgTable.Data.Length);
                _reservationManager.ClearReservation(txtDataImgTableAddr);
            }

            // new imported image
            foreach (var kvp in _temporarySpriteFrames)
            {
                Array.Copy(kvp.Value, 0, _romData, kvp.Key, kvp.Value.Length);
            }
            _temporarySpriteFrames.Clear();
            _uiStateManager.UpdateBinary("ImportedSprites", null);

            if (entryAddr == 0) return;

            var currentEntry = _workingDataEntries[idx];
            DataBindingHelper.BindControlsToObject(this, currentEntry);

            // padding
            currentEntry._Padding1 = 0xFFFF;
            currentEntry._Padding2 = 0x00;

            // pal
            var pal1Item = cmbDataPalIdx1.SelectedItem as PaletteComboItem;
            var pal2Item = cmbDataPalIdx2.SelectedItem as PaletteComboItem;
            currentEntry._PalIdx1 = pal1Item?.PalIdx ?? 0;
            currentEntry._PalIdx2 = pal2Item?.PalIdx ?? 0;

            // size
            var sizeItem = cmbDataSize.SelectedItem as DataSizeComboItem;
            if (sizeItem != null)
            {
                currentEntry._ImgWidth = (ushort)sizeItem.Width;
                currentEntry._ImgHeight = (ushort)sizeItem.Height;
            }

            // pal slot and unknown flag
            byte palSlot = (byte)((int)nudDataPalSlot.Value & GbaConstants.NibbleMask);
            byte unknownFlags = 0;
            if (chkUnknownFlag1.Checked) unknownFlags |= GbaConstants.OverworldSpriteUnknownFlag1Mask;
            if (chkUnknownFlag2.Checked) unknownFlags |= GbaConstants.OverworldSpriteUnknownFlag2Mask;
            if (chkUnknownFlag3.Checked) unknownFlags |= GbaConstants.OverworldSpriteUnknownFlag3Mask;
            currentEntry._PalSlotAndUnknownFlags = (byte)(palSlot | unknownFlags);

            // wirte data entry
            var entryManager = new EntryManager<OverworldDataEntry>(_romData, _tblReader);
            entryManager.Load(entryAddr, 1);
            entryManager.Working[0] = currentEntry;
            entryManager.Save(0, false);
            _originalDataEntries[idx] = CloneHelper.Clone(currentEntry);

            // font
            int fontByteIndex = idx / 2;
            if (fontByteIndex < _fontManager.Working.Count)
            {
                byte fontData = _fontManager.Working[fontByteIndex]._Idx;
                int selectedFontId = (int)(cmbFontIdx.SelectedValue ?? 0);

                if (idx % 2 == 0)
                {
                    fontData = (byte)((fontData & (GbaConstants.NibbleMask << 4)) | (selectedFontId & GbaConstants.NibbleMask));
                }
                else
                {
                    fontData = (byte)((fontData & GbaConstants.NibbleMask) | ((selectedFontId & GbaConstants.NibbleMask) << 4));
                }

                _fontManager.Working[fontByteIndex]._Idx = fontData;
                _fontManager.Save(fontByteIndex, false);
            }

            // temporary pal
            HandleTemporaryPaletteSave();
        }

        private void HandleTemporaryPaletteSave()
        {
            // TableIdx = -1
            if (_temporaryPalette.IsTemporary && _temporaryPalette.TableIdx == -1 && _temporaryPalette.TemporaryData != null)
            {
                Array.Copy(_temporaryPalette.TemporaryData, 0, _romData, _temporaryPalette.PalAddr, _temporaryPalette.TemporaryData.Length);

                var newPalEntry = new OverworldPaletteEntry
                {
                    _Idx = _temporaryPalette.PalIdx,
                    pPalAddr = _temporaryPalette.PalAddr + GbaConstants.BaseAddr,
                    _Padding1 = GbaConstants.PaddingByte
                };

                _palManager.Working.Add(newPalEntry);
                _palManager.Original.Add(new OverworldPaletteEntry());
                _palManager.Save(_palManager.Working.Count - 1, false);
                _palManager.Count = _palManager.Working.Count;

                _reservationManager.ClearReservation(txtCreateNewPalAddr);
                ResetTemporaryPalette();

                // reset cmb
                _isUpdatingUI = true;
                InitializePaletteComboBox();
                LoadToPalComboBox(cmbDataPalIdx1, _workingDataEntries[_currentDataEntryIdx]._PalIdx1);
                LoadToPalComboBox(cmbDataPalIdx2, _workingDataEntries[_currentDataEntryIdx]._PalIdx2);
                _isUpdatingUI = false;
            }
        }

        private void DiscardTemporaryPalette()
        {
            if (_temporaryPalette.IsTemporary && _temporaryPalette.TableIdx == -1)
            {
                _paletteComboSource.Remove(_temporaryPalette);
                _reservationManager.ClearReservation(txtCreateNewPalAddr);
                _uiStateManager.UpdateBinary(_temporaryPalette, null);
                ResetTemporaryPalette();
            }
        }

        private void ResetTemporaryPalette()
        {
            _temporaryPalette.TableIdx = null;
            _temporaryPalette.TemporaryData = null;
            _temporaryPalette.PalAddr = 0;
            _temporaryPalette.PalIdx = 0x11FF;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveCurrentData(_currentDataEntryIdx);
            _uiStateManager.UpdateInitialValues();
        }

        private void OverworldSpriteEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (btnSave.Enabled)
            {
                ControlHelper.HandleUnsavedChanges(
                    () =>
                    {
                        SaveCurrentData(_currentDataEntryIdx);
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
    }
}
