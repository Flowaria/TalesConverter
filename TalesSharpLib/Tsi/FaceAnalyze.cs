using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalesSharp.TalesImage
{
    public class FaceAnalyze
    {
        public string BaseAnalyzePath = Path.Combine(Path.GetTempPath(), "tsianalyze");
        public static void AnalyzeFolder(string path)
        {
            if(Directory.Exists(path))
            {
                var list = Directory.GetFiles(path);
                var lists = new List<string>();
                var dict = new Dictionary<string, int>();
                foreach(var i in list)
                {
                    if (Path.GetExtension(i) == ".png" || Path.GetExtension(i) == ".jpg")
                    {
                        using (Bitmap bmp = new Bitmap(i))
                        {
                            dict.Add(i, bmp.Width * bmp.Height);
                        }
                        lists.Add(Path.GetFileName(i));
                    }
                }
                var sortedDict = from entry in dict orderby entry.Value descending select entry;
                var sortedList = from entry in lists orderby entry descending select entry;
            }
        }

        public static int GetDictionaryKeyIndex(Dictionary<string, Int64> dict, string key)
        {
            int index = -1;
            foreach (String value in dict.Keys)
            {
                index++;
                if (key == value)
                    return index;
            }
            return -1;
        }
    }
}
