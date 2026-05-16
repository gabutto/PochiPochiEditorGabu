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

        public IReadOnlyList<PointerEntry> ParsePointerEntries(
            int startOffset,
            string patternString,
            int maxEntries,
            int? knownCount = null)
        {
            var result = new List<PointerEntry>();
            int limit = knownCount ?? maxEntries;
            PatternDefinition pattern = ParsePattern(patternString);

            for (int i = 0; i < limit; i++)
            {
                int cursor = startOffset + (i * pattern.Length);

                if (!TryMatchPattern(
                    cursor,
                    pattern,
                    out uint targetOffset,
                    out uint px,
                    out uint py,
                    out uint pz))
                {
                    break;
                }

                result.Add(new PointerEntry
                {
                    Index = i,
                    EntryOffset = (uint)cursor,
                    TargetOffset = targetOffset,
                    ParamX = px,
                    ParamY = py,
                    ParamZ = pz
                });
            }

            return result;
        }

        public IReadOnlyList<DataEntry> ParseDataEntries(
            int startOffset,
            string patternString,
            int? maxEntries = null)
        {
            var result = new List<DataEntry>();
            PatternDefinition pattern = ParsePattern(patternString);

            int cursor = startOffset;
            int count = 0;

            while (cursor <= _data.Length - pattern.Length)
            {
                if (maxEntries.HasValue && count >= maxEntries.Value)
                {
                    break;
                }

                if (!TryMatchPattern(
                    cursor,
                    pattern,
                    out _,
                    out uint px,
                    out uint py,
                    out uint pz))
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
            string[] parts = patternStr.Split(
                new char[] { ' ' },
                StringSplitOptions.RemoveEmptyEntries);

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
                    patternBytes[i] = new PatternByte
                    {
                        MatchType = ByteMatchType.Wildcard
                    };
                }
                else if (token == "PP")
                {
                    patternBytes[i] = new PatternByte
                    {
                        MatchType = ByteMatchType.Pointer
                    };

                    if (def.PointerOffset == -1)
                    {
                        def.PointerOffset = i;
                    }

                    ppCount++;
                }
                else if (token.Length == 2 &&
                         IsParamToken(token, out char type, out int paramIndex))
                {
                    int size =
                        type == 'b' ? 1 :
                        type == 's' ? 2 : 4;

                    if (!def.Params[paramIndex].IsUsed)
                    {
                        def.Params[paramIndex].IsUsed = true;
                        def.Params[paramIndex].Offset = i;
                        def.Params[paramIndex].Size = size;
                    }

                    patternBytes[i] = new PatternByte
                    {
                        MatchType = ByteMatchType.Parameter
                    };
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

        private bool IsParamToken(
            string token,
            out char type,
            out int paramIndex)
        {
            type = token[0];
            char id = token[1];

            paramIndex = -1;

            if ((type == 'b' || type == 's' || type == 'i') &&
                (id == 'X' || id == 'Y' || id == 'Z'))
            {
                paramIndex = id - 'X';
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
            out uint paramZ)
        {
            targetOffset = 0;
            paramX = 0;
            paramY = 0;
            paramZ = 0;

            for (int i = 0; i < pattern.Length; i++)
            {
                PatternByte pByte = pattern.Bytes[i];

                if (pByte.MatchType == ByteMatchType.Exact)
                {
                    if (_data[cursor + i] != pByte.Value)
                    {
                        return false;
                    }
                }
            }

            if (pattern.HasPointer)
            {
                uint rawAddr =
                    ReadUInt32LE(cursor + pattern.PointerOffset);

                if (!IsValidPointer(rawAddr, out targetOffset))
                {
                    return false;
                }
            }

            if (pattern.Params[0].IsUsed)
            {
                paramX = ReadParam(
                    cursor + pattern.Params[0].Offset,
                    pattern.Params[0].Size);
            }

            if (pattern.Params[1].IsUsed)
            {
                paramY = ReadParam(
                    cursor + pattern.Params[1].Offset,
                    pattern.Params[1].Size);
            }

            if (pattern.Params[2].IsUsed)
            {
                paramZ = ReadParam(
                    cursor + pattern.Params[2].Offset,
                    pattern.Params[2].Size);
            }

            return true;
        }

        private uint ReadParam(int offset, int size)
        {
            if (size == sizeof(byte))
            {
                return _data[offset];
            }

            if (size == sizeof(ushort))
            {
                return (uint)ReadUShort16LE(offset);
            }

            if (size == sizeof(uint))
            {
                return ReadUInt32LE(offset);
            }

            return 0;
        }

        private bool IsValidPointer(
            uint rawAddr,
            out uint targetOffset)
        {
            targetOffset = 0;

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

        private ushort ReadUShort16LE(int offset)
        {
            return (ushort)(
                _data[offset] |
                (_data[offset + 1] << 8));
        }

        private uint ReadUInt32LE(int offset)
        {
            return (uint)(
                _data[offset] |
                (_data[offset + 1] << 8) |
                (_data[offset + 2] << 16) |
                (_data[offset + 3] << 24));
        }

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

            public ParamDefinition[] Params { get; set; }
                = new ParamDefinition[3];
        }
    }
}
