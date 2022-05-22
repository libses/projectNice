using Domain;
using FFMediaToolkit;
using FFMediaToolkit.Encoding;
using FFMediaToolkit.Graphics;
using NAudio.Wave;
using Spectrogram;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using Domain.Render;

namespace ConsoleApp
{
    public class FftGenerator
    {
        public static List<double[]> GetFft()
        {
            (double[] audio, int sampleRate) = new WavAudioProvider().ReadWavMono("808.wav");
            var sg = new SpectrogramGenerator(sampleRate, fftSize: 4096, stepSize: 2000, maxFreq: 3000);
            sg.Add(audio);
            var fft = sg.GetFFTs();

            return fft;
        }
    }

    public class PhotoProvider
    {
        public static IEnumerable<Bitmap> GetPhotos(int count)
        {
            for (int i = 0; i < count; i++)
            {
                yield return (Bitmap)Bitmap.FromFile("temp\\" + i + ".jpg");
            }
        }
    }
    public interface IWavAudioProvider
    {
        public (double[] audio, int sampleRate) ReadWavMono(string filePath, double multiplier);
    }

    public class WavAudioProvider : IWavAudioProvider
    {
        public (double[] audio, int sampleRate) ReadWavMono(string filePath, double multiplier = 16_000)
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

        public static double[] SumFftInRangeAndNormalize(List<double[]> fft, int x, int y, int down, int up)
        {
            var array = new double[fft.Count];
            var min = 10000000000d;
            var max = 0d;
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
    }

    public class VideoCreator
    {
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
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            var x = 1920;
            var y = 1080;
            var mandel = new Mandelbrot(x, y);
            var counter = 0;
            for (double i = 1d; i > 0.1d; i *= 0.993)
            {
                var bmp = mandel
                    .Config(new MandelbrotSettings(i, -0.74529, 0.113075, x, y))
                    .GetBitmap()
                    .Bitmap;

                bmp.SaveJPG100(String.Format("temp\\{0}.jpg", counter));
                bmp.Dispose();

                counter++;
            }

            VideoCreator.CreateVideoYield(PhotoProvider.GetPhotos(counter), 1920, 1080, 44);
        }
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