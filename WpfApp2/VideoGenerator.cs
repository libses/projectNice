using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Kernel.Domain.Settings;
using Kernel.Domain;
using Kernel.Services;
using Kernel.Services.Interfaces;

namespace WpfApp2;

public class VideoGenerator
{
    private readonly string audioPath;
    private readonly IWavAudioProvider audioProvider;
    private readonly int width;
    private readonly int height;
    private List<double[]> fft;
    public int FftCount => fft.Count;

    private string TempFilesPath;

    public VideoGenerator(IWavAudioProvider audioProvider, int width, int height, string audioPath,
        string tempFilesPath)
    {
        TempFilesPath = tempFilesPath;
        if (!Directory.Exists(TempFilesPath))
            Directory.CreateDirectory(TempFilesPath);
        var di = new DirectoryInfo(TempFilesPath);
        foreach (var file in di.GetFiles())
        {
            file.Delete();
        }
        

        this.audioProvider = audioProvider;
        this.width = width;
        this.height = height;
        this.audioPath = audioPath;
        fft = new FFTGenerator(audioProvider).GetFFT(audioPath);
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

    public async void Funny()
    {
        var f = new Funny(width, height).Config(new FunnySettings(fft));
        await Task.Run(() =>
        {
            for (int i = 0; i < fft.Count; i++)
            {
                var bmp = f.GetBitmap().Bitmap;
                bmp.Save($@"{TempFilesPath}\{i}.bmp");
                bmp.Dispose();
            }
        });
    }

    public async void FunnyAnd()
    {
        var added =
            ImageBase.Create()
                .Config(new ImageSettings(width, height))
                .Add<Planets>(p => p.Config(new PlanetsSettings(20, 10, 100, new Random())))
                .Add<Funny>(p => p.Config(new FunnySettings(fft)));
        await Task.Run(() =>
        {
            for (var i = 0; i < fft.Count; i++)
            {
                var bmp = added.GetBitmap();
                bmp.Bitmap.Save($@"{TempFilesPath}\{i}.bmp");
                bmp.Dispose();
            }
        });
    }
    
    public async void Mandelbrot()
    {
        var added =
            ImageBase.Create()
                .Config(new ImageSettings(width, height))
                // .Add<Planets>(p => p.Config(new PlanetsSettings(20, 10, 100, Brushes.Chartreuse, new Random())))
                .Add<Funny>(p => p.Config(new FunnySettings(fft)))
                .Add<Mandelbrot>(m => m.Config(new MandelbrotSettings(1, 0, 0, width, height)))
            ;
        await Task.Run(() =>
        {
            for (var i = 0; i < fft.Count; i++)
            {
                var bmp = added.GetBitmap();
                bmp.Bitmap.Save($@"{TempFilesPath}\{i}.bmp");
                bmp.Dispose();
            }
        });
    }
}