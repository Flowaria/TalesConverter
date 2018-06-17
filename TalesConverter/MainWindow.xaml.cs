using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TalesConverter
{
    public partial class MainWindow : Window
    {
        public int MaxThread { get; set; } = 4;
        public int MaxImage { get; set; } = 8;

        public MainWindow()
        {
            InitializeComponent();

            slide1.DataContext = this;
            slide2.DataContext = this;

            Left = (SystemParameters.PrimaryScreenWidth / 2) - (Width / 2);
            Top = (SystemParameters.PrimaryScreenHeight / 2) - (Height / 2);
        }

        private void slide2_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            slide2_label.Text = MaxImage.ToString();
        }

        private void slide1_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            slide1_label.Text = MaxThread.ToString();
        }

        private void ImagePanel_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                FileSelected(files);
            }
        }

        private void btn1_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "";
            
            openFileDialog.Filter = "Tales# files (*.tsm, *.tsi) | *.tsm; *.tsi";
            openFileDialog.FilterIndex = 2;
            openFileDialog.Multiselect = true;
            openFileDialog.RestoreDirectory = true;
            if (openFileDialog.ShowDialog() == true)
            {
                FileSelected(openFileDialog.FileNames);
            }
        }

        private void FileSelected(string[] files)
        {
            string dir = null;

            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            dialog.Multiselect = false;
            if(dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                dir = dialog.FileName;
            }
            if (dir != null)
            {
                Hide();

                ConvertingDialog cDialog = new ConvertingDialog();
                cDialog.FileToConvert = files;
                cDialog.SaveDir = dir;
                cDialog.AnalyzeMP3File = cb1.IsChecked.Value;
                cDialog.MergeFaceImage = cb2.IsChecked.Value;
                cDialog.MaxThread = MaxThread;
                cDialog.MaxImage = MaxImage;
                cDialog.ShowDialog();
                Show();
                Activate();

                GC.Collect();
            }
            else
            {
                MessageBox.Show("잘못된 경로를 선택하셨습니다.", "알림",MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
