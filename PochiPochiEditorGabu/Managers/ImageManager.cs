using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using PochiPochiEditorGabu.Constants;

namespace PochiPochiEditorGabu.Managers
{
    public static class ImageManager
    {
        private const int Lz77HeaderSize = 0x4;
        private const int Lz77HeaderIdentifier = 0x10;
        private const int Lz77MaxDistance = 4096;
        private const int Lz77MaxLength = 18;
        private const int Lz77MinMatchLength = 3;
        private const int Lz77MinSafeDistance = 2;
        private const int Lz77CompressedUnitSize = 2;

        public static byte[] DecompressLZ77(byte[] romData, uint address)
        {
            int header = BitConverter.ToInt32(romData, (int)address);
            int decompressedSize = header >> GbaConstants.BitsPerByte;
            var result = new byte[decompressedSize];

            int srcPos = (int)address + Lz77HeaderSize;
            int dstPos = 0;

            while (dstPos < decompressedSize)
            {
                byte flagByte = romData[srcPos++];

                for (int i = 0; i < GbaConstants.BitsPerByte; i++)
                {
                    if (dstPos >= decompressedSize) break;

                    bool isCompressed = (flagByte & (1 << (GbaConstants.BitsPerByte - 1 - i))) != 0;

                    if (isCompressed)
                    {
                        byte byte0 = romData[srcPos];
                        byte byte1 = romData[srcPos + 1];
                        srcPos += Lz77CompressedUnitSize;

                        int length = (byte0 >> GbaConstants.NibbleShift) + Lz77MinMatchLength;
                        int offset = (((byte0 & GbaConstants.NibbleMask) << GbaConstants.BitsPerByte) | byte1) + 1;
                        int copySrc = dstPos - offset;

                        for (int j = 0; j < length; j++)
                        {
                            if (dstPos >= decompressedSize) break;

                            result[dstPos++] = result[copySrc++];
                        }
                    }
                    else
                    {
                        result[dstPos++] = romData[srcPos++];
                    }
                }
            }

            return result;
        }

        public static byte[] CompressLZ77(byte[] imageData)
        {
            int length = imageData.Length;
            var result = new List<byte>(length / GbaConstants.PixelsPerByte4Bpp);

            result.Add((byte)Lz77HeaderIdentifier);
            for (int i = 0; i < Lz77HeaderSize - 1; i++)
            {
                result.Add((byte)((length >> (i * GbaConstants.BitsPerByte)) & GbaConstants.Mask8Bits));
            }

            int pos = 0;
            while (pos < length)
            {
                int flagPos = result.Count;
                byte flag = 0;
                result.Add(0);

                for (int i = 0; i < GbaConstants.BitsPerByte; i++)
                {
                    if (pos >= length) break;

                    var (bestDistance, bestLength) = FindLongestMatch(imageData, pos);

                    if (bestLength >= Lz77MinMatchLength)
                    {
                        flag |= (byte)(1 << (GbaConstants.BitsPerByte - 1 - i));

                        int offsetVal = bestDistance - 1;
                        int lenVal = bestLength - Lz77MinMatchLength;

                        result.Add((byte)(
                            ((lenVal & GbaConstants.NibbleMask) << GbaConstants.NibbleShift) |
                            ((offsetVal >> GbaConstants.BitsPerByte) & GbaConstants.NibbleMask)));
                        result.Add((byte)(offsetVal & GbaConstants.Mask8Bits));

                        pos += bestLength;
                    }
                    else
                    {
                        result.Add(imageData[pos++]);
                    }
                }

                result[flagPos] = flag;
            }

            while (result.Count % GbaConstants.PtrSize != 0)
            {
                result.Add(0);
            }

            return result.ToArray();
        }

        private static (int distance, int length) FindLongestMatch(byte[] data, int pos)
        {
            int maxDist = Math.Min(pos, Lz77MaxDistance);
            int maxLen = Math.Min(data.Length - pos, Lz77MaxLength);

            if (maxDist < Lz77MinSafeDistance || maxLen < Lz77MinMatchLength)
            {
                return (0, 0);
            }

            int bestLength = 0;
            int bestDistance = 0;

            for (int dist = Lz77MinSafeDistance; dist <= maxDist; dist++)
            {
                int len = 0;
                while (len < maxLen && data[pos - dist + len] == data[pos + len])
                {
                    len++;
                }

                if (len > bestLength)
                {
                    bestLength = len;
                    bestDistance = dist;

                    if (bestLength == Lz77MaxLength) break;
                }
            }

            return (bestDistance, bestLength);
        }

