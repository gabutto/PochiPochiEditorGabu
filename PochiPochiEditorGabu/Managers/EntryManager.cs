using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using PochiPochiEditorGabu.Constants;
using PochiPochiEditorGabu.FileReaders;
using PochiPochiEditorGabu.Helpers;

namespace PochiPochiEditorGabu.Managers
{
    public class EntryManager<T> where T : class, new()
    {
        private readonly byte[] _romData;
        private readonly TblFileReader _tblReader;
        private readonly Dictionary<string, int> _dynamicLengths;

        public List<T> Original { get; private set; } = new List<T>();
        public List<T> Working { get; private set; } = new List<T>();

        public uint? Address { get; set; }
        public int Count { get; set; }

        public EntryManager(byte[] romData, TblFileReader tblReader, Dictionary<string, int> dynamicLengths = null)
        {
            _romData = romData;
            _tblReader = tblReader;
            _dynamicLengths = dynamicLengths;
        }

        public void Load(uint? address, int count)
        {
            Address = address;
            Count = count;

            Original = IoHelper.ReadStructures<T>(_romData, address, count, _tblReader, _dynamicLengths);
            Working = Original.Select(x => CloneHelper.Clone(x)).ToList();
        }

        public void Save(int idx, bool appendTerminator = true, byte paddingByte = GbaConstants.PaddingByte)
        {
            int entrySize = GetEntrySize();
            int offset = (int)Address.Value + (idx * entrySize);

            var singleItemList = new List<T> { Working[idx] };
            IoHelper.WriteStructures(_romData, offset, singleItemList, _tblReader, _dynamicLengths, appendTerminator, paddingByte);

            Original[idx] = CloneHelper.Clone(Working[idx]);
        }

        private int GetEntrySize()
        {
            var fields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Instance)
                                  .OrderBy(f => f.MetadataToken)
                                  .ToArray();

            int size = 0;
            foreach (var field in fields)
            {
                if (field.FieldType == typeof(string))
                {
                    var attr = field.GetCustomAttribute<DynamicStringAttribute>();
                    int length = (attr != null && _dynamicLengths != null && _dynamicLengths.ContainsKey(attr.Key))
                        ? _dynamicLengths[attr.Key] : 0;
                    size += length;
                }
                else if (field.FieldType.IsValueType)
                {
                    size += System.Runtime.InteropServices.Marshal.SizeOf(field.FieldType);
                }
            }
            return size;
        }
    }

    public static class CloneHelper
    {
        public static T Clone<T>(T source) where T : class, new()
        {
            var method = typeof(T).GetMethod("MemberwiseClone", BindingFlags.NonPublic | BindingFlags.Instance);
            return (T)method?.Invoke(source, null);
        }
    }






    public class TrainerClassNameEntry
    {
        [DynamicString("TrainerClassNameEntryLength")]
        public string _ClassName = string.Empty;
    }

    public class TrainerClassPrizeMultiplierEntry
    {
        public byte _ClassNameIndex;
        public byte _PrizeMultiplier;
        public byte _Padding1;
        public byte _Padding2;
    }

    public class TrainerClassEncounterMusicEntry
    {
        public ushort EncounterMusicIndex;
    }

    public class TrainerClassBattleMusicEntry
    {
        public ushort BattleMusicIndex;
    }

    public class TrainerClassPokeBallEntry
    {
        public byte PokeBallIndex;
    }

    public class TrainerClassBaseIVEntry
    {
        public byte BaseIv;
    }

    public class TrainerSpriteImageEntry
    {
        public uint pSpriteImgAddr;
        public ushort _DecompressedSize;
        public byte _Index;
        public byte _Padding1;
    }

    public class TrainerSpritePaletteEntry
    {
        public uint pSpritePalAddr;
        public byte _Index;
        public byte _Padding1;
        public byte _Padding2;
        public byte _Padding3;
    }

    public class TrainerSpriteYOffsetEntry
    {
        public byte _TileCount;
        public byte SpriteYOffset;
        public byte _Padding1;
        public byte _Padding2;
    }

    public class TrainerSpriteAnimationPointerEntry
    {
        public uint pAnimPointerAddr;
    }
}
