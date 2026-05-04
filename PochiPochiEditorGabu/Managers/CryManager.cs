using System;
using System.Collections.Generic;
using System.IO;
using System.Media;
using System.Text;
using System.Windows.Forms;

using PochiPochiEditorGabu.Constants;

namespace PochiPochiEditorGabu.Managers
{
    public class Cry
    {
        public bool Compressed;
        public bool Looped;
        public int SampleRate;
        public int LoopStart;
        public int Size;
        public sbyte[] Data;
    }

    public class CryManager
    {
        private const int CryCompressedFlag = 0x1;
        private const int CryLoopedFlag = 0x4000;
        private const int CrySampleRateShift = 10;
        private const int CrySizeAdjustment = 1;
        private const int CryBlockCompressedDataSize = 0x20;
        private const int WavFmtChunkSize = 16;
        private const ushort WavFormatPcm = 1;
        private const ushort WavMonoChannels = 1;
        private const ushort WavBlockAlign = 1;
        private const ushort WavBitsPerSample = 8;
        private const int WavRiffSizeOffset = 4;
        private const int WavRiffHeaderSize = 8;
        private const int NibblesPerByte = 2;
        private const int BlockSampleCount = CryBlockCompressedDataSize * NibblesPerByte;
        private const int BlockTotalSize = CryBlockCompressedDataSize + 1;

        public Cry LoadCryFromAddress(uint cryAddress, byte[] romData)
        {
            var cry = new Cry();

            cry.Compressed = BitConverter.ToInt16(romData, (int)cryAddress) == CryCompressedFlag;
            cry.Looped = BitConverter.ToInt16(romData, (int)cryAddress + 2) == CryLoopedFlag;
            cry.SampleRate = BitConverter.ToInt32(romData, (int)cryAddress + 4) >> CrySampleRateShift;
            cry.LoopStart = BitConverter.ToInt32(romData, (int)cryAddress + 8);
            cry.Size = BitConverter.ToInt32(romData, (int)cryAddress + 12) + CrySizeAdjustment;
            cry.Data = DecompressCryData(romData, (int)cryAddress + 16, cry.Size);

            return cry;
        }

        public sbyte[] DecompressCryData(byte[] romData, int startOffset, int expectedSize)
        {
            sbyte[] lookup = { 0, 1, 4, 9, 16, 25, 36, 49, -64, -49, -36, -25, -16, -9, -4, -1 };
            var data = new List<sbyte>(expectedSize);
            int offset = startOffset;
            int alignment = 0;
            sbyte pcmLevel = 0;

            while (data.Count < expectedSize)
            {
                if (alignment == 0)
                {
                    if (offset < romData.Length)
                    {
                        byte byteValue = romData[offset++];
                        pcmLevel = (sbyte)(byteValue <= 127 ? byteValue : byteValue - 256);
                        data.Add(pcmLevel);
                        alignment = CryBlockCompressedDataSize;
                    }
                    else
                    {
                        break;
                    }
                }

                if (offset >= romData.Length || data.Count >= expectedSize) break;

                byte input = romData[offset++];

                if (alignment < CryBlockCompressedDataSize)
                {
                    int upperNibble = input >> GbaConstants.NibbleShift;
                    pcmLevel = SafeAddSByte(pcmLevel, lookup[upperNibble]);
                    data.Add(pcmLevel);
                    if (data.Count >= expectedSize) break;
                }

                int lowerNibble = input & GbaConstants.NibbleMask;
                pcmLevel = SafeAddSByte(pcmLevel, lookup[lowerNibble]);
                data.Add(pcmLevel);

                alignment--;
            }

            return data.ToArray();
        }

        public sbyte SafeAddSByte(sbyte a, sbyte b)
        {
            int result = a + b;
            return (sbyte)(result > 127 ? 127 : (result < -128 ? -128 : result));
        }

