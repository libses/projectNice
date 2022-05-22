using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using Domain.Render;
using Domain.Settings;
using FFMediaToolkit;
using FFMediaToolkit.Encoding;
using FFMediaToolkit.Graphics;
using ILGPU;
using NAudio.Wave;
using Spectrogram;

namespace WpfApp2;

public class VideoGenerator
{
    private string filePath;
    private readonly int width;
    private readonly int height;

    private static readonly string tempFilesPath =
        $@"C:\Users\Garipov\RiderProjects\ProjectNice\WpfApp2\bin\Debug\net6.0-windows\temp_img";

    public VideoGenerator(string path, int width, int height)
    {
        filePath = path;
        this.width = width;
        this.height = height;
    }

    public (double[] audio, int sampleRate) ReadWavMono(double multiplier = 16_000)
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

    public List<double[]> GenerateMusicVideoFull()
    {
        (double[] audio, int sampleRate) = ReadWavMono();
        var sg = new SpectrogramGenerator(sampleRate, fftSize: 4096, stepSize: 2000, maxFreq: 3000);
        sg.Add(audio);
        var fft = sg.GetFFTs();
        return fft;
    }
    


    public IEnumerable<int> Planets(int speed)
    {
      //  var fft = GenerateMusicVideoFull();
        var f = new Planets(width, height).Config(new PlanetsSettings(20, 10, 100, Brushes.Chartreuse));
        f.speed = 10;
        for (int i = 0; i < 10000000; i++)
        {
            var bmp = f.GetBitmap().Bitmap;
            bmp.Save($@"{tempFilesPath}\{i}.bmp");
            bmp.Dispose();
            yield return 1;
        }
    }
    public IEnumerable<int> Funny()
    {
        var fft = GenerateMusicVideoFull();
        var f = new Funny(width, height).Config(new FunnySettings(fft));
        for (int i = 0; i < fft.Count; i++)
        {
            var bmp = f.GetBitmap().Bitmap;
            bmp.Save($@"{tempFilesPath}\{i}.bmp");
            bmp.Dispose();
            yield return 1;
        }
    }
}