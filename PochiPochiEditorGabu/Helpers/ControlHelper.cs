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
        private static readonly HashSet<Type> ContainerTypes = new HashSet<Type>
        {
            typeof(Panel),
            typeof(GroupBox),
            typeof(TabControl),
            typeof(TabPage),
        };

        /// <summary>
        /// 再帰的コントロールの有効化・無効化（コンテナ系対象外）
        ///
        /// var excludeNames = new[] { "btnTest1", "btnTest2" };
        /// var excludeTypes = new[] { typeof(TextBox), typeof(ComboBox) };
        /// </summary>
        public static void SetControlsEnabled(
            this Control container,
            bool enabled,
            IEnumerable<string> excludeNames = null,
            IEnumerable<Type> excludeTypes = null)
        {
            ExecuteRecursive(
                container,
                ToHashSet(excludeNames),
                ToHashSet(excludeTypes),
                ctrl => ctrl.Enabled = enabled);
        }

        /// <summary>
        /// 再帰的コントロールの初期化（コンテナ系対象外）
        /// </summary>
        public static void ResetControls(
            this Control container,
            IEnumerable<string> excludeNames = null,
            IEnumerable<Type> excludeTypes = null)
        {
            ExecuteRecursive(
                container,
                ToHashSet(excludeNames),
                ToHashSet(excludeTypes),
                ResetControl);
        }

        private static void ResetControl(Control ctrl)
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
        }

        private static void ExecuteRecursive(
            Control container,
            HashSet<string> nameSet,
            HashSet<Type> typeSet,
            Action<Control> action)
        {
            foreach (Control ctrl in container.Controls)
            {
                bool isContainer = ContainerTypes.Contains(ctrl.GetType());

                if (isContainer)
                {
                    ExecuteRecursive(ctrl, nameSet, typeSet, action);
                    continue;
                }

                bool isExcluded = (nameSet != null && nameSet.Contains(ctrl.Name)) ||
                                  (typeSet != null && typeSet.Contains(ctrl.GetType()));
                if (!isExcluded)
                {
                    action(ctrl);
                }
            }
        }

        private static HashSet<T> ToHashSet<T>(IEnumerable<T> source)
        { 
            return source != null 
                ? new HashSet<T>(source) 
                : null;
        }

        /// <summary>
        /// 16進数文字列を変換・真偽
        ///
        /// false の場合 0 が返るので注意
        /// ValidateAndFormatInputTextBox の形式と矛盾
        /// </summary>
        public static bool TryParseAddress(string addrStr, out uint addrValue)
        {
            return uint.TryParse(
                addrStr,
                NumberStyles.HexNumber,
                CultureInfo.InvariantCulture,
                out addrValue);
        }

        /// <summary>
        /// テキストボックス内のアドレスを判定・整形
        ///
        /// showMessage: true でメッセージ表示
        /// 空白・"null"・無効な文字列は null に収束
        /// </summary>
        public static bool ValidateAndFormatInputTextBox(
            TextBox textbox,
            out uint? addrValue,
            bool showMessage = true)
        {
            string addrStr = textbox.Text.Trim();

            // 1：空白の場合
            if (string.IsNullOrWhiteSpace(addrStr))
            {
                addrValue = null;
                textbox.Text = string.Empty;
                return true;
            }

            // 2：文字列 "null" の場合
            if (string.Equals(addrStr, "null", StringComparison.OrdinalIgnoreCase))
            {
                addrValue = null;
                textbox.Text = "null";
                return true;
            }

            // 3：無効な文字列の場合
            if (!TryParseAddress(addrStr, out uint resultValue))
            {
                addrValue = null;
                textbox.Clear();

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

            // 4：有効な文字列の場合
            addrValue = resultValue;
            textbox.Text = resultValue.ToString("X8");
            return true;
        }

        /// <summary>
        /// 入力された16進数文字列の整形のみをイベントに紐付け
        /// </summary>
        public static void AttachAddressAutoFormat(params TextBox[] textboxes)
        {
            foreach (TextBox textbox in textboxes)
            {
                textbox.Leave -= AddressTextBox_Leave;
                textbox.Leave += AddressTextBox_Leave;
            }
        }

        /// <summary>
        /// フォーカスが外れた時の自動整形
        /// </summary>
        private static void AddressTextBox_Leave(object sender, EventArgs e)
        {
            if (sender is TextBox txt)
            {
                ValidateAndFormatInputTextBox(txt, out _, showMessage: false);
            }
        }

        /// <summary>
        /// 保存確認処理
        /// </summary>
        public static DialogResult HandleUnsavedChanges(
            Action saveAction,
            Action discardAction,
            Action cancelAction = null)
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
                    break;
                case DialogResult.No:
                    discardAction?.Invoke();
                    break;
                case DialogResult.Cancel:
                    cancelAction?.Invoke();
                    break;
            }

            return result;
        }

        /// <summary>
        /// コントロールの外側に枠描画
        /// </summary>
        public static void AttachExternalBorder(params Control[] targets)
        {
            foreach (Control target in targets)
            {
                Control parent = target.Parent;
                if (parent == null) continue;

                parent.Paint += (sender, e) =>
                {
                    using (var pen = new Pen(Color.Gray, 1))
                    {
                        var rect = new Rectangle(
                            target.Left - 1,
                            target.Top - 1,
                            target.Width + 1,
                            target.Height + 1);
                        e.Graphics.DrawRectangle(pen, rect);
                    }
                };
            }
        }

        /// <summary>
        /// nud に「&lt;」「&gt;」ボタンを連動させる
        /// </summary>
        public static void AttachNumericUpDownNavigators(
            NumericUpDown nud,
            Button btnPrev,
            Button btnNext)
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

        /// <summary>
        /// 特定のコントロールをラジオボタンと連動させる
        /// </summary>
        public static void AttachEnterEvent(RadioButton rb, Control ctrl)
        {
            ctrl.Enter += (sender, e) => rb.Checked = true;
        }

        /// <summary>
        /// コンボボックスに特定のアイテム名を追加する
        /// </summary>
        public static void SetupComboBoxItems(
            ComboBox cmb,
            int defaultIndex,
            params string[] items)
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

        /// <summary>
        /// テキストファイルから「[00]XXXX」を読み取る
        /// </summary>
        public static void LoadComboBoxFromTextFile(ComboBox comboBox, string filePath)
        {
            bool TryParseLine(string line, out KeyValuePair<int, string> entry)
            {
                int closeBracket = line.IndexOf(']');
                if (line.StartsWith("[") && closeBracket > 1)
                {
                    string hex = line.Substring(1, closeBracket - 1);
                    if (int.TryParse(hex, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out int index))
                    {
                        entry = new KeyValuePair<int, string>(index, line.Trim());
                        return true;
                    }
                }

                entry = default(KeyValuePair<int, string>);
                return false;
            }

            var entries = new List<KeyValuePair<int, string>>();
            foreach (string line in File.ReadLines(filePath))
            {
                if (!string.IsNullOrWhiteSpace(line) && TryParseLine(line, out var entry))
                {
                    entries.Add(entry);
                }
            }

            comboBox.DisplayMember = nameof(KeyValuePair<int, string>.Value);
            comboBox.ValueMember = nameof(KeyValuePair<int, string>.Key);
            comboBox.DataSource = entries;
        }
    }
}
