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

        private readonly Dictionary<TextBox, ReservedAreaInfo> _reservations
            = new Dictionary<TextBox, ReservedAreaInfo>();

        public void SetReservation(TextBox textBox, uint address, byte[] data)
        {
            ClearReservation(textBox, redraw: false);

            _reservations[textBox] = new ReservedAreaInfo
            {
                Address = address,
                Data = (byte[])data.Clone()
            };

            textBox.Text = address.ToString("X8");
            textBox.BackColor = Color.LightPink;

            textBox.TextChanged -= OnTextChanged;
            textBox.TextChanged += OnTextChanged;
        }

        public void ClearReservation(TextBox textBox, bool redraw = true)
        {
            if (textBox == null) return;
            if (!_reservations.Remove(textBox)) return;

            textBox.TextChanged -= OnTextChanged;

            if (redraw)
                textBox.BackColor = textBox.ReadOnly
                    ? SystemColors.Control
                    : SystemColors.Window;
        }

        public void ClearAllReservations()
        {
            foreach (TextBox textBox in _reservations.Keys.ToArray())
            {
                ClearReservation(textBox, redraw: true);
            }
        }

        public ReservedAreaInfo GetReservation(TextBox textBox)
        {
            return _reservations.TryGetValue(textBox, out ReservedAreaInfo info) 
                ? info 
                : null;
        }

        public IReadOnlyList<ReservedAreaInfo> GetAllReservations()
        {
            return _reservations.Values.ToList();
        }

        private void OnTextChanged(object sender, EventArgs e)
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
