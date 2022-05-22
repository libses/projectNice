using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Domain;
using Domain.Render;
using Microsoft.Win32;
using Image = System.Windows.Controls.Image;

namespace WpfApp2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var i = 1d;
            var bmp =
                ImageBase.Create()
                    .Config(new ImageSettings(1500, 1500))
                    .Add<Mandelbrot>(m => m.Config(new MandelbrotSettings(i, 0.311, 0.482)))
                    .Multiply<Gradient>(g => g)
                    .GetBitmap();

            ImageViewer1.Source = BitmapToImageSource(bmp);
            PlayBtn.Click += (_, _) => mediaPlayer.Play();
            PauseBtn.Click += (_, _) => mediaPlayer.Pause();
        }

        static BitmapImage BitmapToImageSource(DirectBitmap bitmap)
        {
            using MemoryStream memory = new MemoryStream();
            bitmap.Bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
            memory.Position = 0;
            BitmapImage bitmapimage = new BitmapImage();
            bitmapimage.BeginInit();
            bitmapimage.StreamSource = memory;
            bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapimage.EndInit();

            return bitmapimage;
        }

        private MediaPlayer mediaPlayer = new MediaPlayer();

        private void btnOpenFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Audio files (*.mp3;*.wav;*.aac)|*.mp3;*.wav;*.aac";
            if (openFileDialog.ShowDialog() == true)
            {
                mediaPlayer.Open(new Uri(openFileDialog.FileName));
            }
            
        }
    }
}