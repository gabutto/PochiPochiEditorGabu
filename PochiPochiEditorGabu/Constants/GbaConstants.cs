namespace PochiPochiEditorGabu.Constants
{
    public class GbaConstants
    {
        public const int HexBase = 16;
        public const int BitsPerByte = 8;
        public const int charPerByte = 2;
        public const int NibbleShift = 4;
        public const int NibbleMask = 0xF;
        public const int Mask2Bits = 0x3;
        public const int Mask8Bits = 0xFF;
        public const int Mask16Bits = 0xFFFF;
        public const uint Mask32Bits = 0xFFFFFFFFU;

        public const uint BaseAddr = 0x8000000U;
        public const int PtrSize = 4;
        public const uint AlignMask = 0xFFFFFFFCU;
        public const int PaddingByte = 0x00;
        public const int FreeSpaceByte = 0xFF;

        public const int PalColorCount = 16;
        public const int BytesPerColor = 2;
        public const int ColorChannelMulti = 8;
        public const int RedShift = 0;
        public const int GreenShift = 5;
        public const int BlueShift = 10;
        public const int RedMask = 0x1F;
        public const int GreenMask = 0x3E0;
        public const int BlueMask = 0x7C00;
        public const int ArgbByteCount = 4;

        public const int TileSize = 8;
        public const int Bpp4 = 4;
        public const int PixelsPerByte4Bpp = BitsPerByte / Bpp4;
        public const int SpriteSize = 64;

        public const int IconFrameSize = 32;
        public const int IconFrameCounts = 2;
        public const int IconBytesPerFrame = (IconFrameSize * IconFrameSize) / PixelsPerByte4Bpp;

        public const int FootprintSize = 16;
        public const int FootprintDataSize = 0x20;
        public const int FootprintBlockCount = 4;
        public const int FootprintBlockDim = 2;
        public const int FootprintTileSize = 8;
        public const int FootprintCanvasScale = 16;

        public const int BattleAllyX = 40;
        public const int BattleAllyY = 48;
        public const int BattleEnemyX = 144;
        public const int BattleEnemyY = 8;
        public const int BattleEnemyShadowX = 160;
        public const int BattleEnemyShadowY = 65;
        public const int BattleAllyBubbleX = 56;
        public const int BattleAllyBubbleY = 72;
        public const int BattleEnemyBubbleX = 160;
        public const int BattleEnemyBubbleY = 24;
        public const int BattleBubbleMultiplier = 4;

        public const int ItemUseAnimPokeX = 88;
        public const int ItemUseAnimPokeY = 40;
        public const int ItemUseAnimItemX = 76;
        public const int ItemUseAnimItemY = 24;
        public const int ItemUse1PreviewItemIdx = 0xD;
        public const int ItemUse2PreviewItemIdx = 0x121;

        public const int EvShiftHp = 0;
        public const int EvShiftAtk = 2;
        public const int EvShiftDefense = 4;
        public const int EvShiftSpeed = 6;
        public const int EvShiftSpAtk = 0;
        public const int EvShiftSpDef = 2;

        public const int LearnsetEntryLength2Byte = 2;
        public const int LearnsetEntryLength3Byte = 3;
        public const int LearnsetMaxLevel2Byte = 127;
        public const int LearnsetMaxLevel3Byte = 255;
        public const int LearnsetTerminator2Byte = 0xFFFF;
        public const int LearnsetTerminator3ByteMoveId = 0x0;
        public const int LearnsetTerminator3ByteLevel = 0xFF;

        public const int PokedexSizeComparisonBaseWidth = 96;
        public const int PokedexSizeComparisonBaseHeight = 72;
        public const int PokedexSizeComparisonPokemonBaseX = 0;
        public const int PokedexSizeComparisonPokemonBaseY = 8;
        public const int PokedexSizeComparisonTrainerBaseX = 40;
        public const int PokedexSizeComparisonTrainerBaseY = 8;
        public const float PokedexSizeComparisonScaleBase = 256.0F;

        public const float WaveformScale = 128.0f;

        public const int EggMoveTableTerminator = 0xFFFF;
        public const int SpeciesIndexThreshold = 0x4E20;

        public const int ItemSpriteSize = 24;

        public const int DefaultScale = 2;
        public const string RomFileFilter = "ROMファイル|*.gba";
        public const string RomFileTitle = "ROMを選択";
        public const string ImageImportFilter = "画像ファイル (*.png;*.bmp)|*.png;*.bmp";
        public const string ImageExportFilter = "PNG画像 (*.png)|*.png|BMP画像 (*.bmp)|*.bmp";
        public const string BinImportExportFilter = "BINファイル (*.bin)|*.bin";

        public const int WordGroupPokemon2 = 0x0;
        public const int WordGroupTrainer = 0x1;
        public const int WordGroupStatus = 0x2;
        public const int WordGroupBattle = 0x3;
        public const int WordGroupGreeting = 0x4;
        public const int WordGroupPeople = 0x5;
        public const int WordGroupVoice = 0x6;
        public const int WordGroupSpeech = 0x7;
        public const int WordGroupEnding = 0x8;
        public const int WordGroupFeeling = 0x9;
        public const int WordGroupCondition = 0xA;
        public const int WordGroupAction = 0xB;
        public const int WordGroupLifestyle = 0xC;
        public const int WordGroupHobby = 0xD;
        public const int WordGroupTime = 0xE;
        public const int WordGroupMisc = 0xF;
        public const int WordGroupAdjective = 0x10;
        public const int WordGroupEvent = 0x11;
        public const int WordGroupMove1 = 0x12;
        public const int WordGroupMove2 = 0x13;
        public const int WordGroupTrendy = 0x14;
        public const int WordGroupPokemon1 = 0x15;

        public const byte OverworldSpriteUnknownFlag1Mask = 0x10;
        public const byte OverworldSpriteUnknownFlag2Mask = 0x40;
        public const byte OverworldSpriteUnknownFlag3Mask = 0x80;
    }
}
