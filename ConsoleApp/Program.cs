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
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using Domain.Render;
using Domain.Settings;

namespace ConsoleApp
{
    internal class Program
    {
        public static (double[] audio, int sampleRate) ReadWavMono(string filePath, double multiplier = 16_000)
        {
            using var afr = new AudioFileReader(filePath);
            int sampleRate = afr.WaveFormat.SampleRate;
            int bytesPerSample = afr.WaveFormat.BitsPerSample / 8;
            int sampleCount = (int) (afr.Length / bytesPerSample);
            int channelCount = afr.WaveFormat.Channels;
            var audio = new List<double>(sampleCount);
            var buffer = new float[sampleRate * channelCount];
            int samplesRead = 0;
            while ((samplesRead = afr.Read(buffer, 0, buffer.Length)) > 0)
                audio.AddRange(buffer.Take(samplesRead).Select(x => x * multiplier));
            return (audio.ToArray(), sampleRate);
        }

        public static double[] DoThings(List<double[]> fft, int x, int y, int down, int up)
        {
            var array = new double[fft.Count];
            var min = 10000000000d; //1562
            var max = 0d; //26263
            for (int i = 0; i < fft.Count; i++)
            {
                var sum = 0d;
                for (int j = down; j < up; j++)
                {
                    sum += fft[i][j];
                }

                if (sum < min)
                {
                    min = sum;
                }

                if (sum > max)
                {
                    max = sum;
                }
            }

            for (int i = 0; i < fft.Count; i++)
            {
                var sum = 0d;
                for (int j = down; j < up; j++)
                {
                    sum += fft[i][j];
                }

                array[i] = (sum - min) / max;
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

        public static void CreateVideoYield(IEnumerable<Bitmap> imgs, int x, int y, int fps)
        {
            var settings = new VideoEncoderSettings(width: x, height: y, framerate: fps, codec: VideoCodec.H265);
            FFmpegLoader.FFmpegPath = @"C:\ff\bin";
            var file = MediaBuilder.CreateContainer(@"C:\videos\example.mp4").WithVideo(settings).Create();
            foreach (var bitmap in imgs)
            {
                var rect = new Rectangle(Point.Empty, bitmap.Size);
                var bitLock = bitmap.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
                var bitmapData = ImageData.FromPointer(bitLock.Scan0, ImagePixelFormat.Bgr24, bitmap.Size);

                file.Video.AddFrame(bitmapData); // Encode the frame

                bitmap.UnlockBits(bitLock);
            }

            file.Dispose();
        }

        public static IEnumerable<Bitmap> PhotoYielder(int count)
        {
            for (int i = 0; i < count; i++)
            {
                yield return (Bitmap) Bitmap.FromFile("temp\\" + i + ".jpg");
            }
        }

        public static (double[], double[], double[]) GenerateMusicVideo()
        {
            (double[] audio, int sampleRate) = ReadWavMono("808.wav");
            var sg = new SpectrogramGenerator(sampleRate, fftSize: 4096, stepSize: 2000, maxFreq: 3000);
            sg.Add(audio);
            var fft = sg.GetFFTs();
            var discrete = fft[0].Length / 3;
            var R = DoThings(fft, 200, 200, 0, discrete);
            var G = DoThings(fft, 200, 200, discrete, 2 * discrete);
            var B = DoThings(fft, 200, 200, 2 * discrete, 3 * discrete);
            return (R, G, B);
        }

        public static List<double[]> GenerateMusicVideoFull()
        {
            (double[] audio, int sampleRate) = ReadWavMono("808.wav");
            var sg = new SpectrogramGenerator(sampleRate, fftSize: 4096, stepSize: 2000, maxFreq: 3000);
            sg.Add(audio);
            var fft = sg.GetFFTs();

            return fft;
        }

        static void Main(string[] args)
        {
            var x = 1280;
            var y = 720;
            var mandel = new Mandelbrot(x, y);
            var sw = new Stopwatch();
            sw.Start();
            if (!Directory.Exists("./temp"))
                Directory.CreateDirectory("./temp");
            var bmp = mandel.Config(new MandelbrotSettings(1d, -0.74529, 0.113075, x, y)).GetBitmap();
            bmp.Bitmap.SaveJPG100($"temp/0.jpg");
            var counter = 0;
            for (var i = 1d * 0.99; i > 0.0001d; i *= 0.99)
            {
                mandel
                    .Config(new MandelbrotSettings(i, -0.74529, 0.113075, x, y))
                    .Update(bmp)
                    .Bitmap
                    .SaveJPG100($"temp\\{counter}.jpg");

                counter++;
            }

            sw.Stop();
            Console.WriteLine(sw.Elapsed);
            //CreateVideoYield(PhotoYielder(counter), 1280, 720, 44);
        }

        //public void BadExample_Planets()
        //{
        //    var x = 500;
        //    var y = 500;
        //    var array = GenerateMusicVideo();
        //    var planets = new Planets(x, y).Config(new PlanetsSettings(10, 10, 60, Brushes.White));
        //    var db = new DirectBitmap[array.Item1.Length];
        //    for (int i = 0; i < array.Item1.Length; i++)
        //    {
        //        var speed = array.Item1[i] + array.Item2[i] + array.Item3[i];
        //        int R = (int) (array.Item1[i] * 255);
        //        int G = (int) (array.Item2[i] * 255);
        //        int B = (int) (array.Item3[i] * 255);
        //        planets.speed = (float) speed;
        //        var color = Color.FromArgb(R, G, B);
        //        var constant = new Constant(x, y).Config(new ConstantSettings(color));
        //        var bmp = planets.GetBitmap();
        //        bmp.Multiply(constant.GetBitmap());
        //        db[i] = bmp;
        //    }

        //    CreateVideo(db, x, y, 44);
        //}

        //public void BadExample_TryFixThatShit()
        //{
        //    var x = 1024;
        //    var y = 1024;
        //    // var windows = new Planets(x, y)
        //    //     .Config(new PlanetsSettings(12, 300, 300, Brushes.White));
        //    //
        //    // var ashes = new Planets(x, y)
        //    //     .Config(new PlanetsSettings(30, 6, 6, Brushes.White));
        //    new Mandelbrot(x, y)
        //        .Config(new MandelbrotSettings(0, 0.311, 0.482, x, y))
        //        .GetBitmap()
        //        .Bitmap
        //        .Save("aboba.bmp");
        //    var imgs = new List<(double, DirectBitmap)>();
        //    var a = new List<double>();
        //    for (var i = 0.01d; i < 1; i *= 1.02)
        //    {
        //        a.Add(i);
        //    }

        //    Console.Write(a.Count);
        //    var d = a.AsParallel();
        //    var consta = new Constant(x, y).Config(new ConstantSettings(Color.Blue));
        //    d.ForAll(i =>
        //    {
        //        // not compile
        //        // var bmp = ImageBase.Create()
        //        //     .Config(new ImageSettings(x, y))
        //        //     .Add<Mandelbrot>(m => m.Config(new MandelbrotSettings(i, 0.311, 0.482)))
        //        //     .Multiply<Gradient>(g => g)
        //        //     .GetBitmap().Multiply(consta.GetBitmap());
        //        // imgs.Add((i, bmp));
        //    });

        //    Console.WriteLine("video!");
        //    //CreateVideo(imgs.OrderByDescending(x => x.Item1).Select(x => x.Item2.Add(ashes.GetBitmap()).Multiply(windows.GetBitmap())).ToArray(), x, y, 24);
        //}
    }

    public static class BitmapExtensions
    {
        public static void SaveJPG100(this Bitmap bmp, string filename)
        {
            EncoderParameters encoderParameters = new EncoderParameters(1);
            encoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100L);
            bmp.Save(filename, GetEncoder(ImageFormat.Jpeg), encoderParameters);
        }

        public static void SaveJPG100(this Bitmap bmp, Stream stream)
        {
            EncoderParameters encoderParameters = new EncoderParameters(1);
            encoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100L);
            bmp.Save(stream, GetEncoder(ImageFormat.Jpeg), encoderParameters);
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
    }
}