        public static Color[] DecompressPalette(byte[] romData, uint address, bool isCompressed)
        {
            byte[] paletteData;
            if (isCompressed)
            {
                paletteData = DecompressLZ77(romData, address);
            }
            else
            {
                paletteData = new byte[GbaConstants.PalColorCount * GbaConstants.BytesPerColor];
                Array.Copy(romData, address, paletteData, 0, paletteData.Length);
            }

            var colors = new Color[GbaConstants.PalColorCount];
            for (int i = 0; i < GbaConstants.PalColorCount; i++)
            {
                int byteIndex = i * GbaConstants.BytesPerColor;
                if (byteIndex + 1 >= paletteData.Length) break;

                int temp = (paletteData[byteIndex + 1] << GbaConstants.BitsPerByte) | paletteData[byteIndex];

                int r = ((temp & GbaConstants.RedMask) >> GbaConstants.RedShift) * GbaConstants.ColorChannelMulti;
                int g = ((temp & GbaConstants.GreenMask) >> GbaConstants.GreenShift) * GbaConstants.ColorChannelMulti;
                int b = ((temp & GbaConstants.BlueMask) >> GbaConstants.BlueShift) * GbaConstants.ColorChannelMulti;

                colors[i] = Color.FromArgb(255, r, g, b);
            }

            return colors;
        }

        public static byte[] CompressPalette(Color[] colors, bool isCompressed)
        {
            var paletteData = new byte[GbaConstants.PalColorCount * GbaConstants.BytesPerColor];
            int count = Math.Min(GbaConstants.PalColorCount, colors.Length);

            for (int i = 0; i < count; i++)
            {
                Color c = colors[i];
                int r = c.R / GbaConstants.ColorChannelMulti;
                int g = c.G / GbaConstants.ColorChannelMulti;
                int b = c.B / GbaConstants.ColorChannelMulti;

                ushort gbaColor = (ushort)((b << GbaConstants.BlueShift) | (g << GbaConstants.GreenShift) | (r << GbaConstants.RedShift));
                paletteData[i * GbaConstants.BytesPerColor] = (byte)(gbaColor & GbaConstants.Mask8Bits);
                paletteData[i * GbaConstants.BytesPerColor + 1] = (byte)((gbaColor >> GbaConstants.BitsPerByte) & GbaConstants.Mask8Bits);
            }

            return isCompressed ? CompressLZ77(paletteData) : paletteData;
        }

        public static Bitmap CreateSprite(byte[] imageData, Color[] palette, int width, int height, bool showBackColor)
        {
            var bmp = new Bitmap(width, height, PixelFormat.Format4bppIndexed);

            ColorPalette bmpPalette = bmp.Palette;
            int paletteCount = Math.Min(palette.Length, GbaConstants.PalColorCount);
            for (int i = 0; i < paletteCount; i++)
            {
                Color c = palette[i];
                bmpPalette.Entries[i] = (i == 0 && !showBackColor)
                    ? Color.FromArgb(0, c.R, c.G, c.B)
                    : Color.FromArgb(255, c.R, c.G, c.B);
            }
            for (int i = paletteCount; i < GbaConstants.PalColorCount; i++)
            {
                bmpPalette.Entries[i] = Color.Black;
            }
            bmp.Palette = bmpPalette;

            BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format4bppIndexed);
            var pixels = new byte[bmpData.Stride * height];
            int dataIndex = 0;

            for (int yTile = 0; yTile < height; yTile += GbaConstants.TileSize)
            {
                for (int xTile = 0; xTile < width; xTile += GbaConstants.TileSize)
                {
                    for (int yPixel = 0; yPixel < GbaConstants.TileSize; yPixel++)
                    {
                        for (int xPixel = 0; xPixel < GbaConstants.TileSize; xPixel += GbaConstants.PixelsPerByte4Bpp)
                        {
                            if (dataIndex >= imageData.Length) break;

                            byte temp = imageData[dataIndex++];
                            int leftIndex = temp & GbaConstants.NibbleMask;
                            int rightIndex = (temp >> GbaConstants.NibbleShift) & GbaConstants.NibbleMask;

                            int byteIndex = (yTile + yPixel) * bmpData.Stride + ((xTile + xPixel) / GbaConstants.PixelsPerByte4Bpp);
                            pixels[byteIndex] = (byte)((leftIndex << GbaConstants.Bpp4) | rightIndex);
                        }
                    }
                }
            }

