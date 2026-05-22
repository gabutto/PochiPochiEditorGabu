using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace PochiPochiEditorGabu.Managers
{
    public class UIStateManager
    {
        private readonly Action<bool> _stateChangedCallback;
        private readonly Dictionary<Control, object> _initialControlValues = new Dictionary<Control, object>();
        private readonly Dictionary<object, byte[]> _initialBinaryValues = new Dictionary<object, byte[]>();
        private readonly Dictionary<object, byte[]> _currentBinaryValues = new Dictionary<object, byte[]>();
        private readonly List<RadioButtonGroup> _radioGroups = new List<RadioButtonGroup>();

        private class RadioButtonGroup
        {
            public RadioButton[] Buttons { get; set; }
            public RadioButton InitialChecked { get; set; }
        }

        public UIStateManager(Action<bool> stateChangedCallback)
        {
            _stateChangedCallback = stateChangedCallback;
        }

        // 指定したコントロールを登録
        public void AddControls(params Control[] controls)
        {
            foreach (var control in controls)
            {
                if (_initialControlValues.ContainsKey(control)) continue;

                _initialControlValues.Add(control, GetControlValue(control));
                AttachEventHandler(control);
            }
        }

        // 再帰的に対象コンテナ内を一括登録
        public void AddControlsRecursive(params Control[] containers)
        {
            var targetControls = new List<Control>();

            foreach (var container in containers)
            {
                if (IsAllowedContainer(container))
                {
                    FindTargetControls(container, targetControls);
                }
            }

            if (targetControls.Count > 0)
            {
                AddControls(targetControls.ToArray());
            }
        }

        private void FindTargetControls(Control parent, List<Control> targetControls)
        {
            if (parent == null) return;

            foreach (Control child in parent.Controls)
            {
                if (IsTrackedControl(child))
                {
                    targetControls.Add(child);
                }
                else if (IsAllowedContainer(child))
                {
                    FindTargetControls(child, targetControls);
                }
            }
        }

        private static bool IsTrackedControl(Control c)
        {
            return c is NumericUpDown || c is TextBox || c is ComboBox || c is CheckBox;
        }

        private static bool IsAllowedContainer(Control c)
        {
            return c is Panel || c is GroupBox || c is TabControl || c is TabPage;
        }

        // バイナリデータを初期値として登録
        public void AddBinaries(params (object Key, byte[] Data)[] items)
        {
            foreach (var (key, data) in items)
            {
                if (_initialBinaryValues.ContainsKey(key)) continue;

                byte[] dataCopy = data?.ToArray();
                _initialBinaryValues.Add(key, dataCopy);
                _currentBinaryValues.Add(key, dataCopy);
            }
        }

        // 登録済みのバイナリデータを更新
        public void UpdateBinary(object key, byte[] newData)
        {
            if (!_initialBinaryValues.ContainsKey(key)) return;

            _currentBinaryValues[key] = newData?.ToArray();
            EvaluateState();
        }

        // バイナリデータが初期値から変化しているか
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

        // ラジオボタンをグループ単位で登録
        public void AddRadioButtons(params RadioButton[][] groups)
        {
            foreach (var group in groups)
            {
                var radioGroup = new RadioButtonGroup
                {
                    Buttons = group,
                    InitialChecked = group.FirstOrDefault(rb => rb.Checked)
                };

                _radioGroups.Add(radioGroup);

                foreach (var rb in group)
                {
                    rb.CheckedChanged += OnRadioButtonCheckedChanged;
                }
            }
        }

        private void OnRadioButtonCheckedChanged(object sender, EventArgs e)
        {
            EvaluateState();
        }

        // 現在の値を新しい初期値として確定
        public void UpdateInitialValues()
        {
            foreach (var ctrl in _initialControlValues.Keys.ToList())
            {
                _initialControlValues[ctrl] = GetControlValue(ctrl);
            }

            foreach (var key in _initialBinaryValues.Keys.ToList())
            {
                _initialBinaryValues[key] = _currentBinaryValues[key]?.ToArray();
            }

            foreach (var group in _radioGroups)
            {
                group.InitialChecked = group.Buttons.FirstOrDefault(rb => rb.Checked);
            }

            EvaluateState();
        }

        private void EvaluateState()
        {
            bool hasChanges = DetectControlChanges()
                           || DetectBinaryChanges()
                           || DetectRadioChanges();

            _stateChangedCallback.Invoke(hasChanges);
        }

        private bool DetectControlChanges()
        {
            foreach (KeyValuePair<Control, object> kvp in _initialControlValues)
            {
                if (!Equals(GetControlValue(kvp.Key), kvp.Value))
                {
                    return true;
                }
            }
            return false;
        }

        private bool DetectBinaryChanges()
        {
            return _initialBinaryValues.Keys.Any(HasBinaryChanges);
        }


        private bool DetectRadioChanges()
        {
            foreach (RadioButtonGroup group in _radioGroups)
            {
                RadioButton current = group.Buttons.FirstOrDefault(rb => rb.Checked);
                if (!ReferenceEquals(current, group.InitialChecked))
                {
                    return true;
                }
            }
            return false;
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
