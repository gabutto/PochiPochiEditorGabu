using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

using PochiPochiEditorGabu.Helpers;

namespace PochiPochiEditorGabu.FileReaders
{
    public class IniFileReader
    {
        private readonly Dictionary<string, object> _inicache = new Dictionary<string, object>();
        private readonly Dictionary<string, List<string>> _configBlocks = new Dictionary<string, List<string>>();

        public T? GetValue<T>(string key) where T : struct
        {
            if (_inicache.TryGetValue(key, out object val) && val != null)
            {
                Type t = typeof(T);

                if (t == typeof(uint))
                {
                    return (T)(object)(uint)val;
                }
                // convert uint to int
                else if (t == typeof(int))
                {
                    return (T)(object)(int)(uint)val;
                }
                else if (t == typeof(bool))
                {
                    return (T)(object)(bool)val;
                }
            }

            return null;
        }

        public uint? GetAddr(string key) => GetValue<uint>(key); // for null ptr
        public int GetInt(string key, int defaultValue = 0) => GetValue<int>(key) ?? defaultValue;
        public bool GetBool(string key, bool defaultValue = false) => GetValue<bool>(key) ?? defaultValue;

        private const string BeginPrefix = "[BEGIN][";
        private const string EndPrefix = "[END][";
        private const string Suffix = "]";
        private const string HexPrefix = "0x";

        public IniFileReader(ComboBox targetCmb, string filePath)
        {
            if (!File.Exists(filePath)) return;

            _inicache.Clear();
            targetCmb.Items.Clear();
            string currentConfigName = string.Empty;
            List<string> currentBlock = new List<string>();

            // Include empty lines
            foreach (string line in File.ReadLines(filePath, Encoding.UTF8))
            {
                if (line.StartsWith(BeginPrefix) && line.EndsWith(Suffix))
                {
                    int start = BeginPrefix.Length;
                    int length = line.Length - Suffix.Length - BeginPrefix.Length;
                    currentConfigName = line.Substring(start, length);
                    currentBlock = new List<string>(); // new one
                }
                else if (line.StartsWith(EndPrefix) && line.EndsWith(Suffix))
                {
                    int start = EndPrefix.Length;
                    int length = line.Length - Suffix.Length - EndPrefix.Length;
                    string endConfigName = line.Substring(start, length);
                    if (currentConfigName == endConfigName)
                    {
                        _configBlocks[currentConfigName] = currentBlock;
                        targetCmb.Items.Add(currentConfigName);
                    }
                    currentConfigName = string.Empty;
                    currentBlock = new List<string>(); // clear
                }
                else if (!string.IsNullOrEmpty(currentConfigName))
                {
                    currentBlock.Add(line);
                }
            }

            if (targetCmb.Items.Count > 0)
            {
                targetCmb.SelectedIndex = 0;
            }
        }

        public void LoadConfig(string selectedConfig, byte[] data)
        {
            if (!_configBlocks.ContainsKey(selectedConfig)) return;

            foreach (string line in _configBlocks[selectedConfig])
            {
                if (TryParseLine(line, out string key, out string rawValue))
                {
                    if (TryParseValue(rawValue, data, out object parsedValue))
                    {
                        _inicache[key] = parsedValue;
                    }
                }
            }
        }

        private bool TryParseLine(string line, out string key, out string rawValue)
        {
            key = string.Empty;
            rawValue = string.Empty;

            if (string.IsNullOrWhiteSpace(line) || line.StartsWith(";")) return false;

            // include empty key and rawvalue
            string[] parts = line.Split('=');
            key = parts[0].Trim();
            rawValue = parts[1].Trim();
            return true;
        }

        private bool TryParseValue(string rawValue, byte[] data, out object parsedValue)
        {
            if (string.IsNullOrEmpty(rawValue))
            {
                parsedValue = null;
                return false;
            }

            if (bool.TryParse(rawValue, out bool boolValue))
            {
                parsedValue = boolValue;
                return true;
            }

            switch (rawValue[0])
            {
                case '*':
                    parsedValue = ReadPointerAddress(rawValue.Substring(1), data);
                    return true;

                case '$':
                    parsedValue = SearchBinaryAndReadPointer(rawValue.Substring(1), data);
                    return true;
            }

            // intentionally convert uint or null
            return TryParseNumber(rawValue, out parsedValue);
        }

        private bool TryParseNumber(string rawValue, out object parsedValue)
        {
            parsedValue = null;
            if (string.IsNullOrWhiteSpace(rawValue)) return false;

            if (rawValue.StartsWith(HexPrefix))
            {
                string hexPart = rawValue.Substring(HexPrefix.Length);
                if (uint.TryParse(hexPart, System.Globalization.NumberStyles.HexNumber, null, out uint hexResult))
                {
                    parsedValue = hexResult;
                    return true;
                }
            }
            else if (uint.TryParse(rawValue, out uint decResult))
            {
                parsedValue = decResult;
                return true;
            }

            return false;
        }

        private uint? ReadPointerAddress(string addrStr, byte[] data)
        {
            if (TryParseNumber(addrStr, out object addrValue))
            {
                uint ptrAddr = (uint)addrValue;

                if (IoHelper.TryReadGbaPointer(ptrAddr, data, out uint? actualAddr))
                {
                    return actualAddr;
                }
                else
                {
                    return null;
                }
            }

            return null;
        }

        private uint? SearchBinaryAndReadPointer(string searchStr, byte[] data)
        {
            string[] parts = searchStr.Split(',');
            string hexString = parts[0].Trim();
            int additionalOffset = 0;
            if (parts.Length > 1 && TryParseNumber(parts[1].Trim(), out object parsedValue))
            {
                additionalOffset = (int)(uint)parsedValue;
            }

            byte[] patternBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < patternBytes.Length; i++)
            {
                patternBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }

            bool isPatternFound = false;
            uint startAddr = 0;

            for (uint i = 0; i <= data.Length - patternBytes.Length; i++)
            {
                bool isMatch = true;
                for (int j = 0; j < patternBytes.Length; j++)
                {
                    if (data[i + j] != patternBytes[j])
                    {
                        isMatch = false;
                        break;
                    }
                }

                if (isMatch)
                {
                    startAddr = i;
                    isPatternFound = true;
                    break;
                }
            }

            if (!isPatternFound) return null;

            uint ptrAddr = startAddr + (uint)patternBytes.Length + (uint)additionalOffset;

            if (IoHelper.TryReadGbaPointer(ptrAddr, data, out uint? actualAddr))
            {
                return actualAddr;
            }

            return null;
        }
    }
}
