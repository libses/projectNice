using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Domain;
using Domain.Render;
using Microsoft.Win32;
using NAudio.Wave;
using Image = System.Windows.Controls.Image;

namespace WpfApp2
{
    // TODO : Сделать рендер реального времени (или псевдо-), так, чтобы музыка и видео воспроизводились синхронно. NAZAR
    // 44.1 fps (1/44.1) NAZAR
    // Все операции над картинками -- в видеокарте. IVAN
    // Добавить ещё операций, кроме Add и Multiply, другие фракталы/картинки
    // Решить вопрос с аллокацией памяти. IVAN
    // Почитать регламент и сделать Readme.md и тому подобное.
    // Тесты, DDD, DI. Alexey
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Image imgViewer;
        private MediaPlayer mediaPlayer = new MediaPlayer();


        public MainWindow()
        {
            InitializeComponent();
            Task.Run(StartImageUpdater);
           // StartImageUpdater();
            PlayBtn.Click += (_, _) =>
            {
                isStarted = true;
                mediaPlayer.Play();
            };
            PauseBtn.Click += (_, _) =>
            {
                isStarted = false;
                mediaPlayer.Pause();
            };
        }

        private double i = 1d;
        private Mandelbrot mandelbrot = new Mandelbrot(1280, 720);

        private static readonly Action EmptyDelegate = delegate { };
        private static bool isStarted = false;

        void StartImageUpdater()
        {
            foreach (var bmp in Audio.BadExample_Planets())
            {
                while (!isStarted)
                {
                }

                Dispatcher.Invoke(() =>
                    ImageViewer1.Source = BitmapToImageSource(bmp)
                );
                Thread.Sleep(10);
            }
        }

        public static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();

            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }

            return null;
        }

        static BitmapImage BitmapToImageSource(DirectBitmap bitmap)
        {
            using var memory = new MemoryStream();
            bitmap.Bitmap.Save(memory, ImageFormat.Bmp);
            memory.Position = 0;
            var bitmapimage = new BitmapImage();
            bitmapimage.BeginInit();
            bitmapimage.StreamSource = memory;
            bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapimage.EndInit();

            return bitmapimage;
        }

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