using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TalesSharp;
using TalesSharp.TsmValid;

namespace TalesConverter.Thread
{
    public class TsmBundleWorker : BundleWorker
    {
        public bool AutoAnalyzeFile { get; set; }

        public override async Task Handle(List<string> tsmFiles)
        {
            await Task.Factory.StartNew(delegate
            {
                TotalResources = tsmFiles.Count;
                TotalCalculated();

                Parallel.ForEach(tsmFiles, new ParallelOptions { MaxDegreeOfParallelism = MaxThread }, i =>
                {
                    byte[] content = File.ReadAllBytes(i);
                    byte[] header = new byte[] { content[0], content[1], content[2] };
                    
                    byte[] result = new byte[content.Length];
                    bool isZip, isEncoded;
                    var decoder = TsmDecoderXml.ReadHeader(header, out isZip, out isEncoded);
                    if (decoder != null)
                    {
                        string cfile = Path.Combine(SaveDirectory, Path.GetFileNameWithoutExtension(i) + (isZip ? ".zip" : ".mp3"));
                        if (isEncoded)
                        {
                            
                            result = decoder.Convert(content);
                            TsmDecoderXml.Swap(ref result, 2);

                            File.WriteAllBytes(cfile, result);
                        }
                        else //rename
                        {
                            File.Copy(i, cfile);
                        }

                        if(isZip && Preferences.TSM_Voice_Extract_Zip)
                        {
                            var dir = cfile.Replace(".zip", "-voice");
                            Directory.CreateDirectory(dir);
                            ZipFile.ExtractToDirectory(cfile, dir);
                            File.Delete(cfile);
                        }
                    }
                    else if(Preferences.TSM_Analyze)
                    {
                        var t = TsmFile.CheckType(i);
                        if(t.HasValue)
                        {
                            var type = t.Value;
                            string cfile = Path.Combine(SaveDirectory, Path.GetFileNameWithoutExtension(i) + ".mp3");
                            if (type == TsmType.Music)
                            {
                                var buffer = TsmFile.TryDecode(i);
                                if(buffer != null)
                                {
                                    File.WriteAllBytes(cfile, buffer);
                                }
                            }
                            else if(type == TsmType.NEMusic)
                            {
                                File.Copy(i, cfile);
                            }
                        }
                    }
                    SingleDone();
                });
            });
        }
    }
}
