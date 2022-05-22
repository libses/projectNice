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
        private MediaPlayer mediaPlayer = new();


        public MainWindow()
        {
            InitializeComponent();
            PlayBtn.Click += (_, _) =>
            {
                if (videoInitialized)
                {
                    isStarted = true;
                    mediaPlayer.Play();
                }
            };
            PauseBtn.Click += (_, _) =>
            {
                isStarted = false;
                mediaPlayer.Pause();
            };
        }

        private static bool isStarted;
        private static bool videoInitialized;

        void StartImageUpdater(string path)
        {
            var generator = new VideoGenerator(path, 1280, 720);
            var i = 0;
            foreach (var _ in generator.Planets(30))
            {
                videoInitialized = true;
                var temp = i;
                i++;
                while (!isStarted)
                {
                }

                Dispatcher.Invoke(() =>
                {
                    var img = new BitmapImage(new Uri(
                        $@"C:\Users\Garipov\RiderProjects\ProjectNice\WpfApp2\bin\Debug\net6.0-windows\temp_img\{temp}.bmp"));
                    return ImageViewer1.Source = img;
                });
                Thread.Sleep(6);
            }
        }

        private void btnOpenFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Audio files (*.mp3;*.wav;*.aac)|*.mp3;*.wav;*.aac";
            if (openFileDialog.ShowDialog() == true)
            {
                mediaPlayer.Open(new Uri(openFileDialog.FileName));
                Task.Run(() => StartImageUpdater(openFileDialog.FileName));
            }
        }
    }
}