using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using TalesSharp.Thread;

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

        public double Progress { get; set; } = 0.0;
        public string LabelText { get; set; } = "Wait a sec!";

        public ConvertingDialog()
        {
            InitializeComponent();

            eProgress.DataContext = this;
            eLabel.DataContext = this;

            Left = (SystemParameters.PrimaryScreenWidth / 2) - (Width / 2);
            Top = (SystemParameters.PrimaryScreenHeight / 2) - (Height / 2);
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            var result = MessageBox.Show("정말로 종료할까요?", "작업 취소중...", MessageBoxButton.OKCancel, MessageBoxImage.Question);
            if(result == MessageBoxResult.OK)
            {

            }
            else
            {
                e.Cancel = true;
            }
        }

        private async void Window_ContentRendered(object sender, EventArgs e)
        {
            eLabel.Content = "Reading File";
            int Count = FileToConvert.Length;

            var tsmFiles = new List<string>();
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
                    tsmFiles.Add(FileToConvert[i]);
                }
            }
            if(tsmFiles.Count > 0)
            {
                eLabel.Content = "TSM 파일 목록 확인 중...";
                eProgress.IsIndeterminate = true;
                var worker = new TsmBundleWorker();
                worker.SaveDirectory = SaveDir;
                worker.MaxThread = MaxThread;
                worker.OnSingleEnd += c_ProgressChanged;
                worker.OnTotalCalculated += c_TotalCalc;
                await worker.Handle(tsmFiles);
            }
            if (tsiFiles.Count > 0)
            {
                eLabel.Content = "TSI 파일(들)에서 이미지 목록 확인 중...";
                eProgress.IsIndeterminate = true;
                var worker = new TsiBundleWorker();
                worker.SaveDirectory = SaveDir;
                worker.MaxThread = MaxThread;
                worker.MaxExtractThread = MaxImage;
                worker.OnSingleEnd += c_ProgressChanged;
                worker.OnTotalCalculated += c_TotalCalc;
                await worker.Handle(tsiFiles);
            }
            Close();
        }

        private void c_TotalCalc(object sender, ProgressEventArgs e)
        {
            Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
            {
                eProgress.IsIndeterminate = false;
            }));
        }

        private void c_ProgressChanged(object sender, ProgressEventArgs e)
        {
            Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
            {
                eProgress.Maximum = e.Max;
                eProgress.Value = e.Current;
                eLabel.Content = e.Max + " / " + e.Current;
            }));
        }
    }
}
