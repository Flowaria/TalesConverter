using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalesConverter.Thread
{
    public class TsmBundleWorker : BundleWorker
    {
        public bool AutoAnalyzeFile { get; set; }

        public override async Task Handle(List<string> tsmFiles)
        {
            await Task.Factory.StartNew(delegate
            {
                Parallel.ForEach(tsmFiles, new ParallelOptions { MaxDegreeOfParallelism = MaxThread }, i =>
                {
                    string filename = Path.GetFileNameWithoutExtension(i);

                    byte[] result = new byte[File.ReadAllBytes(i).Length];
                    var decoder = TsmDecoder.DecodeFile(i, out result, AutoAnalyzeFile);
                    if (decoder != null)
                    {
                        if (decoder.IsEncrypted)
                        {
                            string cfile = Path.Combine(SaveDirectory, filename + (decoder.IsZipFile ? ".zip" : ".mp3"));
                            File.WriteAllBytes(cfile, result);
                        }
                        else //rename
                        {
                            string cfile = Path.Combine(SaveDirectory, filename + (decoder.IsZipFile ? ".zip" : ".mp3"));
                            File.WriteAllBytes(cfile, result);
                        }
                    }
                });
            });
        }
    }
}
