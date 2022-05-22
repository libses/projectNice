using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Kernel;
using Kernel.Domain.Settings;
using Kernel.Domain;
using Kernel.Domain.Utils;
using NAudio.Wave;
using Spectrogram;

namespace WpfApp2;

public class VideoGenerator
{
    private readonly string filePath;
    private readonly int width;
    private readonly int height;

    private const string TempFilesPath =
        $@"C:\Users\Garipov\RiderProjects\ProjectNice\WpfApp2\bin\Debug\net6.0-windows\temp_img";

    public VideoGenerator(string path, int width, int height)
    {
        filePath = path;
        this.width = width;
        this.height = height;
    }

    private (double[] audio, int sampleRate) ReadWavMono(double multiplier = 16_000)
    {
        using var afr = new AudioFileReader(filePath);
        var sampleRate = afr.WaveFormat.SampleRate;
        var bytesPerSample = afr.WaveFormat.BitsPerSample / 8;
        var sampleCount = (int)(afr.Length / bytesPerSample);
        var channelCount = afr.WaveFormat.Channels;
        var audio = new List<double>(sampleCount);
        var buffer = new float[sampleRate * channelCount];
        int samplesRead;
        while ((samplesRead = afr.Read(buffer, 0, buffer.Length)) > 0)
            audio.AddRange(buffer.Take(samplesRead).Select(x => x * multiplier));
        return (audio.ToArray(), sampleRate);
    }

    private List<double[]> GenerateMusicVideoFull()
    {
        var (audio, sampleRate) = ReadWavMono();
        var sg = new SpectrogramGenerator(sampleRate, fftSize: 4096, stepSize: 2000, maxFreq: 3000);
        sg.Add(audio);
        var fft = sg.GetFFTs();
        return fft;
    }


    public IEnumerable<int> Planets(int speed)
    {
        var f = new Planets(width, height).Config(new PlanetsSettings(20, 10, 100, Brushes.Chartreuse));
        f.speed = 10;
        for (var i = 0; i < 10000000; i++)
        {
            var bmp = f.GetBitmap().Bitmap;
            bmp.Save($@"{TempFilesPath}\{i}.bmp");
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
            bmp.Save($@"{TempFilesPath}\{i}.bmp");
            bmp.Dispose();
            yield return 1;
        }
    }

    public IEnumerable<int> FunnyAnd()
    {
        var fft = GenerateMusicVideoFull();
        var funny = new Funny(width, height).Config(new FunnySettings(fft));
        var anim2 = new Planets(width, height).Config(new PlanetsSettings(20, 10, 100, Brushes.Aqua));
        anim2.speed = 10;
        for (int i = 0; i < fft.Count; i++)
        {
            var bmp = funny.GetBitmap();
            bmp.Add(anim2.GetBitmap());
            bmp.Bitmap.Save($@"{TempFilesPath}\{i}.bmp");
            bmp.Dispose();
            yield return 1;
        }
    }
}