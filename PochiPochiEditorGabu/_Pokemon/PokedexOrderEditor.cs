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
    public partial class PokedexOrderEditor : Form
    {
        protected byte[] _romData;
        protected IniFileReader _config;
        protected TblFileReader _tblReader;
        protected ReservationManager _reservationManager;

        private UIStateManager _uiStateManager;

        private EntryManager<PokedexOrderEntry> _orderManager;

        private EntryManager<PokemonNameEntry> _pokemonNameManager;
        private EntryManager<PokemonIconImageEntry> _iconImgManager;
        private EntryManager<PokemonIconPaletteIndexEntry> _iconPalIdxManager;
        private EntryManager<PokemonIconPaletteAddressEntry> _iconPalAddrManager;

        private bool _isUpdatingUI = false;
        private List<PokemonPokedexEntry> _pokedexList = new List<PokemonPokedexEntry>();

        public enum PokedexStatus
        {
            Normal,
            Duplicate,
            OutOfRange
        }

        public class PokemonPokedexEntry
        {
            public int PokemonIndex { get; set; }
            public string Name { get; set; }
            public int PokedexOrder { get; set; }
            public PokedexStatus Status { get; set; } = PokedexStatus.Normal;

            public override string ToString()
            {
                return Name;
            }
        }

        public PokedexOrderEditor(
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

            // order
            _orderManager = EntryManager<PokedexOrderEntry>.Create(
                _romData, _tblReader, _config, "PokedexOrderTableAddress", "PokedexOrderCount");

            // name
            _pokemonNameManager = EntryManager<PokemonNameEntry>.Create(
                _romData, _tblReader, _config, "PokemonNameTableAddress", "PokemonNameCount");

            // icon
            _iconImgManager = EntryManager<PokemonIconImageEntry>.Create(
                _romData, _tblReader, _config, "PokemonIconImageTableAddress", "PokemonIconCount");
            _iconPalIdxManager = EntryManager<PokemonIconPaletteIndexEntry>.Create(
                _romData, _tblReader, _config, "PokemonIconPaletteIndexTableAddress", "PokemonIconCount");
            _iconPalAddrManager = EntryManager<PokemonIconPaletteAddressEntry>.Create(
                _romData, _tblReader, _config, "PokemonIconPaletteAddressTableAddress", "PokemonIconPaletteAddressCount");

            // control
            ControlHelper.AttachExternalBorder(picIcon);
            lstOrder.DrawMode = DrawMode.OwnerDrawFixed;

            btnSave.Enabled = false;
            _uiStateManager = new UIStateManager(hasChanges => btnSave.Enabled = hasChanges);

            // eventhandler
            lstOrder.DrawItem += LstOrder_DrawItem;
            lstOrder.SelectedIndexChanged += LstOrder_SelectedIndexChanged;
            nudOrder.ValueChanged += NudOrder_ValueChanged;
            btnSave.Click += btnSave_Click;
            this.FormClosing += PokedexOrderEditor_FormClosing;

            LoadPokedexOrderData();
        }

        private void LoadPokedexOrderData()
        {
            _pokedexList.Clear();
            lstOrder.Items.Clear();

            for (int i = 0; i < _orderManager.Count; i++)
            {
                //  i=0 -> species=1
                int pokemonIndex = i + 1;
                string name = string.Empty;

                if (pokemonIndex < _pokemonNameManager.Working.Count)
                {
                    name = _pokemonNameManager.Working[pokemonIndex]._PokemonName;
                }

                int order = _orderManager.Working[i]._OrderIdx;

                var entry = new PokemonPokedexEntry
                {
                    PokemonIndex = pokemonIndex,
                    Name = name,
                    PokedexOrder = order
                };

                _pokedexList.Add(entry);
                lstOrder.Items.Add(entry);
            }

            _uiStateManager.AddBinaries((lstOrder, GetOrderBytes()));

            UpdateStatusesAndUnusedList();

            if (lstOrder.Items.Count > 0)
            {
                lstOrder.SelectedIndex = 0;
            }
        }

        private byte[] GetOrderBytes()
        {
            return _orderManager.Working.SelectMany(x => BitConverter.GetBytes(x._OrderIdx)).ToArray();
        }

        private void UpdateStatusesAndUnusedList()
        {
            var orderGroups = _pokedexList.GroupBy(p => p.PokedexOrder)
                                          .ToDictionary(g => g.Key, g => g.Count());

            // status
            foreach (var entry in _pokedexList)
            {
                if (entry.PokedexOrder > _config.GetInt("PokedexDisplayCount"))
                {
                    entry.Status = PokedexStatus.OutOfRange;
                }
                else if (orderGroups.ContainsKey(entry.PokedexOrder) && orderGroups[entry.PokedexOrder] > 1)
                {
                    entry.Status = PokedexStatus.Duplicate;
                }
                else
                {
                    entry.Status = PokedexStatus.Normal;
                }
            }

            // lstUnused
            lstUnused.BeginUpdate();
            lstUnused.Items.Clear();

            for (int i = 1; i <= _orderManager.Count; i++)
            {
                if (!orderGroups.ContainsKey(i))
                {
                    lstUnused.Items.Add(i);
                }
            }

            lstUnused.EndUpdate();
            lstOrder.Invalidate();
        }

        private void LstOrder_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;

            var entry = (PokemonPokedexEntry)lstOrder.Items[e.Index];
            Color backColor;

            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
            {
                backColor = SystemColors.Highlight;
            }
            else
            {
                switch (entry.Status)
                {
                    case PokedexStatus.Duplicate:
                        backColor = Color.LightCoral;
                        break;
                    case PokedexStatus.OutOfRange:
                        backColor = Color.LightYellow;
                        break;
                    default:
                        backColor = e.BackColor;
                        break;
                }
            }

            using (var backBrush = new SolidBrush(backColor))
            {
                e.Graphics.FillRectangle(backBrush, e.Bounds);
            }

            Color foreColor = ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
                                ? SystemColors.HighlightText
                                : e.ForeColor;

            using (var textBrush = new SolidBrush(foreColor))
            {
                float yPos = e.Bounds.Y + (e.Bounds.Height - e.Font.Height) / 2f;

                // name
                e.Graphics.DrawString(entry.Name, e.Font, textBrush, new PointF(e.Bounds.X + 2, yPos));

                // order
                string orderString = entry.PokedexOrder.ToString();
                SizeF orderSize = e.Graphics.MeasureString(orderString, e.Font);
                e.Graphics.DrawString(orderString, e.Font, textBrush, new PointF(e.Bounds.Right - orderSize.Width - 8, yPos));

                // "-"
                string hyphenString = "-";
                int orderAreaWidth = 56;
                e.Graphics.DrawString(hyphenString, e.Font, textBrush, new PointF(e.Bounds.Right - orderAreaWidth, yPos));
            }

            e.DrawFocusRectangle();
        }

        private void LstOrder_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstOrder.SelectedIndex < 0 || _isUpdatingUI) return;

            var entry = (PokemonPokedexEntry)lstOrder.SelectedItem;

            _isUpdatingUI = true;
            nudSpecies.Value = entry.PokemonIndex;
            txtSpeciesHex.Text = entry.PokemonIndex.ToString("X4");
            nudOrder.Value = entry.PokedexOrder;
            _isUpdatingUI = false;

            picIcon.Image?.Dispose();
            picIcon.Image = null;

            var iconBmp = GetPokemonIcon(entry.PokemonIndex, true);
            if (iconBmp != null)
            {
                picIcon.Image = iconBmp;
            }
        }

        private void NudOrder_ValueChanged(object sender, EventArgs e)
        {
            if (_isUpdatingUI || lstOrder.SelectedIndex < 0) return;

            int selectedIndex = lstOrder.SelectedIndex;
            var entry = (PokemonPokedexEntry)lstOrder.Items[selectedIndex];
            int newOrder = (int)nudOrder.Value;

            if (entry.PokedexOrder != newOrder)
            {
                entry.PokedexOrder = newOrder;

                int dataIndex = entry.PokemonIndex - 1;
                if (dataIndex >= 0 && dataIndex < _orderManager.Working.Count)
                {
                    _orderManager.Working[dataIndex]._OrderIdx = checked((ushort)newOrder);
                    _uiStateManager.UpdateBinary(lstOrder, GetOrderBytes());
                }

                UpdateStatusesAndUnusedList();
            }
        }

        private void SavePokedexOrderData()
        {
            for (int i = 0; i < _orderManager.Count; i++)
            {
                _orderManager.Save(i, appendTerminator: false);
            }

            _uiStateManager.UpdateBinary(lstOrder, GetOrderBytes());
            _uiStateManager.UpdateInitialValues();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SavePokedexOrderData();
        }

        private void PokedexOrderEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (btnSave.Enabled)
            {
                _isUpdatingUI = true;

                DialogResult result = ControlHelper.HandleUnsavedChanges(
                    () =>
                    {
                        SavePokedexOrderData();
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

                _isUpdatingUI = false;
            }
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
    }
}