namespace PochiPochiEditorGabu.Constants
{
    public class GbaConstants
    {
        public const int BitsPerByte = 8;
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

        public const int DefaultScale = 2;
        public const string RomFileFilter = "Romファイル|*.gba";
        public const string RomFileTitle = "Romを選択";
        public const string ImageImportFilter = "画像ファイル (*.png;*.bmp)|*.png;*.bmp";
        public const string ImageExportFilter = "PNG画像 (*.png)|*.png|BMP画像 (*.bmp)|*.bmp";
    }
}
