using System;
using System.Reflection;
using System.Windows.Forms;

using PochiPochiEditorGabu.Constants;

namespace PochiPochiEditorGabu.Helpers
{
    public static class DataBindingHelper
    {
        public static void BindObjectToControls(Control container, object obj)
        {
            Type type = obj.GetType();
            FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);

            foreach (var field in fields)
            {
                if (field.Name.StartsWith("_")) continue;

                object value = field.GetValue(obj);
                if (value == null) continue;

                if (field.Name.StartsWith("p"))
                {
                    // Pointer
                    uint ptrValue = (uint)value;
                    uint? offset = (ptrValue == 0) ? (uint?)null : ptrValue - GbaConstants.BaseAddr;
                    SetControlValueByName(container, field.Name.Substring(1), offset);
                }
                else if (field.Name.StartsWith("s"))
                {
                    // Signed
                    decimal signedValue = 0;
                    TypeCode typeCode = Type.GetTypeCode(field.FieldType);

                    switch (typeCode)
                    {
                        case TypeCode.Byte:
                            signedValue = unchecked((sbyte)(byte)value);
                            break;
                        case TypeCode.UInt16:
                            signedValue = unchecked((short)(ushort)value);
                            break;
                        case TypeCode.UInt32:
                            signedValue = unchecked((int)(uint)value);
                            break;
                    }
                    SetControlValueByName(container, field.Name.Substring(1), signedValue);
                }
                else if (field.Name.StartsWith("n"))
                {
                    // Nibble
                    var attr = field.GetCustomAttribute<NibbleControlNamesAttribute>();
                    if (attr != null && value is byte byteVal)
                    {
                        int highValue = (byteVal >> GbaConstants.NibbleShift) & GbaConstants.NibbleMask;
                        int lowValue = byteVal & GbaConstants.NibbleMask;

                        SetControlValueByName(container, attr.HighNibbleName, highValue);
                        SetControlValueByName(container, attr.LowNibbleName, lowValue);
                    }
                }
                else if (field.Name.StartsWith("b"))
                {
                    // bit
                    var attr = field.GetCustomAttribute<BitControlNamesAttribute>();
                    if (attr != null)
                    {
                        long intValue = Convert.ToInt64(value);
                        for (int i = 0; i < attr.BitNames.Length; i++)
                        {
                            string bitName = attr.BitNames[i];
                            if (!string.IsNullOrEmpty(bitName))
                            {
                                bool bitValue = ((intValue >> i) & 1) == 1;
                                SetControlValueByName(container, bitName, bitValue);
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

        public static void BindControlsToObject(Control container, object obj)
        {
            Type type = obj.GetType();
            FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);

            foreach (var field in fields)
            {
                if (field.Name.StartsWith("_")) continue;

                if (field.Name.StartsWith("p"))
                {
                    // Pointer
                    object value = GetControlValueByName(container, field.Name.Substring(1), typeof(int));
                    if (value != null)
                    {
                        uint? offsetValue = Convert.ToUInt32(value);
                        uint? finalValue = (offsetValue == null) ? 0u : offsetValue + GbaConstants.BaseAddr;
                        field.SetValue(obj, finalValue);
                    }
                    else
                    {
                        field.SetValue(obj, 0u);
                    }
                }
                else if (field.Name.StartsWith("s"))
                {
                    // Signed
                    object val = GetControlValueByName(container, field.Name.Substring(1), typeof(int));
                    if (val != null && val is int intVal)
                    {
                        TypeCode typeCode = Type.GetTypeCode(field.FieldType);

                        switch (typeCode)
                        {
                            case TypeCode.Byte:
                                field.SetValue(obj, unchecked((byte)intVal));
                                break;
                            case TypeCode.UInt16:
                                field.SetValue(obj, unchecked((ushort)intVal));
                                break;
                            case TypeCode.UInt32:
                                field.SetValue(obj, unchecked((uint)intVal));
                                break;
                        }
                    }
                }
                else if (field.Name.StartsWith("n"))
                {
                    // Nibble
                    var attr = field.GetCustomAttribute<NibbleControlNamesAttribute>();
                    if (attr != null && Type.GetTypeCode(field.FieldType) == TypeCode.Byte)
                    {
                        object highValObj = GetControlValueByName(container, attr.HighNibbleName, typeof(int));
                        object lowValObj = GetControlValueByName(container, attr.LowNibbleName, typeof(int));

                        byte byteVal = (byte)(field.GetValue(obj) ?? (byte)0);

                        int highVal = (highValObj != null && highValObj is int h) ? h
                                    : ((byteVal >> GbaConstants.NibbleShift) & GbaConstants.NibbleMask);

                        int lowVal = (lowValObj != null && lowValObj is int l) ? l
                                    : (byteVal & GbaConstants.NibbleMask);

                        byteVal = (byte)(((highVal & GbaConstants.NibbleMask) << GbaConstants.NibbleShift) | (lowVal & GbaConstants.NibbleMask));
                        field.SetValue(obj, byteVal);
                    }
                }
                else if (field.Name.StartsWith("b"))
                {
                    // bit
                    var attr = field.GetCustomAttribute<BitControlNamesAttribute>();
                    if (attr != null)
                    {
                        long intValue = Convert.ToInt64(field.GetValue(obj) ?? 0);
                        for (int i = 0; i < attr.BitNames.Length; i++)
                        {
                            string bitName = attr.BitNames[i];
                            if (!string.IsNullOrEmpty(bitName))
                            {
                                object valObj = GetControlValueByName(container, bitName, typeof(bool));
                                if (valObj != null && valObj is bool bitVal)
                                {
                                    if (bitVal)
                                        intValue |= (1L << i);
                                    else
                                        intValue &= ~(1L << i);
                                }
                            }
                        }

                        TypeCode typeCode = Type.GetTypeCode(field.FieldType);
                        switch (typeCode)
                        {
                            case TypeCode.Byte:
                                field.SetValue(obj, unchecked((byte)intValue));
                                break;
                            case TypeCode.UInt16:
                                field.SetValue(obj, unchecked((ushort)intValue));
                                break;
                            case TypeCode.UInt32:
                                field.SetValue(obj, unchecked((uint)intValue));
                                break;
                        }
                    }
                }
                else
                {
                    object val = GetControlValueByName(container, field.Name, field.FieldType);
                    if (val != null)
                    {
                        Type underlyingType = Nullable.GetUnderlyingType(field.FieldType) ?? field.FieldType;
                        object convertedValue = Convert.ChangeType(val, underlyingType);
                        field.SetValue(obj, convertedValue);
                    }
                    else
                    {
                        if (!field.FieldType.IsValueType || Nullable.GetUnderlyingType(field.FieldType) != null)
                        {
                            field.SetValue(obj, null);
                        }
                    }
                }
            }
        }

        private static void SetControlValueByName(Control container, string baseName, object value)
        {
            string[] possibleNames = { $"txt{baseName}", $"nud{baseName}", $"cmb{baseName}", $"chk{baseName}" };

            foreach (string name in possibleNames)
            {
                var matchedControls = container.Controls.Find(name, true);

                if (matchedControls.Length > 0)
                {
                    SetControlValue(matchedControls[0], value);
                    break;
                }
            }
        }

        private static object GetControlValueByName(Control container, string baseName, Type targetType)
        {
            string[] possibleNames = { $"txt{baseName}", $"nud{baseName}", $"cmb{baseName}", $"chk{baseName}" };

            foreach (string name in possibleNames)
            {
                var matchedControls = container.Controls.Find(name, true);

                if (matchedControls.Length > 0)
                {
                    return GetControlValue(matchedControls[0], targetType);
                }
            }

            return null;
        }

        private static void SetControlValue(Control ctrl, object value)
        {
            switch (ctrl)
            {
                case NumericUpDown nud:
                    {
                        nud.Value = Convert.ToDecimal(value ?? nud.Minimum);
                    }
                    break;

                case TextBox txt:
                    {
                        if (value == null)
                        {
                            txt.Text = "null";
                        }
                        else if (value is uint uValue)
                        {
                            txt.Text = uValue.ToString("X8");
                        }
                        else
                        {
                            txt.Text = value.ToString() ?? string.Empty;
                        }
                    }
                    break;

                case ComboBox cmb:
                    {
                        if (!string.IsNullOrEmpty(cmb.ValueMember))
                        {
                            if (value != null && IsNumericType(value))
                            {
                                cmb.SelectedValue = Convert.ToInt32(value);
                            }
                            else
                            {
                                if (value == null)
                                {
                                    cmb.SelectedIndex = -1;
                                }
                                else
                                {
                                    cmb.SelectedValue = value;
                                }
                            }
                        }
                        else
                        {
                            if (value != null && IsNumericType(value))
                            {
                                cmb.SelectedIndex = Convert.ToInt32(value);
                            }
                            else if (value == null)
                            {
                                cmb.SelectedIndex = -1;
                            }
                        }
                    }
                    break;

                case CheckBox chk:
                    {
                        chk.Checked = Convert.ToBoolean(value ?? false);
                    }
                    break;
            }
        }

        private static object GetControlValue(Control ctrl, Type targetType)
        {
            switch (ctrl)
            {
                case NumericUpDown nud:
                    {
                        return nud.Value;
                    }

                case TextBox txt:
                    {
                        string text = txt.Text.Trim();

                        if (text == "null") return null;

                        if (ControlHelper.TryParseAddress(text, out uint addr))
                        {
                            return addr;
                        }

                        return null;
                    }

                case ComboBox cmb:
                    {
                        object val;
                        if (!string.IsNullOrEmpty(cmb.ValueMember))
                        {
                            val = cmb.SelectedValue;
                        }
                        else
                        {
                            val = cmb.SelectedIndex;
                        }

                        if (val != null && targetType != typeof(object))
                        {
                            return Convert.ChangeType(val, targetType);
                        }

                        return val;
                    }

                case CheckBox chk:
                    {
                        return chk.Checked;
                    }

                default:
                    {
                        return null;
                    }
            }
        }

        private static bool IsNumericType(object obj)
        {
            switch (Type.GetTypeCode(obj.GetType()))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Decimal:
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

            public BitControlNamesAttribute(params string[] names)
            {
                BitNames = names;
            }
        }
    }
}
