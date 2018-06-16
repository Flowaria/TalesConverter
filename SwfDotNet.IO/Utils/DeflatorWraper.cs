using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwfDotNet.IO.Utils
{
    public static class DeflatorWraper
    {
        public static byte[] Decompress(byte[] Bytes)
        {
            using (var result = new MemoryStream())
            {
                using (var ms = new MemoryStream(Bytes))
                {
                    using (DeflateStream defstream = new DeflateStream(ms, CompressionMode.Decompress))
                    {
                        defstream.CopyTo(result);
                    }

                }
                return result.ToArray();
            }
        }
        public static byte[] Compress(byte[] Bytes)
        {
            using (var result = new MemoryStream())
            {
                using (var ms = new MemoryStream(Bytes))
                {
                    using (DeflateStream defstream = new DeflateStream(ms, CompressionMode.Compress))
                    {
                        defstream.CopyTo(result);
                    }
                }
                return result.ToArray();
            }
        }
    }
}
