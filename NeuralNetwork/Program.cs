using FFMediaToolkit;
using Kernel.Domain.Utils;
using Kernel.Services;
using Newtonsoft.Json;
using System;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;

namespace NeuralNetwork
{
    internal class Program
    {
        public static void GenerateNeuralVideo()
        {
            var fftR = new FFTGenerator(new WavAudioMonoProvider(16000));
            var lengthIndex = 32;
            var fft = fftR.GetFFT("AI.wav");
            var minFirst = double.MaxValue;
            var maxFirst = 0d;
            var minSecond = double.MaxValue;
            var maxSecond = 0d;
            var minThird = double.MaxValue;
            var maxThird = 0d;
            var fft2 = new double[fft.Count, 3];
            var discrete = fft[0].Length / 3;
            for (int i = 0; i < fft.Count; i++)
            {
                for (int j = 0; j < discrete; j++)
                {
                    fft2[i, 0] += fft[i][j];
                }

                if (fft2[i, 0] > maxFirst)
                {
                    maxFirst = fft2[i, 0];
                }

                if (fft2[i, 0] < minFirst)
                {
                    minFirst = fft2[i, 0];
                }

                for (int j = discrete; j < discrete * 2; j++)
                {
                    fft2[i, 1] += fft[i][j];
                }

                if (fft2[i, 1] > maxSecond)
                {
                    maxSecond = fft2[i, 1];
                }

                if (fft2[i, 1] < minSecond)
                {
                    minSecond = fft2[i, 1];
                }

                for (int j = discrete * 2; j < discrete * 3; j++)
                {
                    fft2[i, 2] += fft[i][j];
                }

                if (fft2[i, 2] > maxThird)
                {
                    maxThird = fft2[i, 2];
                }

                if (fft2[i, 2] < minThird)
                {
                    minThird = fft2[i, 2];
                }
            }

            for (int i = 0; i < fft.Count; i++)
            {
                fft2[i, 0] = (fft2[i, 0] - minFirst) / maxFirst;
                fft2[i, 1] = (fft2[i, 1] - minSecond) / maxSecond;
                fft2[i, 2] = (fft2[i, 2] - minThird) / maxThird;
            }

            var ser = new BinaryFormatter();
            var rws = new StreamReader("rw");
            var gws = new StreamReader("gw");
            var bws = new StreamReader("bw");
            var RW = (double[,,])ser.Deserialize(rws.BaseStream);
            Console.WriteLine("R!");
            var GW = (double[,,])ser.Deserialize(gws.BaseStream);
            var BW = (double[,,])ser.Deserialize(bws.BaseStream);
            var count = 800;
            for (int i = 0; i < count; i++)
            {
                var rwIndex = (int)Math.Min(Math.Round(lengthIndex * (double)i / count), 31);
                var bmp = new DirectBitmap(1280, 676);
                for (int x = 0; x < 1280; x++)
                {
                    for (int y = 0; y < 676; y++)
                    {
                        var r = Math.Min(255, 255 * fft2[i, 0] / RW[rwIndex, x, y]);
                        var g = Math.Min(255, 255 * fft2[i, 1] / GW[rwIndex, x, y]);
                        var b = Math.Min(255, 255 * fft2[i, 2] / BW[rwIndex, x, y]);
                        bmp.SetPixel(x, y, Color.FromArgb((int)r, (int)g, (int)b));
                    }
                }

                if (i % 40 == 0)
                {
                    Console.WriteLine((double)i / count);
                }

                bmp.Bitmap.Save("out/" + i + ".png");
            }

            var provider = new BitmapProvider("out", "png", count);
            var creator = new VideoCreator(new FFMediaToolkit.Encoding.VideoEncoderSettings(1280, 676, 44, FFMediaToolkit.Encoding.VideoCodec.H265));
            creator.CreateWithSound(provider.Get(), "C:\\videos\\ai.mp4", "AI.mp3");
            //creator.Create(provider.Get(), "C:\\videos\\ai.mp4");
        }

