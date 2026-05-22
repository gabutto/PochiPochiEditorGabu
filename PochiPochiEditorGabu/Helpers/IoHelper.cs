using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

using PochiPochiEditorGabu.Constants;
using PochiPochiEditorGabu.FileReaders;

namespace PochiPochiEditorGabu.Helpers
{
    public static class IoHelper
    {
        // リトルエンディアン読み取り（nullポインタ考慮）
        public static bool TryReadGbaPointer(uint ptrAddr, byte[] data, out uint? actualAddr)
        {
            uint rawPtr = (uint)data[ptrAddr]
                        | ((uint)data[ptrAddr + 1] << 8)
                        | ((uint)data[ptrAddr + 2] << 16)
                        | ((uint)data[ptrAddr + 3] << 24);

            if (rawPtr == 0)
            {
                actualAddr = null;
                return true;
            }

            if (rawPtr < GbaConstants.BaseAddr)
            {
                actualAddr = null;
                return false;
            }

            actualAddr = rawPtr - GbaConstants.BaseAddr;
            return true;
        }

        // 構造体読み取り（可変長文字列を考慮）
        public static List<T> ReadStructures<T>(
            byte[] data,
            uint? addr,
            int count,
            TblFileReader tblReader,
            Dictionary<string, int> dynamicLengths = null) where T : new()
        {
            var list = new List<T>(count);
            if (addr == null) return list;

            int currentOffset = (int)addr.Value;
            var handle = GCHandle.Alloc(data, GCHandleType.Pinned);

            try
            {
                IntPtr basePtr = handle.AddrOfPinnedObject();

                FieldInfo[] fields = typeof(T)
                    .GetFields(BindingFlags.Public | BindingFlags.Instance)
                    .OrderBy(f => f.MetadataToken)
                    .ToArray();

                for (int i = 0; i < count; i++)
                {
                    var item = new T();

                    foreach (var field in fields)
                    {
                        if (field.FieldType == typeof(string))
                        {
                            var attr = field.GetCustomAttribute<DynamicStringAttribute>();
                            if (TryGetLength(attr?.EntryLength, dynamicLengths, out int length) && length > 0)
                            {
                                string strVal = tblReader.BytesToString(data, currentOffset, length);
                                field.SetValue(item, strVal);
                                currentOffset += length;
                            }
                        }
                        else if (field.FieldType.IsValueType)
                        {
                            int typeSize = Marshal.SizeOf(field.FieldType);
                            object val = Marshal.PtrToStructure(basePtr + currentOffset, field.FieldType);
                            field.SetValue(item, val);
                            currentOffset += typeSize;
                        }
                    }

                    list.Add(item);
                }
            }
            finally
            {
                handle.Free();
            }

            return list;
        }

        // 単一構造体の書き込み
        // paddingByte1は最大文字数まで埋める
        // paddingByte2はデータ長まで埋める
        public static void WriteStructures<T>(
            byte[] data,
            uint? addr,
            IEnumerable<T> items,
            TblFileReader tblReader,
            Dictionary<string, int> dynamicLengths = null,
            bool appendTerminator = true,
            byte paddingByte1 = GbaConstants.FreeSpaceByte,
            byte paddingByte2 = GbaConstants.PaddingByte)
        {
            if (addr == null) return;
            var handle = GCHandle.Alloc(data, GCHandleType.Pinned);

            try
            {
                IntPtr basePtr = handle.AddrOfPinnedObject();
                int currentOffset = (int)addr.Value;

                FieldInfo[] fields = typeof(T)
                    .GetFields(BindingFlags.Public | BindingFlags.Instance)
                    .OrderBy(f => f.MetadataToken)
                    .ToArray();

                foreach (var item in items)
                {
                    if (item == null) continue;

                    foreach (var field in fields)
                    {
                        if (field.FieldType == typeof(string))
                        {
                            var attr = field.GetCustomAttribute<DynamicStringAttribute>();

                            if (!TryGetLength(attr?.EntryLength, dynamicLengths, out int entryLength) || entryLength <= 0)
                            {
                                continue;
                            }

                            string strVal = field.GetValue(item) is string s 
                                ? s 
                                : string.Empty;

                            if (TryGetLength(attr?.AllowedLength, dynamicLengths, out int allowedLength) && allowedLength > 0)
                            {
                                byte[] rawBytes = tblReader.StringToBytes(strVal, false, -1);
                                var finalBytes = new List<byte>(rawBytes);

                                if (appendTerminator)
                                {
                                    finalBytes.Add(GbaConstants.FreeSpaceByte);

                                    while (finalBytes.Count < allowedLength)
                                    {
                                        finalBytes.Add(paddingByte1);
                                    }
                                }

                                while (finalBytes.Count < entryLength)
                                {
                                    finalBytes.Add(paddingByte2);
                                }
                               
                                Array.Copy(finalBytes.ToArray(), 0, data, currentOffset, entryLength);
                            }
                            else
                            {
                                byte[] result = tblReader.StringToBytes(strVal, appendTerminator, entryLength, paddingByte2);
                                Array.Copy(result, 0, data, currentOffset, entryLength);
                            }

                            currentOffset += entryLength;
                        }
                        else if (field.FieldType.IsValueType)
                        {
                            int typeSize = Marshal.SizeOf(field.FieldType);
                            object value = field.GetValue(item);
                            if (value != null)
                            {
                                Marshal.StructureToPtr(value, basePtr + currentOffset, false);
                            }
                            
                            currentOffset += typeSize;
                        }
                    }
                }
            }
            finally
            {
                handle.Free();
            }
        }

        // 可変長の長さを取得
        private static bool TryGetLength(string key, Dictionary<string, int> dynamicLengths, out int length)
        {
            length = 0;
            return key != null && dynamicLengths != null && dynamicLengths.TryGetValue(key, out length);
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class DynamicStringAttribute : Attribute
    {
        public string EntryLength { get; }
        public string AllowedLength { get; }

        public DynamicStringAttribute(string entryLength, string allowedLength = null)
        {
            EntryLength = entryLength;
            AllowedLength = allowedLength;
        }
    }
}