        public byte[] CompressCryData(sbyte[] data)
        {
            sbyte[] lookup = { 0, 1, 4, 9, 16, 25, 36, 49, -64, -49, -36, -25, -16, -9, -4, -1 };

            int blockCount = (data.Length + (BlockSampleCount - 1)) / BlockSampleCount;
            int remainder = data.Length % BlockSampleCount;
            int lastBlockSize = remainder == 0 ? BlockTotalSize : 1 + (remainder / NibblesPerByte) + (remainder % NibblesPerByte);

            byte[][] blocks = new byte[blockCount][];

            for (int n = 0; n < blockCount; n++)
            {
                blocks[n] = new byte[n < blockCount - 1 ? BlockTotalSize : lastBlockSize];

                int i = n * BlockSampleCount;
                int k = 0;

                if (i < data.Length)
                {
                    blocks[n][k] = (byte)(data[i] & GbaConstants.Mask8Bits);
                }

                k++;

                sbyte pcm = i < data.Length ? data[i] : (sbyte)0;
                i++;

                int j = 1;
                while (j < BlockSampleCount && i < data.Length)
                {
                    sbyte sample = data[i++];
                    int diff = sample - pcm;

                    int lookupI = -1;
                    int bestDiff = int.MaxValue;

                    for (int x = 0; x < lookup.Length; x++)
                    {
                        int newPcm = pcm + lookup[x];
                        if (newPcm <= sbyte.MaxValue && newPcm >= sbyte.MinValue)
                        {
                            int currentDiff = Math.Abs(lookup[x] - diff);
                            if (currentDiff < bestDiff)
                            {
                                lookupI = x;
                                bestDiff = currentDiff;
                                if (bestDiff == 0) break;
                            }
                        }
                    }

                    if (j % NibblesPerByte == 0)
                    {
                        blocks[n][k] |= (byte)(lookupI << GbaConstants.NibbleShift);
                    }
                    else
                    {
                        blocks[n][k] |= (byte)lookupI;
                        k++;
                    }

                    pcm = (sbyte)(pcm + lookup[lookupI]);
                    j++;
                }
            }

            var result = new List<byte>(blockCount * BlockTotalSize);
            for (int n = 0; n < blockCount; n++)
            {
                result.AddRange(blocks[n]);
            }

            return result.ToArray();
        }

        public void EncodeToWavStream(Cry cry, Stream stream)
        {
            using (BinaryWriter writer = new BinaryWriter(stream, Encoding.ASCII, true))
            {
                // RIFF header
                writer.Write(Encoding.ASCII.GetBytes("RIFF"));
                writer.Write(0);
                writer.Write(Encoding.ASCII.GetBytes("WAVE"));

                // fmt chunk
                writer.Write(Encoding.ASCII.GetBytes("fmt "));
                writer.Write(WavFmtChunkSize);           // chunk size
                writer.Write(WavFormatPcm);              // pcm
                writer.Write(WavMonoChannels);           // mono
                writer.Write(cry.SampleRate);            // sample rate
                writer.Write(cry.SampleRate);            // byte rate
                writer.Write(WavBlockAlign);             // block align
                writer.Write(WavBitsPerSample);          // bits per sample

                // data chunk
                writer.Write(Encoding.ASCII.GetBytes("data"));
                writer.Write(cry.Data.Length);           // data size

                // unsigned 8-bit
                foreach (sbyte sample in cry.Data)
                {
                    writer.Write((byte)(sample + 128));
                }

                writer.Seek(WavRiffSizeOffset, SeekOrigin.Begin);
                writer.Write((int)(stream.Length - WavRiffHeaderSize));
            }
        }

        public void PlayCry(Cry cry)
        {
            using (var stream = new MemoryStream())
            {
                EncodeToWavStream(cry, stream);
                stream.Seek(0, SeekOrigin.Begin);
                using (var player = new SoundPlayer(stream))
                {
                    player.Play();
                }
            }
        }

