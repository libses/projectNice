using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using Domain;
using Domain.Render;
using Domain.Settings;
using FFMediaToolkit;
using FFMediaToolkit.Encoding;
using FFMediaToolkit.Graphics;
using NAudio.Wave;
using Spectrogram;

namespace WpfApp2;

public class Audio
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
    
    public static (double[], double[], double[]) GenerateMusicVideo()
    {
        (double[] audio, int sampleRate) = ReadWavMono(@"C:\Users\Garipov\RiderProjects\ProjectNice\ConsoleApp\bin\Debug\net6.0-windows\808.wav");
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
        (double[] audio, int sampleRate) = ReadWavMono(@"C:\Users\Garipov\RiderProjects\ProjectNice\ConsoleApp\bin\Debug\net6.0-windows\808.wav");
        var sg = new SpectrogramGenerator(sampleRate, fftSize: 4096, stepSize: 2000, maxFreq: 3000);
        sg.Add(audio);
        var fft = sg.GetFFTs();

        return fft;
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
    
    public static IEnumerable<DirectBitmap> BadExample_Planets()
    {
        var x = 720;
        var y = 720;
        var fft = GenerateMusicVideoFull();
        var array = GenerateMusicVideo();
        var db = new DirectBitmap[fft.Count];
        var f = new Funny(x, y).Config(new FunnySettings(fft));
        for (int i = 0; i < fft.Count; i++)
        {
            yield return f.GetBitmap();
        }
        CreateVideo(db, x, y, 44);
    }
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
}