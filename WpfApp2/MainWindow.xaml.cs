using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using Domain;
using Domain.Render;
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
            //InitializeComponent();

            ////  MePlayer.Source = new Uri("https://stsci-opo.org/STScI-01G0MSZ1K4MDRWH3ATFGNPBYB2.mp4");
            //// PauseBtn.Click += (_, _) => MePlayer.Pause();
            //// PlayBtn.Click += (_, _) => MePlayer.Play();
            ////     var img = new Image(){Width = 100, Height = 200};
            //var i = 1d;
            ////var bmp = ImageBase.Create()
            ////    .Config(new ImageSettings(1500, 1500))
            ////    .Add<Mandelbrot>(m => m.Config(new MandelbrotSettings(i, 0.311, 0.482)))
            ////    .Multiply<Gradient>(g => g)
            ////    .GetBitmap();

            //ImageViewer1.Source = BitmapToImageSource(bmp);
            //PlayBtn.Click += (_, _) =>
            //{
            //    i /= 1.05;
            //    //var bmp = ImageBase.Create()
            //    //    .Config(new ImageSettings(1500, 1500))
            //    //    .Add<Mandelbrot>(m => m.Config(new MandelbrotSettings(i, 0.311, 0.482)))
            //    //    .Multiply<Gradient>(g => g)
            //    //    .GetBitmap();
            //    ImageViewer1.Source = BitmapToImageSource(bmp);
            //};
        }

        static BitmapImage BitmapToImageSource(DirectBitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }
    }
}