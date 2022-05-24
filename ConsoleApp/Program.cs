using FFMediaToolkit.Encoding;
using Kernel;
using Kernel.Domain;
using Kernel.Domain.Utils;
using Kernel.Services;

namespace ConsoleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var x = 1080;
            var y = 1080;
            var fftP = new FFTGenerator(new WavAudioMonoProvider(16000));
            var fft = fftP.GetFFT("606.wav");
            var threeD = new ThreeD(x, y).Config(new ThreeDSettings(fft));
            var bmp = threeD.GetBitmap();
            bmp.Bitmap.SaveJPG100("temp\\0.jpg");
            for (int i = 1; i < fft.Count; i++)
            {
                threeD.GetBitmapU(bmp);
                bmp.Bitmap.SaveJPG100("temp\\" + i + ".jpg");
                //bmp.Dispose();
            }

            var provider = new BitmapProvider("temp", "jpg", fft.Count);
            var v = new VideoCreator(new VideoEncoderSettings(x, y, 44, VideoCodec.H265));
            //v.Create(provider.Get(), "C:\\videos\\example.mp4");
            v.CreateWithSound(provider.Get(), "C:\\videos\\example.mp4", "606.mp3");
        }
    }
}