using System;
using System.Reflection;
using System.Windows.Forms;

using PochiPochiEditorGabu.Constants;

namespace PochiPochiEditorGabu.Helpers
{
    public static class DataBindingHelper
    {
        private static readonly string[] ControlPrefixes = { "txt", "nud", "cmb", "chk" };

        // 構造体→コントロール
        public static void BindObjectToControls(Control container, object obj)
        {
            foreach (var field in obj.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                // スキップ
                if (field.Name.StartsWith("_")) continue;

                object value = field.GetValue(obj);
                if (value == null) continue;

                // ポインタ（ベースアドレス減算）
                if (field.Name.StartsWith("p"))
                {
                    uint ptrValue = (uint)value;
                    uint? offset = ptrValue == 0 
                        ? (uint?)null 
                        : ptrValue - GbaConstants.BaseAddr;
                    SetControlValueByName(container, field.Name.Substring(1), offset);
                }
                // 符号付き
                else if (field.Name.StartsWith("s"))
                {
                    decimal signedValue;

                    unchecked
                    {
                        switch (Type.GetTypeCode(field.FieldType))
                        {
                            case TypeCode.Byte:
                                signedValue = (sbyte)(byte)value;
                                break;
                            case TypeCode.UInt16:
                                signedValue = (short)(ushort)value;
                                break;
                            case TypeCode.UInt32:
                                signedValue = (int)(uint)value;
                                break;
                            default:
                                signedValue = 0m;
                                break;
                        }
                    }

                    SetControlValueByName(container, field.Name.Substring(1), signedValue);
                }
                // ニブル
                else if (field.Name.StartsWith("n"))
                {
                    var attr = field.GetCustomAttribute<NibbleControlNamesAttribute>();
                    if (attr != null && value is byte byteVal)
                    {
                        SetControlValueByName(
                            container, 
                            attr.HighNibbleName,
                            (byteVal >> GbaConstants.NibbleShift) & GbaConstants.NibbleMask);
                        SetControlValueByName(
                            container,
                            attr.LowNibbleName,
                            byteVal & GbaConstants.NibbleMask);
                    }
                }
                // ビットフラグ
                else if (field.Name.StartsWith("b"))
                {
                    var attr = field.GetCustomAttribute<BitControlNamesAttribute>();
                    if (attr != null)
                    {
                        uint uintValue = Convert.ToUInt32(value);
                        for (int i = 0; i < attr.BitNames.Length; i++)
                        {
                            string bitName = attr.BitNames[i];
                            if (!string.IsNullOrEmpty(bitName))
                            {
                                SetControlValueByName(container, bitName, ((uintValue >> i) & 1) == 1);
                            }
                        }
                    }
                }
                else
                {
                    SetControlValueByName(container, field.Name, value);
                }
            }
        }

        // コントロール→構造体
        public static void BindControlsToObject(Control container, object obj)
        {
            Type type = obj.GetType();
            FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);

            foreach (var field in fields)
            {
                // スキップ
                if (field.Name.StartsWith("_")) continue;

                // ポインタ（ベースアドレス加算）
                if (field.Name.StartsWith("p"))
                {
                    object value = GetControlValueByName(container, field.Name.Substring(1), typeof(uint));

                    if (value != null)
                    {
                        uint offset = Convert.ToUInt32(value);
                        field.SetValue(obj, offset == 0 
                            ? 0u 
                            : offset + GbaConstants.BaseAddr);
                    }
                    else
                    {
                        field.SetValue(obj, 0u);
                    }
                }
                // 符号付き
                else if (field.Name.StartsWith("s"))
                {
                    object val = GetControlValueByName(container, field.Name.Substring(1), typeof(decimal));
                    if (val == null) continue;

                    int intVal = (int)Math.Truncate(Convert.ToDecimal(val));

                    unchecked
                    {
                        switch (Type.GetTypeCode(field.FieldType))
                        {
                            case TypeCode.Byte:
                                field.SetValue(obj, (byte)intVal);
                                break;
                            case TypeCode.UInt16:
                                field.SetValue(obj, (ushort)intVal);
                                break;
                            case TypeCode.UInt32:
                                field.SetValue(obj, (uint)intVal);
                                break;
                        }
                    }
                }
                // ニブル
                else if (field.Name.StartsWith("n"))
                {
                    var attr = field.GetCustomAttribute<NibbleControlNamesAttribute>();
                    if (attr == null || Type.GetTypeCode(field.FieldType) != TypeCode.Byte) continue;

                    byte current = (byte)(field.GetValue(obj) ?? (byte)0);

                    object highObj = GetControlValueByName(container, attr.HighNibbleName, typeof(int));
                    object lowObj = GetControlValueByName(container, attr.LowNibbleName, typeof(int));

                    int high = highObj != null 
                        ? Convert.ToInt32(highObj) 
                        : (current >> GbaConstants.NibbleShift) & GbaConstants.NibbleMask;
                    int low = lowObj != null 
                        ? Convert.ToInt32(lowObj) 
                        : current & GbaConstants.NibbleMask;

                    field.SetValue(obj, (byte)(((high & GbaConstants.NibbleMask) << GbaConstants.NibbleShift)
                                              | (low & GbaConstants.NibbleMask)));
                }
                // ビットフラグ
                else if (field.Name.StartsWith("b"))
                {
                    var attr = field.GetCustomAttribute<BitControlNamesAttribute>();
                    if (attr == null) continue;

                    long intValue = Convert.ToInt64(field.GetValue(obj) ?? 0L);

                    for (int i = 0; i < attr.BitNames.Length; i++)
                    {
                        string bitName = attr.BitNames[i];
                        if (string.IsNullOrEmpty(bitName)) continue;

                        object valObj = GetControlValueByName(container, bitName, typeof(bool));
                        if (valObj is bool bitVal)
                        {
                            if (bitVal)
                            {
                                intValue |= (1L << i);
                            }
                            else
                            {
                                intValue &= ~(1L << i);
                            }
                        }
                    }

                    unchecked
                    {
                        switch (Type.GetTypeCode(field.FieldType))
                        {
                            case TypeCode.Byte:
                                field.SetValue(obj, (byte)intValue);
                                break;
                            case TypeCode.UInt16:
                                field.SetValue(obj, (ushort)intValue);
                                break;
                            case TypeCode.UInt32:
                                field.SetValue(obj, (uint)intValue);
                                break;
                        }
                    }
                }
                else
                {
                    object val = GetControlValueByName(container, field.Name, field.FieldType);
                    if (val != null)
                    {
                        Type underlying = Nullable.GetUnderlyingType(field.FieldType) ?? field.FieldType;
                        field.SetValue(obj, Convert.ChangeType(val, underlying));
                    }
                    else if (!field.FieldType.IsValueType || Nullable.GetUnderlyingType(field.FieldType) != null)
                    {
                        field.SetValue(obj, null);
                    }
                }
            }
        }