            Marshal.Copy(pixels, 0, bmpData.Scan0, pixels.Length);
            bmp.UnlockBits(bmpData);
            return bmp;
        }

        public static bool ExtractImageAndPalette(
            Bitmap bmp,
            int expectedWidth,
            int expectedHeight,
            out byte[] imageData,
            out Color[] palette)
        {
            imageData = null;
            palette = null;

            if (bmp.Width != expectedWidth || bmp.Height != expectedHeight)
            {
                MessageBox.Show(
                    $"画像サイズは {expectedWidth}x{expectedHeight} である必要があります。",
                    "",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return false;
            }

            if (bmp.PixelFormat != PixelFormat.Format4bppIndexed)
            {
                MessageBox.Show(
                    "4bpp(16色)のインデックスカラー画像を使用してください。",
                    "",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return false;
            }

            ColorPalette pal = bmp.Palette;
            var colors = new Color[GbaConstants.PalColorCount];
            for (int i = 0; i < GbaConstants.PalColorCount; i++)
            {
                colors[i] = (i < pal.Entries.Length)
                    ? Color.FromArgb(255, pal.Entries[i].R, pal.Entries[i].G, pal.Entries[i].B)
                    : Color.FromArgb(255, 0, 0, 0);
            }
            palette = colors;

            BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, PixelFormat.Format4bppIndexed);
            var pixels = new byte[bmpData.Stride * bmp.Height];
            Marshal.Copy(bmpData.Scan0, pixels, 0, pixels.Length);
            bmp.UnlockBits(bmpData);

            var dataList = new List<byte>();
            for (int yTile = 0; yTile < expectedHeight; yTile += GbaConstants.TileSize)
            {
                for (int xTile = 0; xTile < expectedWidth; xTile += GbaConstants.TileSize)
                {
                    for (int yPixel = 0; yPixel < GbaConstants.TileSize; yPixel++)
                    {
                        for (int xPixel = 0; xPixel < GbaConstants.TileSize; xPixel += GbaConstants.PixelsPerByte4Bpp)
                        {
                            int byteIndex = (yTile + yPixel) * bmpData.Stride + ((xTile + xPixel) / GbaConstants.PixelsPerByte4Bpp);
                            byte pixelByte = pixels[byteIndex];

                            int p1 = (pixelByte >> GbaConstants.Bpp4) & GbaConstants.NibbleMask;
                            int p2 = pixelByte & GbaConstants.NibbleMask;
                            dataList.Add((byte)((p2 << GbaConstants.NibbleShift) | p1));
                        }
                    }
                }
            }

            imageData = dataList.ToArray();
            return true;
        }

        public static void ExportIndexedImage(Bitmap bmp, string filePath)
        {
            if (bmp == null) return;

            using (var exportBmp = (Bitmap)bmp.Clone())
            {
                ColorPalette pal = exportBmp.Palette;
                for (int i = 0; i < pal.Entries.Length; i++)
                {
                    Color e = pal.Entries[i];
                    pal.Entries[i] = Color.FromArgb(255, e.R, e.G, e.B);
                }
                exportBmp.Palette = pal;

                var format = Path.GetExtension(filePath).ToLower() == ".bmp"
                    ? ImageFormat.Bmp
                    : ImageFormat.Png;
                exportBmp.Save(filePath, format);
            }
        }

        public static Bitmap ScalePixelArt(Bitmap originalBmp, int scaleFactor = GbaConstants.DefaultScale)
        {
            int newWidth = originalBmp.Width * scaleFactor;
            int newHeight = originalBmp.Height * scaleFactor;
            var scaledBmp = new Bitmap(newWidth, newHeight);

            using (Graphics g = Graphics.FromImage(scaledBmp))
            {
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                g.PixelOffsetMode = PixelOffsetMode.Half;
                g.DrawImage(originalBmp, new Rectangle(0, 0, newWidth, newHeight));
            }

            return scaledBmp;
        }

        public static Bitmap[] CreatePokemonIconFrames(byte[] imageData, Color[] palette, bool showBackColor)
        {
            var frames = new Bitmap[GbaConstants.IconFrameCounts];

            for (int i = 0; i < GbaConstants.IconFrameCounts; i++)
            {
                var frameData = new byte[GbaConstants.IconBytesPerFrame];
                Array.Copy(imageData, i * GbaConstants.IconBytesPerFrame, frameData, 0, GbaConstants.IconBytesPerFrame);
                frames[i] = CreateSprite(frameData, palette, GbaConstants.IconFrameSize, GbaConstants.IconFrameSize, showBackColor);
            }

            return frames;
        }

        public class PokemonIconAnimator
        {
            private readonly Timer _animTimer;
            private readonly PictureBox _targetBox;
            private Bitmap[] _frames;
            private int _currentFrame;

            public PokemonIconAnimator(PictureBox targetPictureBox, int intervalMs = 416)
            {
                _targetBox = targetPictureBox;
                _animTimer = new Timer { Interval = intervalMs };
                _animTimer.Tick += AnimTimer_Tick;
            }

            public void SetFrames(Bitmap[] newFrames)
            {
                _frames = newFrames;
                _currentFrame = 0;

                if (_frames != null && _frames.Length > 0)
                {
                    _targetBox.Image = _frames[0];
                }
            }

            public void StartAnimation()
            {
                if (_frames != null && _frames.Length == 2)
                {
                    _animTimer.Start();
                }
            }

            public void StopAnimation()
            {
                _animTimer.Stop();

                if (_frames != null && _frames.Length > 0)
                {
                    _currentFrame = 0;
                    _targetBox.Image = _frames[0];
                }
            }

            private void AnimTimer_Tick(object sender, EventArgs e)
            {
                if (_frames == null || _frames.Length < 2) return;

                _currentFrame = (_currentFrame + 1) % _frames.Length;
                _targetBox.Image = _frames[_currentFrame];
            }
        }

        public static Bitmap DecodeFootprint(byte[] bits, Color[] palette)
        {
            var bmp = new Bitmap(GbaConstants.FootprintSize, GbaConstants.FootprintSize, PixelFormat.Format32bppArgb);
            Color color0 = palette[0];
            Color color1 = palette[1];

            var rect = new Rectangle(0, 0, GbaConstants.FootprintSize, GbaConstants.FootprintSize);
            BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.WriteOnly, bmp.PixelFormat);
            var rgbValues = new byte[Math.Abs(bmpData.Stride) * bmp.Height];

            int byteTrack = 0;
            for (int block = 0; block < GbaConstants.FootprintBlockCount; block++)
            {
                int startX = (block % GbaConstants.FootprintBlockDim) * GbaConstants.FootprintTileSize;
                int startY = (block / GbaConstants.FootprintBlockDim) * GbaConstants.FootprintTileSize;

                for (int y = 0; y < GbaConstants.FootprintTileSize; y++)
                {
                    if (byteTrack >= bits.Length) break;

                    byte currentByte = bits[byteTrack++];

                    for (int x = 0; x < GbaConstants.FootprintTileSize; x++)
                    {
                        Color pixelColor = (currentByte & (1 << x)) != 0 ? color1 : color0;
                        int index = ((startY + y) * bmpData.Stride) + ((startX + x) * GbaConstants.ArgbByteCount);

                        rgbValues[index + 0] = pixelColor.B;
                        rgbValues[index + 1] = pixelColor.G;
                        rgbValues[index + 2] = pixelColor.R;
                        rgbValues[index + 3] = pixelColor.A;
                    }
                }
            }

            Marshal.Copy(rgbValues, 0, bmpData.Scan0, rgbValues.Length);
            bmp.UnlockBits(bmpData);
            return bmp;
        }

