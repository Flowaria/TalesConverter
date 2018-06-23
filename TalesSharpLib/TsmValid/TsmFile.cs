using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalesSharp.TsmValid
{
    public enum TsmType
    {
        Music, Zip, NEMusic, NEZip
    }

    public class TsmFile
    {
        public static TsmType? CheckType(string filename)
        {
            if (!File.Exists(filename))
                return null;

            byte[] content = File.ReadAllBytes(filename);

            if (content[1] == 75 && content[2] == 3 && content[3] == 4)
                return (content[0] != 80)?TsmType.Zip:TsmType.NEZip;

            //Clean-up Files ID3, PK3
            using (var fs = File.OpenRead(filename))
            {
                var a = fs.ReadByte();
                var b = fs.ReadByte();
                var c = fs.ReadByte();

                if ((b == 68 && c == 51) || (b == 75 && c == 51))
                {
                    while ((fs.Length - fs.Position) > 2)
                    {
                        if ((a = fs.ReadByte()) == 255 && (b = fs.ReadByte()) == 251)
                        {
                            fs.Position = 0;
                            a = fs.ReadByte();
                            b = fs.ReadByte();
                            c = fs.ReadByte();
                            if(b == 68)
                                return (a != 73) ? TsmType.Music : TsmType.NEMusic;
                            if(b == 75)
                                return (a != 80) ? TsmType.Music : TsmType.NEMusic;
                        }
                    }
                }
            }
            return null;
        }

        public static byte[] TryDecode(string filename)
        {
            var t = CheckType(filename);
            if(t.HasValue && t.Value == TsmType.Music)
            {
                byte[] content = File.ReadAllBytes(filename);
                var da = content[0] ^ TsmDecoder.V1.Password;
                if (da == 80 || da == 73)
                {
                    return TsmDecoder.V1.Convert(content);
                }
                else
                {
                    return TsmDecoder.V2.Convert(content);
                }
            }
            return null;
        }
    }
}