        private static void SetControlValueByName(Control container, string baseName, object value)
        {
            foreach (string prefix in ControlPrefixes)
            {
                var matched = container.Controls.Find(prefix + baseName, true);
                if (matched.Length > 0)
                {
                    SetControlValue(matched[0], value);
                    return;
                }
            }
        }

        private static object GetControlValueByName(Control container, string baseName, Type targetType)
        {
            foreach (string prefix in ControlPrefixes)
            {
                var matched = container.Controls.Find(prefix + baseName, true);
                if (matched.Length > 0)
                    return GetControlValue(matched[0], targetType);
            }
            return null;
        }

        private static void SetControlValue(Control ctrl, object value)
        {
            switch (ctrl)
            {
                case NumericUpDown nud:
                    decimal dec = Convert.ToDecimal(value ?? nud.Minimum);
                    nud.Value = Math.Max(nud.Minimum, Math.Min(nud.Maximum, dec));
                    break;

                case TextBox txt:
                    if (value == null)
                    {
                        txt.Text = "null";
                    }
                    else if (value is uint uVal)
                    {
                        txt.Text = uVal.ToString("X8");
                    }
                    else
                    {
                        txt.Text = value.ToString() ?? string.Empty;
                    }
                    break;

                case ComboBox cmb:
                    if (!string.IsNullOrEmpty(cmb.ValueMember))
                    {
                        cmb.SelectedValue = (value != null && IsNumericType(value))
                            ? (object)Convert.ToInt32(value)
                            : value;
                    }
                    else if (value != null && IsNumericType(value))
                    {
                        cmb.SelectedIndex = Convert.ToInt32(value);
                    }
                    else
                    {
                        cmb.SelectedIndex = -1;
                    }
                    break;

                case CheckBox chk:
                    chk.Checked = Convert.ToBoolean(value ?? false);
                    break;
            }
        }

        private static object GetControlValue(Control ctrl, Type targetType)
        {
            switch (ctrl)
            {
                case NumericUpDown nud:
                    return nud.Value;
                case TextBox txt:
                    string text = txt.Text.Trim();

                    if (string.IsNullOrEmpty(text) || text == "null")
                    {
                        return null;
                    }

                    if (ControlHelper.TryParseAddress(text, out uint addr))
                    {
                        return addr;
                    }

                    return null;
                case ComboBox cmb:
                    object val = !string.IsNullOrEmpty(cmb.ValueMember)
                        ? cmb.SelectedValue
                        : cmb.SelectedIndex;
                    if (val != null && targetType != typeof(object))
                    {
                        return Convert.ChangeType(val, targetType);
                    }
                    return val;
                case CheckBox chk:
                    return chk.Checked;
                default:
                    return null;
            }
        }

        private static bool IsNumericType(object obj)
        {
            switch (Type.GetTypeCode(obj.GetType()))
            {
                case TypeCode.Byte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.Int16:
                case TypeCode.Int32:
                    return true;
                default:
                    return false;
            }
        }

        [AttributeUsage(AttributeTargets.Field)]
        public class NibbleControlNamesAttribute : Attribute
        {
            public string HighNibbleName { get; }
            public string LowNibbleName { get; }

            public NibbleControlNamesAttribute(string highName, string lowName)
            {
                HighNibbleName = highName;
                LowNibbleName = lowName;
            }
        }

        [AttributeUsage(AttributeTargets.Field)]
        public class BitControlNamesAttribute : Attribute
        {
            public string[] BitNames { get; }
            public BitControlNamesAttribute(params string[] names) => BitNames = names;
        }
    }
}
