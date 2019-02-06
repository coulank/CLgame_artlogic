using System.IO;
using System.IO.Compression;

namespace Coulank
{
    public class Compress
    {
        public static byte[] ByteCompress(byte[] inByte)
        {
            var ms = new MemoryStream();
            var compressedStream = new DeflateStream(ms,
            CompressionMode.Compress, true);
            compressedStream.Write(inByte, 0, inByte.Length);
            compressedStream.Close();
            byte[] outByte = ms.ToArray();
            ms.Close();
            return outByte;
        }
        public static byte[] ByteDecompress(byte[] compressByte)
        {
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
}