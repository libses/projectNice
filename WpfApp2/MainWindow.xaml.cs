using System;
using System.Windows;

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
            // btn.Click += (sender, args) => btn.Content = "Clicked";
            MePlayer.Source = new Uri("https://stsci-opo.org/STScI-01G0MSZ1K4MDRWH3ATFGNPBYB2.mp4");
            PauseBtn.Click += (_, _) => MePlayer.Pause();
            PlayBtn.Click += (_, _) => MePlayer.Play();
        }
    }
}