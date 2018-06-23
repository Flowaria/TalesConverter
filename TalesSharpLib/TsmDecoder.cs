using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalesSharp.TalesMusic;

namespace TalesSharp
{
    public class TsmDecoderProperties
    {
        public bool IsZipFile { get; set; }
        public bool IsEncoded { get; set; }
    }

    public partial class TsmDecoder
    {
        public const int TSM_SIZE = 512000;
        public const int TSM_LEFTOVER = 256;

        public int Start { get; set; } = 0;
        public int Pattern { get; set; } = 250;
        public int Password { get; set; }
        public int Salt { get; set; } = 3;

        public Dictionary<byte, byte>[] Swap { get; set; } = new Dictionary<byte, byte>[] { new Dictionary<byte, byte>(), new Dictionary<byte, byte>() };

        public TsmDecoderProperties Properties = null;

        public TsmDecoder()
        {

        }

        public byte[] Convert(byte[] buffer)
        {
            int _start = Start;
            int _pattern = Pattern;
            int _password = Password;
            int _salt = Salt;

            byte[] result = new byte[buffer.Length];
            buffer.CopyTo(result, 0);

            for (int j = Start; j < TSM_SIZE; j += Pattern)
            {
                if (j < result.Length)
                {
                    result[j] = (byte)(result[j] ^ _password);
                    _password += _salt;
                    _password %= TSM_LEFTOVER;
                }
                else
                {
                    break;
                }
            }

            return result;
        }
    }
}
