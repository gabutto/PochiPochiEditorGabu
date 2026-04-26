using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace PochiPochiEditorGabu.Managers
{
    public class ReservationManager
    {
        public class ReservedAreaInfo
        {
            public uint Address { get; set; }
            public byte[] Data { get; set; }
        }

        private readonly Dictionary<TextBox, ReservedAreaInfo> _reservations = new Dictionary<TextBox, ReservedAreaInfo>();

        public void SetReservation(TextBox textBox, uint address, byte[] data)
        {
            ClearReservation(textBox, redraw: false);

            _reservations[textBox] = new ReservedAreaInfo
            {
                Address = address,
                Data = data.ToArray()
            };

            textBox.Text = address.ToString("X8");
            textBox.BackColor = Color.LightPink;

            textBox.TextChanged -= TextBox_TextChanged;
            textBox.TextChanged += TextBox_TextChanged;
        }

        public void ClearReservation(TextBox textBox, bool redraw = true)
        {
            if (textBox == null) return;

            if (_reservations.Remove(textBox))
            {
                if (redraw)
                {
                    textBox.BackColor = SystemColors.Window;
                }
                textBox.TextChanged -= TextBox_TextChanged;
            }
        }

        public void ClearAllReservations()
        {
            foreach (var textBox in _reservations.Keys.ToList())
            {
                ClearReservation(textBox, redraw: true);
            }
        }

        public ReservedAreaInfo GetReservation(TextBox textBox) =>
            _reservations.TryGetValue(textBox, out var info) ? info : null;

        public IEnumerable<ReservedAreaInfo> GetAllReservations() =>
            _reservations.Values.ToList();

        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            if (sender is TextBox txt && _reservations.TryGetValue(txt, out var info))
            {
                string reservedText = info.Address.ToString("X8");
                if (!string.Equals(txt.Text.Trim(), reservedText, StringComparison.OrdinalIgnoreCase))
                {
                    ClearReservation(txt, redraw: true);
                }
            }
        }
    }
}
