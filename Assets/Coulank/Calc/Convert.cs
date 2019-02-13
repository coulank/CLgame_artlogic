using System.IO;
using System.IO.Compression;
using System;
using System.Text.RegularExpressions;

namespace Coulank.Convert
{

    public class Compress
    {
        public static byte[] ByteCompress(byte[] inByte, bool strict = false)
        {
            if ((inByte.Length == 0) && !strict) return inByte;
            var ms = new MemoryStream();
            var compressedStream = new DeflateStream(ms,
            CompressionMode.Compress, true);
            compressedStream.Write(inByte, 0, inByte.Length);
            compressedStream.Close();
            byte[] outByte = ms.ToArray();
            ms.Close();
            return outByte;
        }
        public static byte[] ByteDecompress(byte[] compressByte, bool strict = false)
        {
            if ((compressByte.Length == 0) && !strict) return compressByte;
            byte[] buffer = new byte[1024];
            var ms = new MemoryStream(compressByte);
            MemoryStream outstream = new MemoryStream();
            DeflateStream decompressStream = new DeflateStream(ms, CompressionMode.Decompress);
            while (true)
            {
                int readSize = decompressStream.Read(buffer, 0, buffer.Length);
                if (readSize == 0) break;
                outstream.Write(buffer, 0, readSize);
            }
            decompressStream.Close();
            ms.Close();
            byte[] outByte = outstream.ToArray();
            outstream.Close();
            return outByte;
        }
    }
    public class Cast {
        /// <summary>
        /// Bools To Bytes
        /// </summary>
        /// <param name="bools">bool[]</param>
        /// <param name="shifted">Register bit shift</param>
        /// <param name="frequency">1/8 times param</param>
        public static byte[] Bool2Byte(bool[] bools, int shifted = 0, int frequency = 1)
        {
            int byteLength = (int)Math.Floor((double)(((bools.Length - 1) * frequency + shifted) / 8)) + 1;
            byte[] bytes = new byte[byteLength];
            for (int i = 0; i < bools.Length; i++)
            {
                int p_byte = (int)Math.Floor((double)((i * frequency + shifted) / 8));
                int p_bool = (i * frequency + shifted) % 8;
                bytes[p_byte] |= (byte)(bools[i] ? (1 << p_bool) : 0);
            }
            return bytes;
        }
        /// <summary>
        /// Bytes To Bools
        /// </summary>
        /// <param name="bytes">byte[]</param>
        /// <param name="shifted">Register bit shift</param>
        /// <param name="frequency">1/8 times param</param>
        public static bool[] Byte2Bool(byte[] bytes, int shifted = 0, int frequency = 1)
        {
            int boolLength = 8 * bytes.Length * frequency + shifted;
            bool[] bools = new bool[boolLength];
            for (int i = 0; i < bytes.Length; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    int p_bool = (i * 8 + j) * frequency + shifted;
                    bools[p_bool] = (bytes[i] & (1 << j)) > 0;
                }
            }
            return bools;
        }
        public static string Byte2Str(byte[] bytes, 
            EStringByte eStrByte = EStringByte.Hex, string separator = "")
        {
            switch (eStrByte)
            {
                case EStringByte.Base64:
                    return System.Convert.ToBase64String(bytes);
                default:
                    string[] hexes = new string[bytes.Length];
                    for (int i = 0; i < bytes.Length; i++)
                    {
                        hexes[i] = bytes[i].ToString("X2");
                    }
                    if (separator == "")
                        return string.Concat(hexes);
                    else
                        return string.Join(separator, hexes);
            }
        }
        public static byte[] Str2Byte(string str,
            EStringByte eStrByte = EStringByte.Hex, string separator = "")
        {
            switch (eStrByte)
            {
                case EStringByte.Base64:
                    return System.Convert.FromBase64String(str);
                default:
                    byte[] bytes;
                    if (separator == "")
                    {
                        MatchCollection matches = Regex.Matches(str, @"[\dA-Fa-f][\dA-Fa-f]?");
                        bytes = new byte[matches.Count];
                        for (int i = 0; i < bytes.Length; i++)
                        {
                            bytes[i] = byte.Parse(matches[i].Value, System.Globalization.NumberStyles.HexNumber);
                        }
                    }
                    else
                    {
                        string[] hexes = str.Split(separator.ToCharArray());
                        bytes = new byte[hexes.Length];
                        for (int i = 0; i < bytes.Length; i++)
                        {
                            bytes[i] = byte.Parse(hexes[i], System.Globalization.NumberStyles.HexNumber);
                        }
                    }
                    return bytes;
            }
        }
    }
}