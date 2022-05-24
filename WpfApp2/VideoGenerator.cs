using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Kernel.Domain.Settings;
using Kernel.Domain;
using Kernel.Domain.Utils;
using Kernel.Services;
using Kernel.Services.Interfaces;

namespace WpfApp2;

public class VideoGenerator
{
    private readonly string audioPath;
    private readonly IWavAudioProvider audioProvider;
    private readonly int width;
    private readonly int height;

    private string TempFilesPath;

    public VideoGenerator(IWavAudioProvider audioProvider, int width, int height, string audioPath,
        string tempFilesPath)
    {
        TempFilesPath = tempFilesPath;
        if (!Directory.Exists(TempFilesPath))
            Directory.CreateDirectory(TempFilesPath);

        this.audioProvider = audioProvider;
        this.width = width;
        this.height = height;
        this.audioPath = audioPath;
    }


    public IEnumerable<int> Planets()
    {
        var f = new Planets(width, height)
            .Config(new PlanetsSettings(20, 10, 100, new Random()));
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
        var fft = new FFTGenerator(audioProvider).GetFFT(audioPath);
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
        var fft = new FFTGenerator(audioProvider).GetFFT(audioPath);
        var added =
            ImageBase.Create()
                .Config(new ImageSettings(width, height))
                .Add<Funny>(f => f.Config(new FunnySettings(fft)))
                .Add<ThreeD>(f => f.Config(new ThreeDSettings(fft)))
                .Add<Mandelbrot>(f => f.Config(new MandelbrotSettings(2d, 0, 0, width, height)))
                .Add<Constant>(c => c.Config(new ConstantSettings(Color.Brown)));
        for (int i = 0; i < fft.Count; i++)
        {
            var bmp = added.GetBitmap();
            bmp.Bitmap.Save($@"{TempFilesPath}\{i}.bmp");
            bmp.Dispose();
            yield return 1;
        }
    }
}