        public Cry ConvertWavToCryData(string filename)
        {
            var cry = new Cry();

            using (BinaryReader reader = new BinaryReader(File.OpenRead(filename)))
            {
                Cry ShowErrorAndReturnNull(string msg)
                {
                    MessageBox.Show(msg, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return null;
                }

                if (Encoding.ASCII.GetString(reader.ReadBytes(4)) != "RIFF")
                    return ShowErrorAndReturnNull("WAVEファイルではありません");

                int fileSize = reader.ReadInt32();
                if (fileSize + 8 != reader.BaseStream.Length)
                    return ShowErrorAndReturnNull("ファイルサイズが不正です");

                if (Encoding.ASCII.GetString(reader.ReadBytes(4)) != "WAVE")
                    return ShowErrorAndReturnNull("WAVEファイルではありません");

                if (Encoding.ASCII.GetString(reader.ReadBytes(4)) != "fmt ")
                    return ShowErrorAndReturnNull("fmtチャンクが見つかりません");

                int fmtChunkSize = reader.ReadInt32();
                if (fmtChunkSize != 16)
                    return ShowErrorAndReturnNull("不正なfmtチャンクです");

                if (reader.ReadInt16() != 1)
                    return ShowErrorAndReturnNull("PCM形式のWAVEファイルのみ対応しています");

                if (reader.ReadInt16() != 1)
                    return ShowErrorAndReturnNull("モノラルのWAVEファイルのみ対応しています");

                cry.SampleRate = reader.ReadInt32();
                reader.ReadInt32(); // byte rate

                reader.ReadInt16(); // block align
                if (reader.ReadInt16() != 8)
                    return ShowErrorAndReturnNull("8ビットのWAVEファイルのみ対応しています");

                if (Encoding.ASCII.GetString(reader.ReadBytes(4)) != "data")
                    return ShowErrorAndReturnNull("dataチャンクが見つかりません");

                int dataSize = reader.ReadInt32();
                cry.Data = new sbyte[dataSize];

                byte[] rawData = reader.ReadBytes(dataSize);
                for (int i = 0; i < dataSize; i++)
                {
                    cry.Data[i] = (sbyte)(rawData[i] - 128);
                }
            }

            cry.Compressed = true;
            cry.Looped = false;
            cry.LoopStart = 0;
            cry.Size = cry.Data.Length;

            return cry;
        }

        public byte[] GetCryBinaryData(Cry cry)
        {
            byte[] compressedData = CompressCryData(cry.Data);

            ushort compressedFlag = (ushort)(cry.Compressed ? CryCompressedFlag : 0);
            ushort loopedFlag = (ushort)(cry.Looped ? CryLoopedFlag : 0);
            uint sampleRateValue = (uint)cry.SampleRate << CrySampleRateShift;
            uint sizeValue = (uint)(cry.Data.Length - CrySizeAdjustment);

            byte[] result = new byte[16 + compressedData.Length];
            int offset = 0;

            void Write16(ushort val)
            {
                result[offset++] = (byte)(val & GbaConstants.Mask8Bits);
                result[offset++] = (byte)(val >> GbaConstants.BitsPerByte);
            }

            void Write32(uint val)
            {
                result[offset++] = (byte)(val & GbaConstants.Mask8Bits);
                result[offset++] = (byte)((val >> GbaConstants.BitsPerByte) & GbaConstants.Mask8Bits);
                result[offset++] = (byte)((val >> (2 * GbaConstants.BitsPerByte)) & GbaConstants.Mask8Bits);
                result[offset++] = (byte)((val >> (3 * GbaConstants.BitsPerByte)) & GbaConstants.Mask8Bits);
            }

            Write16(compressedFlag);
            Write16(loopedFlag);
            Write32(sampleRateValue);
            Write32((uint)cry.LoopStart);
            Write32(sizeValue);

            Array.Copy(compressedData, 0, result, offset, compressedData.Length);
            return result;
        }
    }
}
