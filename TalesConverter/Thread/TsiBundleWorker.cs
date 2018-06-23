using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalesConverter.Thread
{
    public class TsiBundleWorker : BundleWorker
    {
        public int MaxExtractThread { get; set; }

        public override async Task Handle(List<string> tsiFiles)
        {
            await Task.Factory.StartNew(delegate
            {
                List<ImageContainer> WorkImg = new List<ImageContainer>();

                Parallel.ForEach(tsiFiles, new ParallelOptions { MaxDegreeOfParallelism = MaxThread }, file =>
                {
                    using (TsiThread worker = new TsiThread(MaxExtractThread))
                    {
                        worker.ExtractFilesFromSWF(file);

                        TotalResources += worker.ImgJPG.Count + worker.ImgPNG.Count;

                        WorkImg.AddRange(worker.ImgPNG);
                        WorkImg.AddRange(worker.ImgJPG);
                    }
                });

                TotalCalculated();

                Parallel.ForEach(WorkImg, new ParallelOptions { MaxDegreeOfParallelism = MaxExtractThread }, imgTag =>
                {
                    string directory = Path.Combine(SaveDirectory, imgTag.SWFFileName);
                    Directory.CreateDirectory(directory);
                    if (imgTag.PngTag != null)
                    {
                        imgTag.PngTag.DecompileToFile(Path.Combine(directory, imgTag.Name + ".png"));
                        SingleDone();
                    }
                    else if (imgTag.JpegTag != null)
                    {
                        imgTag.JpegTag.DecompileToFile(Path.Combine(directory, imgTag.Name + ".jpg"));
                        SingleDone();
                    }
                });

                if (Preferences.TSI_Zip_Image)
                {
                    Parallel.ForEach(tsiFiles, new ParallelOptions { MaxDegreeOfParallelism = MaxThread }, file =>
                    {
                        string directory = Path.Combine(SaveDirectory, Path.GetFileNameWithoutExtension(file));
                        if(Directory.Exists(directory))
                        {
                            ZipFile.CreateFromDirectory(directory, directory+".images.zip");
                            Directory.Delete(directory, true);
                        }
                    });
                }
            });
        }
    }
}
