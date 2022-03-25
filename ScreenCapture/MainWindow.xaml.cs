using ScreenCapture.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ScreenCapture
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern IntPtr GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        private IntPtr _activeHandle;

        public MainWindow()
        {
            InitializeComponent();

            _fileName.Text = Settings.Default.LastFileName == null ? @"C:\Temp\Screen" : Settings.Default.LastFileName;
        }

        private string _lastFileName = null;

        private void Capture_Click(object sender, RoutedEventArgs e)
        {
            Save();
        }

        BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);

                memory.Position = 0;

                BitmapImage bitmapimage = new BitmapImage();

                bitmapimage.BeginInit();

                bitmapimage.StreamSource = memory;

                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;

                bitmapimage.EndInit();

                return bitmapimage;
            }
        }

        private void Save(string fileName = null)
        {
            try
            {
                List<int> test1 = new List<int>();

                List<int> test2 = test1.Take(20).ToList();

                Settings.Default.LastFileName = _fileName.Text;

                Settings.Default.Save();

                bool isReplace = fileName != null;

                if (fileName == null)
                {
                    fileName = _fileName.Text + "SC-%NOW%";
                }

                if (!string.IsNullOrEmpty(_notesTextBox.Text) && !string.IsNullOrEmpty(_lastFileName))
                {
                    File.WriteAllText(_lastFileName.Replace(".png", ".txt"), _notesTextBox.Text);

                    if (!isReplace)
                    {
                        _notesTextBox.Text = "";
                    }
                }

                _lastFileName = ScreenCapturer.CaptureAndSave(fileName, _activeHandle);

                var img = Bitmap.FromFile(_lastFileName);

                Bitmap bm = new Bitmap(img);

                _image.Stretch = Stretch.Fill;

                _image.Source = BitmapToImageSource(bm);

                FileInfo fi = new FileInfo(_lastFileName);

                _savedFileName.Text = fi.Name;
            }
            catch (Exception)
            {

            }
        }

        private void SelectWindow_Click(object sender, RoutedEventArgs e)
        {
            _textBlock.Text = "Searching...";

            IntPtr _currentHandle = GetForegroundWindow();

            _activeHandle = _currentHandle;

            while (_activeHandle == _currentHandle)
            {
                _activeHandle = GetForegroundWindow();

                System.Threading.Thread.Sleep(10);
            }

            StringBuilder builder = new StringBuilder(255);

            GetWindowText(GetForegroundWindow(), builder, 255);

            _textBlock.Text = builder.ToString();
        }

        private void FileExplorer_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog folderBrowser = new OpenFileDialog();

            // Set validate names and check file exists to false otherwise windows will
            // not let you select "Folder Selection."
            folderBrowser.ValidateNames = false;

            folderBrowser.CheckFileExists = false;

            folderBrowser.CheckPathExists = true;

            // Always default to Folder Selection.
            folderBrowser.FileName = "Select Folder";

            if (folderBrowser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string folderPath = Path.GetDirectoryName(folderBrowser.FileName);

                _fileName.Text = folderPath + "\\";
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (_lastFileName != null)
            {
                File.Delete(_lastFileName);

                _lastFileName = null;

                _savedFileName.Text = "";
            }
        }

        private void ReplaceLastImage_Click(object sender, RoutedEventArgs e)
        {
            if (_lastFileName != null)
            {
                File.Delete(_lastFileName);
            }

            Save(_lastFileName.Replace(".png", ""));
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (!string.IsNullOrEmpty(_notesTextBox.Text) && !string.IsNullOrEmpty(_lastFileName))
            {
                File.WriteAllText(_lastFileName.Replace(".png", ".txt"), _notesTextBox.Text);
            }

            base.OnClosing(e);
        }
    }
}
