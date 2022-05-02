using AnimatedGif;
using Domain;
using FFMediaToolkit;
using FFMediaToolkit.Encoding;
using FFMediaToolkit.Graphics;
using NAudio.Dsp;
using NAudio.Wave;
using Spectrogram;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp
{
    internal class Program
    {
        public static (double[] audio, int sampleRate) ReadWavMono(string filePath, double multiplier = 16_000)
        {
            using var afr = new AudioFileReader(filePath);
            int sampleRate = afr.WaveFormat.SampleRate;
            int bytesPerSample = afr.WaveFormat.BitsPerSample / 8;
            int sampleCount = (int)(afr.Length / bytesPerSample);
            int channelCount = afr.WaveFormat.Channels;
            var audio = new List<double>(sampleCount);
            var buffer = new float[sampleRate * channelCount];
            int samplesRead = 0;
            while ((samplesRead = afr.Read(buffer, 0, buffer.Length)) > 0)
                audio.AddRange(buffer.Take(samplesRead).Select(x => x * multiplier));
            return (audio.ToArray(), sampleRate);
        }

        static void Main(string[] args)
        {
            //var mb = new Mandelbrot(3000, 3000);
            (double[] audio, int sampleRate) = ReadWavMono("606.wav");
            var sg = new SpectrogramGenerator(sampleRate, fftSize: 4096, stepSize: 2000, maxFreq: 3000);
            sg.Add(audio);
            var fft = sg.GetFFTs();
            var sample = fft.Take(660);
            var sum = sample.Select(x => x.Sum());
            var max = sum.Max();
            var min = sum.Min();
            var n = new FFT(200, 200);
            var imgs = sample.Select(x => n.GetBitmap(x, min, max)).ToArray();
            var settings = new VideoEncoderSettings(width: 200, height: 200, framerate: 44, codec: VideoCodec.H264);
            FFmpegLoader.FFmpegPath = @"C:\ff\bin";
            var file = MediaBuilder.CreateContainer(@"C:\videos\example.mp4").WithVideo(settings).Create();
            for (int i = 0; i < imgs.Length; i++)
            {
                Console.WriteLine(i/(double)imgs.Length);
                var bitmap = imgs[i];
                var rect = new Rectangle(Point.Empty, bitmap.Size);
                var bitLock = bitmap.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
                var bitmapData = ImageData.FromPointer(bitLock.Scan0, ImagePixelFormat.Bgr24, bitmap.Size);

                file.Video.AddFrame(bitmapData); // Encode the frame

                bitmap.UnlockBits(bitLock);
            }

            file.Dispose();
            //sg.SaveImage("hal.png");
            //44.25 fps

            /*int Fact(int n)
            {
                var r = 1;
                for (var i = 1; i < n; i++)
                {
                    r *= i;
                }
                return r;
            }

            var framesCount = 20;
            var imgs = new Bitmap[framesCount];
            var render = Parallel.For(0, framesCount,
                i =>
                {
                    imgs[i] = mb.GetBitmap(199, 284, 1 + Fact(i)/20d);
                });
            if (render.IsCompleted)
                using (var gif = AnimatedGif.AnimatedGif.Create("output.gif", 20))
                {
                    foreach (var img in imgs)
                    {
                        gif.AddFrame(img, delay: 200, quality: GifQuality.Bit8);
                    }
                }*/
            //mb.GetBitmap(900, 1500, 1.2).Save("aboba.bmp");
        }
    }
}