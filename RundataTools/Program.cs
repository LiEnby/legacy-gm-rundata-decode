using Ionic.Zlib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RundataTools
{
    class Program
    {
        static int ReadInt32(Stream str)
        {
            byte[] intBytes = new byte[4];
            str.Read(intBytes, 0, 4);
            return BitConverter.ToInt32(intBytes, 0);
        }
        static void WriteInt32(Stream str,int i)
        {
            byte[] intBytes = BitConverter.GetBytes(i);
            str.Write(intBytes, 0, 4);
        }

        static byte[] Compress(byte[] buffer)
        {
            MemoryStream ms = new MemoryStream();
            ZlibStream str = new ZlibStream(ms, CompressionMode.Compress, CompressionLevel.Level6, true);
            str.Write(buffer, 0x00, buffer.Length);
            str.Close();
            ms.Seek(0x00, SeekOrigin.Begin);
            return ms.ToArray();
        }
        static void Main(string[] args)
        {
            if(args[0] == "-d")
            {
                FileStream fs = File.OpenRead(args[1]);
                int BufferSz = ReadInt32(fs);
                byte[] RunDataBuffer = new byte[BufferSz];
                fs.Read(RunDataBuffer, 0x00, BufferSz);
                byte[] RunDataUncompressed = ZlibStream.UncompressBuffer(RunDataBuffer);
                File.WriteAllBytes(args[2], RunDataUncompressed);
            }
            if(args[0] == "-e")
            {
                byte[] RunDataUncompressed = File.ReadAllBytes(args[1]);
                FileStream fs = File.OpenWrite(args[2]);
                byte[] RunDataCompressed = Compress(RunDataUncompressed);
                WriteInt32(fs, RunDataCompressed.Length);
                fs.Write(RunDataCompressed, 0x00, RunDataCompressed.Length);
            }
        }
    }
}
