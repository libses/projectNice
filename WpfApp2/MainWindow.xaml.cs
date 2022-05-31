using System;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Kernel.Domain;
using Kernel.Services;
using Microsoft.Win32;

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
        private readonly MediaPlayer mediaPlayer = new();
        private static bool isStarted;
        private static bool videoInitialized;
        private CancellationTokenSource cts = new();

        private const string TempFiles =
            @".\temp_img";


        public MainWindow()
        {
            InitializeComponent();
            PlayBtn.Click += (_, _) =>
            {
                while (!videoInitialized)
                {
                    Thread.Yield();
                }

                isStarted = true;
                mediaPlayer.Play();
            };
            PauseBtn.Click += (_, _) =>
            {
                isStarted = false;
                mediaPlayer.Pause();
            };
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            cts.Cancel();
            base.OnClosing(e);
        }

        private void StartImageUpdater(string audioPath, CancellationToken ct)
        {
            var generator = new VideoGenerator(new WavAudioMonoProvider(16000), 400, 400, audioPath, TempFiles);
            var i = 0;
            generator.Mandelbrot();
            for (var j = 0; j < generator.FftCount; j++)
            {
                videoInitialized = true;
                var temp = i;
                i++;
                if (ct.IsCancellationRequested)
                    break;
                while (!isStarted)
                {
                }

                Dispatcher.Invoke(() =>
                {
                    var filename = $@"{TempFiles}\{temp}.bmp";
                    var next = $@"{TempFiles}\{temp+1}.bmp";
                    while (!File.Exists(next))
                    {
                        
                    }
                    var img = new BitmapImage(new Uri(filename));
                    return ImageViewer1.Source = img;
                });
                Thread.Sleep(30);
            }
        }

        private void btnOpenFile_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Audio files (*.mp3;*.wav;*.aac)|*.mp3;*.wav;*.aac"
            };
            cts.Cancel();
            cts = new CancellationTokenSource();
            if (openFileDialog.ShowDialog() == true)
            {
                mediaPlayer.Open(new Uri(openFileDialog.FileName));
                isStarted = false;
                Task.Run(() => StartImageUpdater(openFileDialog.FileName, cts.Token));
            }
        }
    }
}