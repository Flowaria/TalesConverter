using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalesConverter
{
    public class LiteConverter
    {
        Process converter = new Process();
        ProcessStartInfo info = new ProcessStartInfo();

        string parm = "-output {0} {1}";

        private LiteConverter(string dir, List<string> tsmFiles)
        {
            info.FileName = "LiteConverter.exe";
            info.Arguments = String.Format(parm, dir, '"'+String.Join("\" \"",tsmFiles.ToArray())+'"');
            converter.StartInfo = info;
        }

        public static void Convert(string output, List<string> tsmFiles)
        {
            new LiteConverter(output, tsmFiles);
        }
    }
}
