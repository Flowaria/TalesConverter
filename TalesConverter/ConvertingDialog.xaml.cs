using SwfDotNet.IO;
using SwfDotNet.IO.Tags;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace TalesConverter
{
    public partial class ConvertingDialog : Window
    {
        public string[] FileToConvert { get; set; }
        public string SaveDir { get; set; }

        public bool AnalyzeMP3File { get; set; } = false;
        public bool MergeFaceImage { get; set; } = false;

        public int MaxThread { get; set; } = 10;
        public int MaxImage { get; set; } = 8;

        public ConvertingDialog()
        {
            InitializeComponent();

            ShowInTaskbar = false;
            double screenWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
            double screenHeight = System.Windows.SystemParameters.PrimaryScreenHeight;
            double windowWidth = this.Width;
            double windowHeight = this.Height;
            this.Left = (screenWidth / 2) - (windowWidth / 2);
            this.Top = (screenHeight / 2) - (windowHeight / 2);
            Topmost = true;
        }

        private async void Window_ContentRendered(object sender, EventArgs e)
        {
            eLabel.Content = "Reading File";
            int Count = FileToConvert.Length;

            var tsiFiles = new List<string>();

            Directory.CreateDirectory(SaveDir);

            for (int i=0;i<Count;i++)
            {
                if (!File.Exists(FileToConvert[i]))
                    continue;

                string extension = Path.GetExtension(FileToConvert[i]);
                string filename = Path.GetFileNameWithoutExtension(FileToConvert[i]);

                if (extension.ToLower().Equals(".tsi"))
                {
                    tsiFiles.Add(FileToConvert[i]);
                }
                else if(extension.ToLower().Equals(".tsm"))
                {
                    eProgress.IsIndeterminate = false;
                    eProgress.Value = (double)i / Count * 100;
                    eLabel.Content = filename;
                    await Task.Factory.StartNew(delegate
                    {
                        byte[] result = new byte[File.ReadAllBytes(FileToConvert[i]).Length];
                        var decoder = TsmDecoder.DecodeFile(FileToConvert[i], out result, AnalyzeMP3File);
                        if(decoder != null)
                        {
                            Console.WriteLine("dec"+result.Length);
                            if (decoder.IsEncrypted)
                            {
                                string cfile = Path.Combine(SaveDir, filename + (decoder.IsZipFile ? ".zip" : ".mp3"));
                                File.WriteAllBytes(cfile, result);
                            }
                            else //rename
                            {
                                string cfile = Path.Combine(SaveDir, filename + (decoder.IsZipFile ? ".zip" : ".mp3"));
                                File.WriteAllBytes(cfile, result);
                            }
                        }
                    });
                }
            }
            if (tsiFiles.Count > 0)
            {
                eProgress.IsIndeterminate = true;
                eLabel.Content = "TSI 파일에서 이미지 추출중...";

                await Task.Factory.StartNew(delegate
                {
                    Parallel.ForEach(tsiFiles, new ParallelOptions { MaxDegreeOfParallelism = MaxThread }, file =>
                    {
                        using (TsiThread c = new TsiThread(MaxImage))
                        {
                            string save_path = Path.Combine(SaveDir, Path.GetFileNameWithoutExtension(file));
                            Directory.CreateDirectory(save_path);

                            c.ExtractFilesFromSWF(file);

                            foreach (var img in c.ImgPNG)
                            {
                                if(File.Exists(img.SaveUrl))
                                {
                                    File.Move(img.SaveUrl, Path.Combine(save_path, img.Name + ".png"));
                                }
                            }

                            foreach (var img in c.ImgJPG)
                            {
                                if (File.Exists(img.SaveUrl))
                                {
                                    File.Move(img.SaveUrl, Path.Combine(save_path, img.Name + ".jpg"));
                                }
                            }
                        }
                    });
                });
                Close();
            }
            else
            {
                Close();
            }
        }
    }
}
