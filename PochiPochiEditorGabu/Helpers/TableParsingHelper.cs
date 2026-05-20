using System;
using System.Collections.Generic;

using PochiPochiEditorGabu.Constants;

namespace PochiPochiEditorGabu.Helpers
{
    public class PointerEntry
    {
        public int Index { get; set; }
        public uint TargetOffset { get; set; }
        public uint EntryOffset { get; set; }
        public uint ParamX { get; set; }
        public uint ParamY { get; set; }
        public uint ParamZ { get; set; }
    }

    public class DataEntry
    {
        public uint EntryOffset { get; set; }
        public uint ParamX { get; set; }
        public uint ParamY { get; set; }
        public uint ParamZ { get; set; }
    }

    public class TableParsingHelper
    {
        private readonly byte[] _data;
        private readonly uint _baseAddr;
        private readonly int _ptrSize;

        public TableParsingHelper(byte[] data)
        {
            _data = data;
            _baseAddr = GbaConstants.BaseAddr;
            _ptrSize = GbaConstants.PtrSize;
        }
        
        /// <summary>
        /// ポインタエントリー判定
        /// </summary>
        public IReadOnlyList<PointerEntry> ParsePointerEntries(
            int startOffset,
            string patternString,
            int? maxEntries = null,
            HashSet<uint> referencePointers = null,
            bool allowNullPointer = false)
        {
            var result = new List<PointerEntry>();
            var pattern = ParsePattern(patternString);
            int cursor = startOffset;
            int count = 0;

            while (cursor <= _data.Length - pattern.Length)
            {
                if (maxEntries.HasValue && count >= maxEntries.Value) break;

                // エントリー0を除く位置が参照ポインタと一致したら終端と判断する
                if (count > 0 && referencePointers != null && referencePointers.Contains((uint)cursor)) break;

                if (!TryMatchPattern(cursor, pattern, out uint targetOffset,
                                     out uint px, out uint py, out uint pz,
                                     allowNullPointer))
                {
                    break;
                }

                result.Add(new PointerEntry
                {
                    Index = count,
                    EntryOffset = (uint)cursor,
                    TargetOffset = targetOffset,
                    ParamX = px,
                    ParamY = py,
                    ParamZ = pz
                });

                cursor += pattern.Length;
                count++;
            }

            return result;
        }

        /// <summary>
        /// データエントリー判定
        /// </summary>
        public IReadOnlyList<DataEntry> ParseDataEntries(
            int startOffset,
            string patternString,
            int? maxEntries = null,
            bool allowNullPointer = false)
        {
            var result = new List<DataEntry>();
            var pattern = ParsePattern(patternString);
            int cursor = startOffset;
            int count = 0;

            while (cursor <= _data.Length - pattern.Length)
            {
                if (maxEntries.HasValue && count >= maxEntries.Value) break;

                if (!TryMatchPattern(cursor, pattern, out _,
                                     out uint px, out uint py, out uint pz,
                                     allowNullPointer))
                {
                    break;
                }

                result.Add(new DataEntry
                {
                    EntryOffset = (uint)cursor,
                    ParamX = px,
                    ParamY = py,
                    ParamZ = pz
                });

                cursor += pattern.Length;
                count++;
            }

            return result;
        }

        private PatternDefinition ParsePattern(string patternStr)
        {
            string[] parts = patternStr.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var patternBytes = new PatternByte[parts.Length];
            var def = new PatternDefinition
            {
                Length = parts.Length,
                Bytes = patternBytes,
                PointerOffset = -1
            };

            int ppCount = 0;

            for (int i = 0; i < parts.Length; i++)
            {
                string token = parts[i];

                if (token == "??")
                {
                    patternBytes[i] = new PatternByte { MatchType = ByteMatchType.Wildcard };
                }
                else if (token == "PP")
                {
                    patternBytes[i] = new PatternByte { MatchType = ByteMatchType.Pointer };
                    if (def.PointerOffset == -1) def.PointerOffset = i;
                    ppCount++;
                }
                else if (token.Length == 2 && IsParamToken(token, out char type, out int paramIdx))
                {
                    // b=1, s=2, i=4 バイト
                    int size = type == 'b' ? 1 : (type == 's' ? 2 : 4);

                    ref ParamDefinition p = ref def.Params[paramIdx];
                    if (!p.IsUsed)
                    {
                        p.IsUsed = true;
                        p.Offset = i;
                        p.Size = size;
                    }

                    patternBytes[i] = new PatternByte { MatchType = ByteMatchType.Parameter };
                }
                else
                {
                    patternBytes[i] = new PatternByte
                    {
                        MatchType = ByteMatchType.Exact,
                        Value = Convert.ToByte(token, 16)
                    };
                }
            }

            def.HasPointer = ppCount == _ptrSize;
            return def;
        }

