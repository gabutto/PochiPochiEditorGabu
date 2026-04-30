using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace PochiPochiEditorGabu.Helpers
{
    public static class ControlHelper
    {
        public static void SetControlsEnabled(
            this Control container,
            bool enabled,
            IEnumerable<string> excludeNames = null,
            IEnumerable<Type> excludeTypes = null)
        {
            var nameSet = excludeNames != null ? new HashSet<string>(excludeNames) : null;
            var typeSet = excludeTypes != null ? new HashSet<Type>(excludeTypes) : null;

            ExecuteRecursive(container, nameSet, typeSet, ctrl => ctrl.Enabled = enabled);
        }

        public static void ResetControls(
            this Control container,
            IEnumerable<string> excludeNames = null,
            IEnumerable<Type> excludeTypes = null)
        {
            var nameSet = excludeNames != null ? new HashSet<string>(excludeNames) : null;
            var typeSet = excludeTypes != null ? new HashSet<Type>(excludeTypes) : null;

            ExecuteRecursive(container, nameSet, typeSet, ctrl =>
            {
                switch (ctrl)
                {
                    case TextBox textBox:
                        textBox.Text = string.Empty;
                        break;
                    case NumericUpDown nud:
                        nud.Value = Math.Max(nud.Minimum, 0);
                        break;
                    case ComboBox comboBox:
                        comboBox.SelectedIndex = -1;
                        break;
                    case CheckBox checkBox:
                        checkBox.Checked = false;
                        break;
                    case RadioButton radioButton:
                        radioButton.Checked = false;
                        break;
                }
            });
        }

        private static void ExecuteRecursive(
            Control container,
            HashSet<string> nameSet,
            HashSet<Type> typeSet,
            Action<Control> action)
        {
            foreach (Control ctrl in container.Controls)
            {
                if ((nameSet?.Contains(ctrl.Name) == true) || (typeSet?.Contains(ctrl.GetType()) == true))
                {
                    continue;
                }

                action(ctrl);

                if (ShouldRecurse(ctrl))
                {
                    ExecuteRecursive(ctrl, nameSet, typeSet, action);
                }
            }
        }

        private static bool ShouldRecurse(Control ctrl)
        {
            return ctrl is Panel || ctrl is GroupBox || ctrl is TabControl || ctrl is TabPage;
        }

        public static bool TryParseAddress(string addrStr, out uint addrValue)
        {
            return uint.TryParse(addrStr, System.Globalization.NumberStyles.HexNumber, null, out addrValue);
        }

        public static bool ValidateAndFormatInputTextBox(TextBox txt, out uint? addrValue, bool showMessage = true)
        {
            string addrStr = txt.Text.Trim();

            // input "null"
            if (addrStr.Equals("null", StringComparison.OrdinalIgnoreCase))
            {
                addrValue = null;
                txt.Text = "null";
                return true;
            }

            if (!TryParseAddress(addrStr, out uint resultValue))
            {
                addrValue = null;

                if (showMessage)
                {
                    MessageBox.Show(
                        "16進数アドレスを入力してください。",
                        "", 
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
                return false;
            }

            addrValue = resultValue;
            txt.Text = resultValue.ToString("X8");
            return true;
        }

        public static void AttachAddressAutoFormat(params TextBox[] textboxes)
        {
            foreach (TextBox textbox in textboxes)
            {
                textbox.Leave += AddressTextBox_Leave;
            }
        }

        private static void AddressTextBox_Leave(object sender, EventArgs e)
        {
            if (!(sender is TextBox txt)) return;
            if (string.IsNullOrWhiteSpace(txt.Text)) return;

            ValidateAndFormatInputTextBox(txt, out _, showMessage: false);
        }

        public static DialogResult HandleUnsavedChanges(Action saveAction, Action proceedAction, Action cancelAction = null)
        {
            DialogResult result = MessageBox.Show(
                "現在の変更が保存されていません。保存しますか？",
                "",
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Question);

            switch (result)
            {
                case DialogResult.Yes:
                    saveAction?.Invoke();
                    proceedAction?.Invoke();
                    break;
                case DialogResult.No:
                    proceedAction?.Invoke();
                    break;
                case DialogResult.Cancel:
                    cancelAction?.Invoke();
                    break;
            }

            return result;
        }

        public static void AttachExternalBorder(params Control[] targets)
        {
            foreach (var target in targets)
            {
                var parent = target.Parent;
                if (parent != null)
                {
                    parent.Paint += (sender, e) =>
                    {
                        using (var p = new Pen(Color.Gray, 1))
                        {
                            var rect = new Rectangle(
                                target.Left - 1,
                                target.Top - 1,
                                target.Width + 1,
                                target.Height + 1);
                            e.Graphics.DrawRectangle(p, rect);
                        }
                    };
                }
            }
        }

        public static void AttachNumericUpDownNavigators(NumericUpDown nud, Button btnPrev, Button btnNext)
        {
            void UpdateButtons()
            {
                if (btnPrev != null)
                {
                    bool canGoPrev = nud.Value > nud.Minimum;
                    if (!canGoPrev && btnPrev.Focused)
                    {
                        nud.Focus();
                    }
                    btnPrev.Enabled = canGoPrev;
                }

                if (btnNext != null)
                {
                    bool canGoNext = nud.Value < nud.Maximum;
                    if (!canGoNext && btnNext.Focused)
                    {
                        nud.Focus();
                    }
                    btnNext.Enabled = canGoNext;
                }
            }

            if (btnPrev != null)
            {
                btnPrev.Click += (sender, e) =>
                {
                    if (nud.Value > nud.Minimum)
                    {
                        nud.Value--;
                    }
                };
            }

            if (btnNext != null)
            {
                btnNext.Click += (sender, e) =>
                {
                    if (nud.Value < nud.Maximum)
                    {
                        nud.Value++;
                    }
                };
            }

            nud.ValueChanged += (sender, e) => UpdateButtons();
            UpdateButtons();
        }

        public static void AttachRadioButtonToTextBoxFocus(RadioButton rb, TextBox txt)
        {
            txt.Enter += (sender, e) => rb.Checked = true;
        }

        public static void SetupComboBoxItems(ComboBox cmb, int defaultIndex, params string[] items)
        {
            cmb.BeginUpdate();
            try
            {
                cmb.Items.Clear();
                cmb.Items.AddRange(items);
                cmb.SelectedIndex = defaultIndex;
            }
            finally
            {
                cmb.EndUpdate();
            }
        }

        public static void LoadComboBoxFromTextFile(ComboBox comboBox, string filePath)
        {
            var entries = File.ReadLines(filePath)
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .Select(line =>
                {
                    int closeBracketIndex = line.IndexOf(']');
                    if (line.StartsWith("[") && closeBracketIndex > 1)
                    {
                        string hex = line.Substring(1, closeBracketIndex - 1);
                        if (int.TryParse(hex, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out int index))
                        {
                        return new KeyValuePair<int, string>(index, line.Trim());
                        }
                    }
                    return (KeyValuePair<int, string>?)null;
                })
                .Where(entry => entry.HasValue)
                .Select(entry => entry.Value)
                .ToList();

            comboBox.DisplayMember = "Value";
            comboBox.ValueMember = "Key";
            comboBox.DataSource = entries;
        }
    }
}
