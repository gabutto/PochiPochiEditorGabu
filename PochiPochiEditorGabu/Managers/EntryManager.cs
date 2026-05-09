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

        public void Save(
            int idx,
            bool appendTerminator = true,
            byte paddingByte1 = GbaConstants.FreeSpaceByte, 
            byte paddingByte2 = GbaConstants.PaddingByte)
        {
            int entrySize = GetEntrySize();
            int offset = (int)Address.Value + (idx * entrySize);

            var singleItemList = new List<T> { Working[idx] };
            IoHelper.WriteStructures(
                _romData, offset, 
                singleItemList, 
                _tblReader, 
                _dynamicLengths, 
                appendTerminator,
                paddingByte1, 
                paddingByte2);

            Original[idx] = CloneHelper.Clone(Working[idx]);
        }

        public int GetEntrySize()
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
                    int length = (attr != null && _dynamicLengths != null && _dynamicLengths.ContainsKey(attr.EntryLength))
                        ? _dynamicLengths[attr.EntryLength] : 0;
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
                if (attr != null)
                {
                    if (!dynamicLengths.ContainsKey(attr.EntryLength))
                        dynamicLengths[attr.EntryLength] = config.GetInt(attr.EntryLength);

                    if (!string.IsNullOrEmpty(attr.AllowedLength) && !dynamicLengths.ContainsKey(attr.AllowedLength))
                        dynamicLengths[attr.AllowedLength] = config.GetInt(attr.AllowedLength);
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

        public void Discard(int idx)
        {
            Working[idx] = CloneHelper.Clone(Original[idx]);
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

    public class PokemonNameEntry
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

    public class PokemonCoordBattleAllyEntry
    {
        [DataBindingHelper.NibbleControlNames(
            "CoordBattleAllyBubbleX",
            "CoordBattleAllyBubbleY")]
        public byte nCoordBattleAllyBubble;
        public byte sCoordBattleAllyPokemon;
        public byte _Padding1;
        public byte _Padding2;
    }

    public class PokemonCoordBattleEnemyEntry
    {
        [DataBindingHelper.NibbleControlNames(
            "CoordBattleEnemyBubbleX",
            "CoordBattleEnemyBubbleY")]
        public byte nCoordBattleEnemyBubble;
        public byte sCoordBattleEnemyPokemon;
        public byte _Padding1;
        public byte _Padding2;
    }

    public class PokemonCoordBattleEnemyShaowEntry
    {
        public byte sCoordBattleEnemyShadowY;
    }

    public class PokemonCoordItemUseEntry
    {
        public byte CoordItemUse2X;
        public byte CoordItemUse2Y;
        public byte CoordItemUse2Zoom;
        public byte CoordItemUse1X;
        public byte CoordItemUse1Y;
    }

    public class PokemonStatsNormalEntry
    {
        public byte StatsHp;
        public byte StatsAtk;
        public byte StatsDef;
        public byte StatsSpeed;
        public byte StatsSpAtk;
        public byte StatsSpDef;
        public byte StatsType1;
        public byte StatsType2;
        public byte StatsCatchRate;
        public byte StatsExp;
        public ushort _StatsEvs;
        public ushort StatsHoldItem1;
        public ushort StatsHoldItem2;
        public byte StatsGender;
        public byte StatsEggStep;
        public byte StatsHappiness;
        public byte StatsGrowthRate;
        public byte StatsEggGroup1;
        public byte StatsEggGroup2;
        public byte StatsAbility1;
        public byte StatsAbility2;
        public byte StatsRunRate;
        [DataBindingHelper.NibbleControlNames("StatsFlip", "StatsColor")]
        public byte nStatsUnknownValue;
        public byte StatsAbilityHidden;
        public byte _Padding1;
    }

    public class PokemonStatsExpansionEntry
    {
        public byte StatsHp;
        public byte StatsAtk;
        public byte StatsDef;
        public byte StatsSpeed;
        public byte StatsSpAtk;
        public byte StatsSpDef;
        public byte StatsType1;
        public byte StatsType2;
        public byte StatsCatchRate;
        public byte _Padding1;
        public ushort _StatsEvs;
        public ushort StatsHoldItem1;
        public ushort StatsHoldItem2;
        public byte StatsGender;
        public byte StatsEggStep;
        public byte StatsHappiness;
        public byte StatsGrowthRate;
        public byte StatsEggGroup1;
        public byte StatsEggGroup2;
        public ushort StatsAbility1;
        public byte StatsRunRate;
        [DataBindingHelper.NibbleControlNames("StatsFlip", "StatsColor")]
        public byte StatsUnknownValue;
        public ushort StatsAbility2;
        public ushort StatsAbilityHidden;
        public ushort StatsExp;
    }

    public class AbilityNameEntry
    {
        [DynamicString("AbilityNameEntryLength")]
        public string _AbilityName = string.Empty;
    }

    public class TypeNameEntry
    {
        [DynamicString("TypeNameEntryLength")]
        public string _TypeName = string.Empty;
    }

    public class PokemonEvolutionEntry
    {
        public byte EvoCondMethod; 
        public byte _padding1;
        public byte EvoCondParam1A;
        public byte EvoCondParam1B; 
        public ushort EvoToPokemon; 
        public byte EvoCondParam2A; 
        public byte EvoCondParam2B;
    }

    public class PokemonLearnsetEntry
    {
        public uint pLearnsetAddr;
    }

    public class TmHmMoveEntry
    {
        public ushort _MoveIdx;
    }

    public class TutorMoveEntry
    {
        public ushort _MoveIdx;
    }

    public class PokedexEntry
    {
        [DynamicString(
            "PokedexCategoryEntryLength", 
            "PokedexCategoryMaxLength")]
        public string _DexCategory;
        public ushort DexHeight;
        public ushort DexWeight;
        public ushort _Padding1;
        public uint pDexDescAddr;
        public ushort _Padding2;
        public ushort DexSizeCompParam1;
        public ushort sDexSizeCompParam2;
        public ushort DexSizeCompParam3;
        public ushort sDexSizeCompParam4;
        public ushort _Padding3;
    }


    public class PokemonCryData1Entry
    {
        public byte _Type;
        public byte _Key;
        public byte _Padding1;
        public byte _Padding2;
        public uint pCryDataAddr;
        public byte _Unknown1;
        public byte _Padding3;
        public byte _Unknown2;
        public byte _Padding4;
    }

    public class PokemonCryData2Entry
    {
        public byte _Type;
        public byte _Key;
        public byte _Padding1;
        public byte _Padding2;
        public uint pCryDataAddr;
        public byte _Unknown1;
        public byte _Padding3;
        public byte _Unknown2;
        public byte _Padding4;
    }

    public class PokemonCryExtendEntry
    {
        public ushort _ExtendIdx;
    }

    public class PokemonBattleMusicEntry
    {
        public ushort BattleMusic;
    }

    public class PokedexOrderEntry
    {
        public ushort _OrderIdx;
    }

    public class PokedexHabitatAreaEntry
    {
        public uint pAreaAddr;
        public byte PageCount;
        public byte _Padding1;
        public byte _Padding2;
        public byte _Padding3;
    }

    public class PokedexHabitatPageEntry
    {
        public uint pPageAddr;
        public byte PokemonCount;
        public byte _Padding1;
        public byte _Padding2;
        public byte _Padding3;
    }

    public class PokedexSearchSortEntry
    {
        public ushort _Idx;
    }

    public class MoveNameEntry
    {
        [DynamicString("MoveNameEntryLength")]
        public string _MoveName = string.Empty;
    }






    public class EggMoveEntry
    {
        public ushort _MoveIdx;
    }






    public class ItemSpriteEntry
    {
        public uint pSpriteImgAddr;
        public uint pSpritePalAddr;
    }

    public class ItemDataEntry
    {
        [DynamicString("ItemNameEntryLength", "ItemNameMaxLength")]
        public string _ItemName = string.Empty;
        public ushort Idx;
        public ushort Price;
        public byte HoldEffectIdx;
        public byte EffectValue;
        public uint pDescAddr;
        public byte CanHold;
        public byte UnknownValue;
        public byte PocketIdx;
        public byte FieldUseType;
        public uint pFieldUseAddr;
        public byte BattleUseType;
        public byte _Padding1;
        public byte _Padding2;
        public byte _Padding3;
        public uint pBattleUseAddr;
        public byte SpecialIdx;
        public byte _Padding4;
        public byte _Padding5;
        public byte _Padding6;
    }

    public class ItemEffectEntry
    {
        public uint pItemEffectAddr;
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
