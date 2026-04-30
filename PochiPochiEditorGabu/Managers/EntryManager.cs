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

        public static EntryManager<T> Create(
            byte[] romData,
            TblFileReader tblReader,
            IniFileReader config,
            string addressKey,
            string countKey)
        {
            var dynamicLengths = new Dictionary<string, int>();

            var fields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (var field in fields)
            {
                var attr = field.GetCustomAttribute<DynamicStringAttribute>();
                if (attr != null && !dynamicLengths.ContainsKey(attr.Key))
                {
                    dynamicLengths[attr.Key] = config.GetInt(attr.Key);
                }
            }

            var manager = new EntryManager<T>(
                romData,
                tblReader,
                dynamicLengths.Count > 0 ? dynamicLengths : null
            );

            uint? tableAddr = config.GetAddr(addressKey);
            int count = config.GetInt(countKey);
            manager.Load(tableAddr, count);

            return manager;
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

    // entry

    public class PokemonNameeEntry
    {
        [DynamicString("PokemonNameEntryLength")]
        public string _PokemonName = string.Empty;
    }

    public class PokemonSpriteFrontImageEntry
    {
        public uint pSpriteFrontImgAddr;
        public ushort _DecompressedSize;
        public byte _Index;
        public byte _Padding1;
    }

    public class PokemonSpriteBackImageEntry
    {
        public uint pSpriteBackImgAddr;
        public ushort _DecompressedSize;
        public byte _Index;
        public byte _Padding1;
    }

    public class PokemonSpriteNormalPaletteEntry
    {
        public uint pSpriteNormalPalAddr;
        public byte _Index;
        public byte _Padding1;
        public byte _Padding2;
        public byte _Padding3;
    }

    public class PokemonSpriteShinyPaletteEntry
    {
        public uint pSpriteShinyPalAddr;
        public byte _Index;
        public byte _Padding1;
        public byte _Padding2;
        public byte _Padding3;
    }

    public class PokemonIconImageEntry
    {
        public uint pIconImgAddr;
    }

    public class PokemonIconPaletteIndexEntry
    {
        public byte IconPalIdx;
    }

    public class PokemonIconPaletteAddressEntry
    {
        public uint _IconPaletteAddr;
        public ushort _Unknown1;
        public byte _Padding1;
        public byte _Padding2;
    }

    public class PokemonFootPrintImageEntry
    {
        public uint pFootprintImgAddr;
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
        public uint pAnimPtrAddr;
    }
}
