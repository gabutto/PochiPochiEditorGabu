using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using PochiPochiEditorGabu.Constants;

namespace PochiPochiEditorGabu.FileReaders
{
    public class TblFileReader
    {
        private readonly ByteTrieNode _byteTrieRoot;
        private readonly StringTrieNode _stringTrieRoot;

        private const byte NewlineByte = 0xFE;
        private const byte TerminatorByte = 0xFF;

        public TblFileReader(string filePath)
        {
            _byteTrieRoot = new ByteTrieNode();
            _stringTrieRoot = new StringTrieNode();

            foreach (string line in File.ReadLines(filePath, Encoding.UTF8))
            {
                if (string.IsNullOrEmpty(line) || line.StartsWith(";")) continue;

                string[] parts = line.Split('=');
                string hexKey = parts[0].Replace(" ", "");
                string value = parts[1];

                int byteLen = hexKey.Length / 2;
                byte[] bytes = new byte[byteLen];
                for (int i = 0; i < byteLen; i++)
                {
                    bytes[i] = Convert.ToByte(hexKey.Substring(i * 2, 2), 16);
                }

                ByteTrieNode currentByteNode = _byteTrieRoot;
                foreach (byte b in bytes)
                {
                    if (!currentByteNode.Children.TryGetValue(b, out ByteTrieNode next))
                    {
                        next = new ByteTrieNode();
                        currentByteNode.Children[b] = next;
                    }
                    currentByteNode = next;
                }
                currentByteNode.Value = value;
                currentByteNode.IsTerminal = true;

                if (!string.IsNullOrEmpty(value))
                {
                    StringTrieNode currentStrNode = _stringTrieRoot;
                    foreach (char c in value)
                    {
                        if (!currentStrNode.Children.TryGetValue(c, out StringTrieNode next))
                        {
                            next = new StringTrieNode();
                            currentStrNode.Children[c] = next;
                        }
                        currentStrNode = next;
                    }
                    currentStrNode.Value = bytes;
                    currentStrNode.IsTerminal = true;
                }
            }
        }

        public string BytesToString(byte[] bytes, int offset, int maxLength)
        {
            if (bytes == null) return string.Empty;

            StringBuilder result = new StringBuilder();
            int length = Math.Min(bytes.Length - offset, maxLength);
            int i = 0;

            while (i < length)
            {
                int currentIdx = offset + i;
                byte currentByte = bytes[currentIdx];

                // Terminator
                if (currentByte == TerminatorByte)
                {
                    break;
                }

                // Newline
                if (currentByte == NewlineByte)
                {
                    result.Append(Environment.NewLine);
                    i++;
                    continue;
                }

                int matchLength = 0;
                string matchedString = null;
                ByteTrieNode currentNode = _byteTrieRoot;

                for (int j = 0; j < length - i; j++)
                {
                    byte b = bytes[currentIdx + j];
                    if (currentNode.Children.TryGetValue(b, out ByteTrieNode next))
                    {
                        currentNode = next;
                        if (currentNode.IsTerminal)
                        {
                            matchLength = j + 1;
                            matchedString = currentNode.Value;
                        }
                    }
                    else
                    {
                        break;
                    }
                }

                if (matchLength > 0 && matchedString != null)
                {
                    result.Append(matchedString);
                    i += matchLength;
                }
                else
                {
                    // Ignore
                    i++;
                }
            }

            return result.ToString();
        }

        public byte[] StringToBytes(
            string text,
            bool appendTerminator,
            int targetLength = -1, 
            byte paddingByte = GbaConstants.PaddingByte)
        {
            List<byte> result = new List<byte>();
            if (string.IsNullOrEmpty(text))
            {
                text = string.Empty;
            }

            int i = 0;
            while (i < text.Length)
            {
                // Newline
                if (text[i] == '\r' && text[i + 1] == '\n')
                {
                    i += 2;
                    continue;
                }

                int matchLength = 0;
                byte[] matchedBytes = null;
                StringTrieNode currentNode = _stringTrieRoot;

                for (int j = i; j < text.Length; j++)
                {
                    char c = text[j];
                    if (currentNode.Children.TryGetValue(c, out StringTrieNode next))
                    {
                        currentNode = next;
                        if (currentNode.IsTerminal)
                        {
                            matchLength = j - i + 1;
                            matchedBytes = currentNode.Value;
                        }
                    }
                    else
                    {
                        break;
                    }
                }

                if (matchLength > 0 && matchedBytes != null)
                {
                    result.AddRange(matchedBytes);
                    i += matchLength;
                }
                else
                {
                    // Ignore
                    i++;
                }
            }

            // Terminator
            if (appendTerminator)
            {
                result.Add(TerminatorByte);
            }

            // Padding
            if (targetLength > 0)
            {
                while (result.Count < targetLength)
                    result.Add(paddingByte);
            }

            return result.ToArray();
        }

        private class ByteTrieNode
        {
            public Dictionary<byte, ByteTrieNode> Children { get; } = new Dictionary<byte, ByteTrieNode>();
            public string Value { get; set; }
            public bool IsTerminal { get; set; }
        }

        private class StringTrieNode
        {
            public Dictionary<char, StringTrieNode> Children { get; } = new Dictionary<char, StringTrieNode>();
            public byte[] Value { get; set; }
            public bool IsTerminal { get; set; }
        }
    }
}
