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
        public static bool TryReadGbaPointer(uint ptrAddr, byte[] data, out uint? actualAddr)
        {
            uint rawPtr = (uint)data[ptrAddr] |
                         ((uint)data[ptrAddr + 1] << 8) |
                         ((uint)data[ptrAddr + 2] << 16) |
                         ((uint)data[ptrAddr + 3] << 24);

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

        public static List<T> ReadStructures<T>(
            byte[] data,
            uint? addr,
            int count,TblFileReader tblReader,
            Dictionary<string, int> dynamicLengths = null) where T : new()
        {
            var list = new List<T>();

            if (addr == null) return list;
            int currentOffset = (int)addr.Value;

            GCHandle handle = GCHandle.Alloc(data, GCHandleType.Pinned);

            try
            {
                IntPtr basePtr = handle.AddrOfPinnedObject();

                var fields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Instance)
                                      .OrderBy(f => f.MetadataToken)
                                      .ToArray();

                for (int i = 0; i < count; i++)
                {
                    T item = new T();
                    foreach (var field in fields)
                    {
                        if (field.FieldType == typeof(string))
                        {
                            var attr = field.GetCustomAttribute<DynamicStringAttribute>();
                            int length = (attr != null && dynamicLengths != null && dynamicLengths.ContainsKey(attr.EntryLength))
                                ? dynamicLengths[attr.EntryLength] : 0;

                            if (length > 0)
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

        public static void WriteStructures<T>(
            byte[] data,
            int address,
            IEnumerable<T> items,
            TblFileReader tblReader,
            Dictionary<string, int> dynamicLengths = null,
            bool appendTerminator = true,
            byte freeSpaceByte = GbaConstants.FreeSpaceByte,
            byte paddingByte = GbaConstants.PaddingByte)
        {
            GCHandle handle = GCHandle.Alloc(data, GCHandleType.Pinned);

            try
            {
                IntPtr basePtr = handle.AddrOfPinnedObject();
                int currentOffset = address;

                var fields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Instance)
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
                            int entryLength = (
                                attr != null && 
                                dynamicLengths != null && 
                                dynamicLengths.ContainsKey(attr.EntryLength))
                                ? dynamicLengths[attr.EntryLength] : 0;

                            int allowedLength = (
                                attr != null && 
                                !string.IsNullOrEmpty(attr.AllowedLength) && 
                                dynamicLengths != null && 
                                dynamicLengths.ContainsKey(attr.AllowedLength))
                                ? dynamicLengths[attr.AllowedLength] : -1;

                            if (entryLength > 0)
                            {
                                string strVal = (field.GetValue(item) as string) ?? string.Empty;

                                if (allowedLength > 0)
                                {
                                    byte[] rawBytes = tblReader.StringToBytes(strVal, false, -1);
                                    List<byte> finalBytes = new List<byte>(rawBytes);

                                    // FreeSpaceByte
                                    if (appendTerminator)
                                    {
                                        while (finalBytes.Count < allowedLength)
                                        {
                                            finalBytes.Add(freeSpaceByte);
                                        }
                                    }

                                    // PaddingByte
                                    while (finalBytes.Count < entryLength)
                                    {
                                        finalBytes.Add(paddingByte);
                                    }

                                    byte[] result = finalBytes.Take(entryLength).ToArray();
                                    Array.Copy(result, 0, data, currentOffset, entryLength);
                                }
                                else
                                {
                                    byte[] result = tblReader.StringToBytes(strVal, appendTerminator, entryLength, paddingByte);
                                    Array.Copy(result, 0, data, currentOffset, entryLength);
                                }

                                currentOffset += entryLength;
                            }
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
