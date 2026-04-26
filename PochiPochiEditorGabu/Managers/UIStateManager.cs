using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace PochiPochiEditorGabu.Managers
{
    public class UIStateManager
    {
        private readonly Action<bool> _stateChangedCallback;

        private readonly Dictionary<Control, object> _initialValues = new Dictionary<Control, object>();
        private readonly Dictionary<object, byte[]> _initialBinaryValues = new Dictionary<object, byte[]>();
        private readonly Dictionary<object, byte[]> _currentBinaryValues = new Dictionary<object, byte[]>();

        public UIStateManager(Action<bool> stateChangedCallback)
        {
            _stateChangedCallback = stateChangedCallback;
        }

        public void AddControls(params Control[] controls)
        {
            foreach (var control in controls)
            {
                if (_initialValues.ContainsKey(control)) continue;

                _initialValues.Add(control, GetControlValue(control));
                AttachEventHandler(control);
            }
        }

        public void AddBinaries(params (object Key, byte[] Data)[] items)
        {
            foreach (var (key, data) in items)
            {
                if (_initialBinaryValues.ContainsKey(key)) continue;

                byte[] dataCopy = data.ToArray();
                _initialBinaryValues.Add(key, dataCopy);
                _currentBinaryValues.Add(key, dataCopy);
            }
        }

        public void UpdateBinary(object key, byte[] newData)
        {
            if (!_initialBinaryValues.ContainsKey(key)) return;

            _currentBinaryValues[key] = newData.ToArray();
            EvaluateState();
        }

        public bool HasBinaryChanges(object key)
        {
            if (!_initialBinaryValues.TryGetValue(key, out var init) ||
                !_currentBinaryValues.TryGetValue(key, out var curr))
            {
                return false;
            }

            if (init == null && curr == null) return false;
            if (init == null || curr == null) return true;
            if (init.Length != curr.Length) return true;

            return !init.SequenceEqual(curr);
        }

        public void UpdateInitialValues()
        {
            foreach (var ctrl in _initialValues.Keys.ToList())
            {
                _initialValues[ctrl] = GetControlValue(ctrl);
            }

            foreach (var key in _initialBinaryValues.Keys.ToList())
            {
                _initialBinaryValues[key] = _currentBinaryValues[key].ToArray();
            }

            EvaluateState();
        }

        private void EvaluateState()
        {
            bool hasChanges = false;

            foreach (var kvp in _initialValues)
            {
                object currentValue = GetControlValue(kvp.Key);
                if (!Equals(currentValue, kvp.Value))
                {
                    hasChanges = true;
                    break;
                }
            }

            if (!hasChanges)
            {
                hasChanges = _initialBinaryValues.Keys.Any(HasBinaryChanges);
            }

            _stateChangedCallback?.Invoke(hasChanges);
        }

        private static object GetControlValue(Control control)
        {
            switch (control)
            {
                case NumericUpDown nud:
                    return nud.Value;
                case TextBox txt:
                    return txt.Text;
                case ComboBox cmb:
                    return (!string.IsNullOrEmpty(cmb.ValueMember) && cmb.SelectedValue != null)
                        ? cmb.SelectedValue
                        : cmb.SelectedIndex;
                case CheckBox chk:
                    return chk.Checked;
                default:
                    return null;
            }
        }

        private void AttachEventHandler(Control control)
        {
            switch (control)
            {
                case NumericUpDown nud:
                    nud.ValueChanged += (s, e) => EvaluateState();
                    break;
                case TextBox txt:
                    txt.TextChanged += (s, e) => EvaluateState();
                    break;
                case ComboBox cmb:
                    cmb.SelectedIndexChanged += (s, e) => EvaluateState();
                    break;
                case CheckBox chk:
                    chk.CheckedChanged += (s, e) => EvaluateState();
                    break;
            }
        }
    }
}
