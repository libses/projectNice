using FFMediaToolkit.Encoding;
using Kernel;
using Kernel.Domain;
using Kernel.Domain.Settings;
using Kernel.Domain.Utils;
using Kernel.Services;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using Kernel.Domain.Interfaces;
using Ninject;

namespace VideoGenerator
{
    public enum Sign
    {
        Add,
        Multiply
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("На компьютере в папке C:\\ff\\ должен лежать кодек ffmpeg");
            Console.WriteLine("Укажите разрешение по горизонтали");
            var x = int.Parse(Console.ReadLine());
            Console.WriteLine("Укажите разрешение по вертикали");
            var y = int.Parse(Console.ReadLine());
            Console.WriteLine("Укажите имя входного mp3 файла");
            var mp3name = Console.ReadLine();
            Console.WriteLine("Укажите имя входного wav файла");
            var wavname = Console.ReadLine();
            Console.WriteLine("Укажите количество кадров генерации");
            var count = int.Parse(Console.ReadLine());
            Console.WriteLine("Укажите путь до выходного файла (example.mp4)");
            var path = Console.ReadLine();
            Console.WriteLine("Генерации на выбор: f, t, m, p, c");
            Console.WriteLine("Чтобы сгенерировать что-либо введите формулу генерации");
            Console.WriteLine("Построчно вводите добавляемые алгоритмы генерации и их сочетание");
            Console.WriteLine("Например \nf\n+\nc 210 200 100");
            Console.WriteLine("и e чтобы закончить ввод");
            Console.WriteLine();
            Console.WriteLine(
                "параметры: \nf пусто, \nt пусто, \nm пусто, \np количествоПланет минРазмер максРазмер \nc R G B");
            var fftR = new FFTGenerator(new WavAudioMonoProvider(16000));
            var fft = fftR.GetFFT(wavname);
            var read = Console.ReadLine();
            var ib = ImageBase.Create().Config(new ImageSettings(x, y));
            //Временный костыль, понял, что нужно сделать. Потом уберу енам.
            var sign = Sign.Add;
            var zoom = 1d;
            var mandel = new Mandelbrot(x, y).Config(new MandelbrotSettings(zoom, -0.74529, 0.113075, x, y));
            bool isMandelHere = false;
            var kernel = KernelBuilder
                .Create()
                .ConfigureFFTGenerator(16000)
                .ConfigureVideoCreator(x, y, 44, "C:\\ff\\bin")
                .ConfigureBitmapProvider("temp", "jpg", count)
                .Kernel;
            kernel.Bind<ImageBase>().ToMethod(_ => ImageBase.Create().Config(new ImageSettings(x, y)));
            kernel.Bind<Builder>().ToSelf();

            while (read != "e")
            {
                var split = read.Split();
                var operation = split[0];
                switch (operation)
                {
                    case "f":
                    {
                        if (sign == Sign.Add)
                            kernel.Bind<IRenderable>().ToMethod(_ => new Funny(x, y).Config(new FunnySettings(fft)));
                        // ib = ib.Add<Funny>(z => new Funny(x, y).Config(new FunnySettings(fft)));
                        else if (sign == Sign.Multiply)
                            ib = ib.Multiply<Funny>(z => new Funny(x, y).Config(new FunnySettings(fft)));
                        break;
                    }
                    case "c":
                    {
                        var rgb = split.Skip(1).Select(x => int.Parse(x)).ToArray();
                        if (sign == Sign.Add)
                            kernel.Bind<IRenderable>().ToMethod(_ =>
                                new Constant(x, y).Config(
                                    new ConstantSettings(Color.FromArgb(rgb[0], rgb[1], rgb[2]))));

                        // ib = ib.Add<Constant>(z =>
                        //     new Constant(x, y).Config(new ConstantSettings(Color.FromArgb(rgb[0], rgb[1], rgb[2]))));
                        else if (sign == Sign.Multiply)
                            ib = ib.Multiply<Constant>(z =>
                                new Constant(x, y).Config(
                                    new ConstantSettings(Color.FromArgb(rgb[0], rgb[1], rgb[2]))));
                        break;
                    }
                    case "t":
                    {
                        if (sign == Sign.Add)
                            kernel.Bind<IRenderable>().ToMethod(_ => new ThreeD(x, y).Config(new ThreeDSettings(fft)));
                        // ib = ib.Add<ThreeD>(z => new ThreeD(x, y).Config(new ThreeDSettings(fft)));
                        else if (sign == Sign.Multiply)
                            ib = ib.Multiply<ThreeD>(z => new ThreeD(x, y).Config(new ThreeDSettings(fft)));
                        break;
                    }
                    case "m":
                    {
                        isMandelHere = true;
                        if (sign == Sign.Add)
                            kernel.Bind<IRenderable>().ToMethod(_ => mandel);
                        //     ib = ib.Add<Mandelbrot>(x => mandel);
                        else if (sign == Sign.Multiply)
                            ib = ib.Multiply<Mandelbrot>(x => mandel);
                        break;
                    }
                    case "p":
                    {
                        var planetsConf = split.Skip(1).Select(x => int.Parse(x)).ToArray();
                        if (sign == Sign.Add)
                            kernel.Bind<IRenderable>().ToMethod(_ => new Planets(x, y).Config(new PlanetsSettings(
                                planetsConf[0], planetsConf[1],
                                planetsConf[2], new Random())));
                        // ib = ib.Add<Planets>(z =>
                        //     new Planets(x, y).Config(new PlanetsSettings(planetsConf[0], planetsConf[1],
                        //         planetsConf[2], new Random())));
                        else if (sign == Sign.Multiply)
                            ib = ib.Multiply<Planets>(z =>
                                new Planets(x, y).Config(new PlanetsSettings(planetsConf[0], planetsConf[1],
                                    planetsConf[2], new Random())));
                        break;
                    }
                    case "+":
                    {
                        sign = Sign.Add;
                        break;
                    }
                    case "*":
                    {
                        sign = Sign.Multiply;
                        break;
                    }
                }

                read = Console.ReadLine();
            }

            if (!Directory.Exists("temp"))
            {
                Directory.CreateDirectory("temp");
            }

            for (int i = 0; i < count; i++)
            {
                if (isMandelHere)
                {
                    zoom = zoom * 0.99;
                    mandel = mandel.Config(new MandelbrotSettings(zoom, mandel.Settings.A, mandel.Settings.B, x, y));
                    mandel.Apply();
                }

                kernel.Get<Builder>().Build().GetBitmap().Bitmap.SaveJPG100("temp/" + i + ".jpg");
                //     ib.GetBitmap().Bitmap.SaveJPG100("temp/" + i + ".jpg");
            }

            var vc = new VideoCreator(new VideoEncoderSettings(x, y, 44, VideoCodec.H265));
            var prov = new BitmapProvider("temp", "jpg", count);
            vc.CreateWithSound(prov.Get(), path, mp3name);
        }
    }
}