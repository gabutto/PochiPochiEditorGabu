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
            public byte[] CurrentData { get; set; }
            public byte[] InitialData { get; set; }
        }

        public event Action<bool> ReservationStateChanged;
        private readonly Dictionary<TextBox, ReservedAreaInfo> _reservations = new Dictionary<TextBox, ReservedAreaInfo>();

        public void SetReservation(TextBox textBox, uint address, byte[] data)
        {
            ClearReservation(textBox, redraw: false);

            var info = new ReservedAreaInfo
            {
                Address = address,
                CurrentData = data?.ToArray(),
                InitialData = data?.ToArray()
            };
            _reservations[textBox] = info;

            textBox.Text = address.ToString("X8");
            textBox.BackColor = Color.LightPink;
            textBox.TextChanged -= TextBox_TextChanged;
            textBox.TextChanged += TextBox_TextChanged;

            EvaluateState();
        }

        public void ClearReservation(TextBox textBox, bool redraw = true)
        {
            if (textBox == null) return;

            if (_reservations.Remove(textBox))
            {
                // reset color
                if (redraw) 
                {
                    textBox.BackColor = SystemColors.Window;
                }

                textBox.TextChanged -= TextBox_TextChanged;
                EvaluateState();
            }
        }

        public void ClearAllReservations()
        {
            foreach (var textBox in _reservations.Keys.ToList())
            {
                ClearReservation(textBox, redraw: true);
            }
        }

        public void UpdateReservationData(TextBox textBox, byte[] newData)
        {
            if (_reservations.TryGetValue(textBox, out var info))
            {
                info.CurrentData = newData?.ToArray();
                EvaluateState();
            }
        }

        public bool HasReservationChanges()
        {
            return _reservations.Values.Any(res =>
            {
                if (res.CurrentData == null && res.InitialData == null) return false;
                if (res.CurrentData == null || res.InitialData == null) return true;
                if (res.CurrentData.Length != res.InitialData.Length) return true;
                return !res.CurrentData.SequenceEqual(res.InitialData);
            });
        }

        public void AcceptAllChanges()
        {
            foreach (var res in _reservations.Values)
            {
                res.InitialData = res.CurrentData?.ToArray();
            }

            EvaluateState();
        }

        private void EvaluateState()
        {
            ReservationStateChanged?.Invoke(HasReservationChanges());
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