        private static bool IsParamToken(string token, out char type, out int paramIndex)
        {
            type = token[0];
            char id = token[1];
            paramIndex = -1;

            if ((type == 'b' || type == 's' || type == 'i') && (id >= 'X' && id <= 'Z'))
            {
                paramIndex = id - 'X'; // X=0, Y=1, Z=2
                return true;
            }
            return false;
        }

        private bool TryMatchPattern(
            int cursor,
            PatternDefinition pattern,
            out uint targetOffset,
            out uint paramX,
            out uint paramY,
            out uint paramZ,
            bool allowNullPointer)
        {
            targetOffset = 0;
            paramX = paramY = paramZ = 0;

            // 固定値バイト
            for (int i = 0; i < pattern.Length; i++)
            {
                if (pattern.Bytes[i].MatchType == ByteMatchType.Exact &&
                    _data[cursor + i] != pattern.Bytes[i].Value)
                {
                    return false;
                }
            }

            // ポインタ
            if (pattern.HasPointer)
            {
                uint rawAddr = ReadUInt32LE(cursor + pattern.PointerOffset);
                if (!IsValidPointer(rawAddr, out targetOffset, allowNullPointer))
                {
                    return false;
                }
            }

            // パラメータ
            if (pattern.Params[0].IsUsed)
            {
                paramX = ReadParam(cursor + pattern.Params[0].Offset, pattern.Params[0].Size);
            }
               
            if (pattern.Params[1].IsUsed)
            {
                paramY = ReadParam(cursor + pattern.Params[1].Offset, pattern.Params[1].Size);
            }
                
            if (pattern.Params[2].IsUsed)
            {
                paramZ = ReadParam(cursor + pattern.Params[2].Offset, pattern.Params[2].Size);
            }

            return true;
        }

        private uint ReadParam(int offset, int size)
        {
            switch (size)
            {
                case 1: 
                    return _data[offset];
                case 2: 
                    return ReadUInt16LE(offset);
                case 4: 
                    return ReadUInt32LE(offset);
                default: 
                    return 0;
            }
        }

        private bool IsValidPointer(uint rawAddr, out uint targetOffset, bool allowNullPointer)
        {
            targetOffset = 0;

            if (rawAddr == 0)
            {
                return allowNullPointer;
            }

            if (rawAddr < _baseAddr)
            {
                return false;
            }

            byte msb = (byte)(rawAddr >> 24);
            if (msb != 0x08 && msb != 0x09)
            {
                return false;
            }

            uint offset = rawAddr - _baseAddr;
            if (offset >= _data.Length)
            {
                return false;
            }

            targetOffset = offset;
            return true;
        }

        private uint ReadUInt16LE(int offset)
            => (uint)(_data[offset] | (_data[offset + 1] << 8));

        private uint ReadUInt32LE(int offset)
            => (uint)(_data[offset] |
                     (_data[offset + 1] << 8) |
                     (_data[offset + 2] << 16) |
                     (_data[offset + 3] << 24));

        private enum ByteMatchType
        {
            Exact, 
            Wildcard, 
            Pointer, 
            Parameter 
        }

        private struct PatternByte
        {
            public ByteMatchType MatchType;
            public byte Value;
        }

        private struct ParamDefinition
        {
            public bool IsUsed;
            public int Offset;
            public int Size;
        }

        private class PatternDefinition
        {
            public int Length { get; set; }
            public PatternByte[] Bytes { get; set; }
            public int PointerOffset { get; set; }
            public bool HasPointer { get; set; }

            // Params[0] = X, Params[1] = Y, Params[2] = Z
            public ParamDefinition[] Params { get; set; } = new ParamDefinition[3];
        }
    }
}
