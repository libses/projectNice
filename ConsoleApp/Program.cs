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
using Domain.Render;

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

        public static DirectBitmap[] DoThings(SpectrogramGenerator sg, int take, int x, int y)
        {
            var fftBitmap = new FFT(x, y);
            var fft = sg.GetFFTs();
            var array = new DirectBitmap[take];
            var min = 10000000000d;
            var max = 0d;
            for (int i = 0; i < take; i++)
            {
                var sum = 0d;
                foreach (var e in fft[i])
                {
                    sum += e;
                }

                if (sum < min)
                {
                    min = sum;
                }

                if (sum > max)
                {
                    max = sum;
                }

                if (sum > 0.00001)
                {
                    array[i] = fftBitmap
                        .Config(new FFTSettings(fft[i], min, max, sum))
                        .GetBitmap();
                }
                else
                {
                    array[i] = new DirectBitmap(x, y);
                }
            }

            return array;
        }
        //H265
        public static void CreateVideo(DirectBitmap[] imgs, int x, int y, int fps)
        {
            var settings = new VideoEncoderSettings(width: x, height: y, framerate: fps, codec: VideoCodec.H265);
            FFmpegLoader.FFmpegPath = @"C:\ff\bin";
            var file = MediaBuilder.CreateContainer(@"C:\videos\example.mp4").WithVideo(settings).Create();
            for (int i = 0; i < imgs.Length; i++)
            {
                var bitmap = imgs[i];
                var rect = new Rectangle(Point.Empty, bitmap.Bitmap.Size);
                var bitLock = bitmap.Bitmap.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
                var bitmapData = ImageData.FromPointer(bitLock.Scan0, ImagePixelFormat.Bgr24, bitmap.Bitmap.Size);

                file.Video.AddFrame(bitmapData); // Encode the frame

                bitmap.Bitmap.UnlockBits(bitLock);
            }

            file.Dispose();
        }

        public static void GenerateMusicVideo()
        {
            (double[] audio, int sampleRate) = ReadWavMono("606.wav");
            var sg = new SpectrogramGenerator(sampleRate, fftSize: 4096, stepSize: 2000, maxFreq: 3000);
            sg.Add(audio);
            var imgs = DoThings(sg, 600, 200, 200);
            CreateVideo(imgs, 200, 200, 44);
        }
        //835 1360
        static void Main(string[] args)
        {
            var x = 1024;
            var y = 1024;
            var windows = new Planets(x, y)
                .Config(new PlanetsSettings(12, 300, Brushes.White));

            var ashes = new Planets(x, y)
                .Config(new PlanetsSettings(30, 6, Brushes.White));
            new Mandelbrot(x, y)
                .Config(new MandelbrotSettings(0, 0.311, 0.482))
                .GetBitmap()
                .Bitmap
                .Save("aboba.bmp");
            var imgs = new List<(double, DirectBitmap)>();
            var a = new List<double>();
            for (var i = 0.01d; i < 1; i *= 1.02)
            {
                a.Add(i);
            }

            Console.Write(a.Count);
            var d = a.AsParallel();
            var consta = new Constant(x, y).Config(new ConstantSettings(Color.Blue));
            d.ForAll(i =>
            {
                var bmp = ImageBase.Create()
                    .Config(new ImageSettings(x, y))
                    .Add<Mandelbrot>(m => m.Config(new MandelbrotSettings(i, 0.311, 0.482)))
                    .Multiply<Gradient>(g => g)
                    .GetBitmap().Multiply(consta.GetBitmap());
                imgs.Add((i, bmp));
            });

            Console.WriteLine("video!");
            CreateVideo(imgs.OrderByDescending(x => x.Item1).Select(x => x.Item2.Add(ashes.GetBitmap()).Multiply(windows.GetBitmap())).ToArray(), x, y, 24);
        }
    }
}