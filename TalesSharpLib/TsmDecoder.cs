using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalesSharp.TalesMusic;

namespace TalesSharp
{
    public class TsmDecoder
    {
        public const int TSM_SIZE = 512000;
        public const int TSM_LEFTOVER = 256;

        public int Start { get; set; } = 0;
        public int Pattern { get; set; } = 250;
        public int Password { get; set; }
        public int Salt { get; set; }

        public bool IsZipFile { get; set; }
        public bool IsEncoded { get; set; }

        public TsmDecoder()
        {

        }

        public byte[] Convert(byte[] buffer)
        {
            if(IsEncoded)
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

                TsmDecoderData.Swap(ref result, 2);
                return result;
            }
            else
            {
                return buffer;
            }
        }

        public static TsmDecoder DecodeFile(string tsmfile, out byte[] result, bool try_analyze = false)
        {
            if(File.Exists(tsmfile))
            {
                string filename = Path.GetFileNameWithoutExtension(tsmfile);
                using (var s = File.OpenRead(tsmfile))
                {
                    var buffer = File.ReadAllBytes(tsmfile);
                    result = new byte[buffer.Length];

                    byte[] head = new byte[4];
                    s.Read(head, 0, 4);
                    var decoder = TsmDecoderData.Read(head);
                    if (decoder != null)
                    {
                        decoder.Convert(buffer).CopyTo(result, 0);
                        return decoder;
                    }
                    else if (try_analyze)
                    {
                        if(buffer[1] == 251) //mp3
                        {
                            if (buffer[0] == 251)
                            {

                            }
                            else
                            {
                                byte key = 0;
                                for (byte i = 0; i <= 255; i++)
                                {
                                    if (251 == (buffer[0] ^ i)) key = i;
                                }
                                if (key != 0)
                                {
                                    byte[] xorbuffer = new byte[buffer.Length];
                                    buffer.CopyTo(xorbuffer, 0);
                                    var xordec = new TsmDecoder();
                                    xordec.IsEncoded = true;
                                    xordec.IsZipFile = false;
                                    xordec.Start = 0;
                                    xordec.Pattern = 250;
                                    xordec.Password = key;
                                    xordec.Salt = 3;

                                    string tempfile = Path.GetTempFileName();
                                    //File.WriteAllBytes(tempfile, xordec.Convert(xorbuffer));

                                    Console.WriteLine("DODO");
                                    var reader = MP3Reader.ReadBuffer(xordec.Convert(xorbuffer));
                                    if (reader.IsValidFile())
                                    {
                                        Console.WriteLine("Yay");
                                    }
                                }
                            }
                               
                        }
                        if (buffer[1] == 75) //zip
                        {

                        }
                        
                    }
                }
            }
            result = null;
            return null;
        }
    }
}