        static void Main(string[] args)
        {
            FFmpegLoader.FFmpegPath = "C:\\ff\\bin";
            //var fftR = new FFTGenerator(new WavAudioMonoProvider(16000));
            //var fft = fftR.GetFFT("AI.wav");
            //var minFirst = double.MaxValue;
            //var maxFirst = 0d;
            //var minSecond = double.MaxValue;
            //var maxSecond = 0d;
            //var minThird = double.MaxValue;
            //var maxThird = 0d;
            //var fft2 = new double[fft.Count, 3];
            //var discrete = fft[0].Length / 3;
            //for (int i = 0; i < fft.Count; i++)
            //{
            //    for (int j = 0; j < discrete; j++)
            //    {
            //        fft2[i, 0] += fft[i][j];
            //    }

            //    if (fft2[i, 0] > maxFirst)
            //    {
            //        maxFirst = fft2[i, 0];
            //    }

            //    if (fft2[i, 0] < minFirst)
            //    {
            //        minFirst = fft2[i, 0];
            //    }

            //    for (int j = discrete; j < discrete * 2; j++)
            //    {
            //        fft2[i, 1] += fft[i][j];
            //    }

            //    if (fft2[i, 1] > maxSecond)
            //    {
            //        maxSecond = fft2[i, 1];
            //    }

            //    if (fft2[i, 1] < minSecond)
            //    {
            //        minSecond = fft2[i, 1];
            //    }

            //    for (int j = discrete * 2; j < discrete * 3; j++)
            //    {
            //        fft2[i, 2] += fft[i][j];
            //    }

            //    if (fft2[i, 2] > maxThird)
            //    {
            //        maxThird = fft2[i, 2];
            //    }

            //    if (fft2[i, 2] < minThird)
            //    {
            //        minThird = fft2[i, 2];
            //    }
            //}

            //for (int i = 0; i < fft.Count; i++)
            //{
            //    fft2[i, 0] = (fft2[i, 0] - minFirst) / maxFirst;
            //    fft2[i, 1] = (fft2[i, 1] - minSecond) / maxSecond;
            //    fft2[i, 2] = (fft2[i, 2] - minThird) / maxThird;
            //}

            //var videoLength = 3521;
            //var lengthIndex = 32;
            //var RW = new double[lengthIndex, 1280, 676];
            //var GW = new double[lengthIndex, 1280, 676];
            //var BW = new double[lengthIndex, 1280, 676];
            //var trainCount = 800;
            //for (int i = 0; i < trainCount; i++)
            //{
            //    var fftIndex = (int)Math.Round((double)fft2.Length * i / videoLength);
            //    var currentImage = (Bitmap)Bitmap.FromFile("train32/frame" + i + ".png");
            //    var snoop = new BmpPixelSnoop(currentImage);
            //    var RWIndex = (int)Math.Min(31, Math.Round((double)i * lengthIndex / trainCount));
            //    for (int x = 0; x < 1280; x++)
            //    {
            //        for (int y = 0; y < 676; y++)
            //        {
            //            var color = snoop.GetPixel(x, y);
            //            var r = (double)(color.R + 1) / 256d;
            //            var g = (double)(color.G + 1) / 256d;
            //            var b = (double)(color.B + 1) / 256d;
            //            RW[RWIndex, x, y] += fft2[fftIndex, 0] / (r * trainCount);
            //            GW[RWIndex, x, y] += fft2[fftIndex, 1] / (g * trainCount);
            //            BW[RWIndex, x, y] += fft2[fftIndex, 2] / (b * trainCount);
            //        }
            //    }

            //    if (i % 25 == 0)
            //    {
            //        Console.WriteLine((double)i / videoLength);
            //    }

            //    snoop.Dispose();
            //    currentImage.Dispose();
            //}

            //Stream rws = File.Create("rw");
            //Stream gws = File.Create("gw");
            //Stream bws = File.Create("bw");
            //var serializer = new BinaryFormatter();
            //serializer.Serialize(rws, RW);
            //serializer.Serialize(gws, GW);
            //serializer.Serialize(bws, BW);
            //rws.Close();
            //gws.Close();
            //bws.Close();
            GenerateNeuralVideo();
        }
    }
}
