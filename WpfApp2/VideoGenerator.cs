using System.Collections.Generic;
using System.Drawing;
using Kernel.Domain.Settings;
using Kernel.Domain;
using Kernel.Domain.Utils;
using Kernel.Services;
using Kernel.Services.Interfaces;

namespace WpfApp2;

public class VideoGenerator
{
    private readonly string filePath;
    private readonly IWavAudioProvider audioProvider;
    private readonly int width;
    private readonly int height;

    private const string TempFilesPath =
        $@"C:\Users\Garipov\RiderProjects\ProjectNice\WpfApp2\bin\Debug\net6.0-windows\temp_img";

    public VideoGenerator(IWavAudioProvider audioProvider, int width, int height, string filePath)
    {
        this.audioProvider = audioProvider;
        this.width = width;
        this.height = height;
        this.filePath = filePath;
    }


    public IEnumerable<int> Planets(int speed)
    {
        var f = new Planets(width, height)
            .Config(new PlanetsSettings(20, 10, 100, Brushes.Chartreuse));
        f.speed = speed;
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
        var fft = new FFTGenerator(audioProvider).GetFFT(filePath);
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
        var fft = new FFTGenerator(audioProvider).GetFFT(filePath);
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