        public static bool ExtractFootprint(Bitmap bmp, out byte[] footprintData)
        {
            footprintData = null;

            if (bmp.Width != GbaConstants.FootprintSize || bmp.Height != GbaConstants.FootprintSize)
            {
                MessageBox.Show(
                    $"画像サイズは {GbaConstants.FootprintSize}x{GbaConstants.FootprintSize} である必要があります。",
                    "",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return false;
            }

            if (bmp.PixelFormat != PixelFormat.Format1bppIndexed)
            {
                MessageBox.Show(
                    "1bpp(2色)のインデックスカラー画像を使用してください。",
                    "",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return false;
            }

            BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, PixelFormat.Format1bppIndexed);
            int stride = Math.Abs(bmpData.Stride);
            var pixels = new byte[stride * bmp.Height];
            Marshal.Copy(bmpData.Scan0, pixels, 0, pixels.Length);
            bmp.UnlockBits(bmpData);

            var data = new byte[GbaConstants.FootprintDataSize];

            for (int yBlock = 0; yBlock < GbaConstants.FootprintBlockDim; yBlock++)
            {
                for (int xBlock = 0; xBlock < GbaConstants.FootprintBlockDim; xBlock++)
                {
                    int startX = xBlock * GbaConstants.FootprintTileSize;
                    int startY = yBlock * GbaConstants.FootprintTileSize;
                    int blockIndex = yBlock * GbaConstants.FootprintBlockDim + xBlock;
                    int baseByteIdx = blockIndex * GbaConstants.FootprintTileSize;

                    for (int y = 0; y < GbaConstants.FootprintTileSize; y++)
                    {
                        byte byteValue = 0;
                        int rowOffset = (startY + y) * stride;

                        for (int x = 0; x < GbaConstants.FootprintTileSize; x++)
                        {
                            int imgX = startX + x;
                            int bitIndex = (GbaConstants.BitsPerByte - 1) - (imgX % GbaConstants.BitsPerByte);
                            bool isSet = (pixels[rowOffset + (imgX / GbaConstants.BitsPerByte)] & (1 << bitIndex)) != 0;

                            if (isSet)
                            {
                                byteValue |= (byte)(1 << x);
                            }
                        }
                        data[baseByteIdx + y] = byteValue;
                    }
                }
            }

            footprintData = data;
            return true;
        }

        public static Bitmap ConvertFootprintToBitmap(byte[] footPrintData)
        {
            if (footPrintData == null) return null;

            var bmp = new Bitmap(GbaConstants.FootprintSize, GbaConstants.FootprintSize, PixelFormat.Format1bppIndexed);
            ColorPalette pal = bmp.Palette;
            pal.Entries[0] = Color.FromArgb(248, 248, 248);
            pal.Entries[1] = Color.FromArgb(0, 0, 0);
            bmp.Palette = pal;

            BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.WriteOnly, PixelFormat.Format1bppIndexed);
            try
            {
                int stride = Math.Abs(bmpData.Stride);
                var pixels = new byte[stride * bmp.Height];

                for (int yBlock = 0; yBlock < GbaConstants.FootprintBlockDim; yBlock++)
                {
                    for (int xBlock = 0; xBlock < GbaConstants.FootprintBlockDim; xBlock++)
                    {
                        int startX = xBlock * GbaConstants.FootprintTileSize;
                        int startY = yBlock * GbaConstants.FootprintTileSize;
                        int blockIndex = yBlock * GbaConstants.FootprintBlockDim + xBlock;
                        int baseByteIdx = blockIndex * GbaConstants.FootprintTileSize;

                        for (int y = 0; y < GbaConstants.FootprintTileSize; y++)
                        {
                            byte byteValue = footPrintData[baseByteIdx + y];
                            int rowOffset = (startY + y) * stride;

                            for (int x = 0; x < GbaConstants.FootprintTileSize; x++)
                            {
                                if ((byteValue & (1 << x)) == 0) continue;

                                int imgX = startX + x;
                                int bitIndex = (GbaConstants.BitsPerByte - 1) - (imgX % GbaConstants.BitsPerByte);
                                pixels[rowOffset + (imgX / GbaConstants.BitsPerByte)] |= (byte)(1 << bitIndex);
                            }
                        }
                    }
                }

                Marshal.Copy(pixels, 0, bmpData.Scan0, pixels.Length);
            }
            finally
            {
                bmp.UnlockBits(bmpData);
            }

            return bmp;
        }
    }
}
