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

namespace VideoGenerator
{
    public enum Sign
    {
        Add,
        Multiply
    }

    public class ConsoleF
    {
        public static string[] Lines = new string[] { "720", "720", "606.mp3", "606.wav", "30", "C:\\videos\\dottrace.mp4", "f", "+", "t", "e" };
        public static int i = -1;

        public static void WriteLine(string a)
        {

        }

        public static string ReadLine()
        {
            i++;
            Console.WriteLine(Lines[i]);
            return Lines[i];
        }
    }

    internal class Program
    {
        //стирать консоль чтобы стирала
        //хелп
        static void Main(string[] args)
        {
            Console.WriteLine("На компьютере в папке C:\\ff\\ должен лежать кодек ffmpeg");
            Console.WriteLine("Укажите разрешение по горизонтали");
            var x = int.Parse(ConsoleF.ReadLine());
            Console.WriteLine("Укажите разрешение по вертикали");
            var y = int.Parse(ConsoleF.ReadLine());
            Console.WriteLine("Укажите имя входного mp3 файла");
            var mp3name = ConsoleF.ReadLine();
            Console.WriteLine("Укажите имя входного wav файла");
            var wavname = ConsoleF.ReadLine();
            Console.WriteLine("Укажите количество секунд генерации");
            var seconds = int.Parse(ConsoleF.ReadLine());
            var count = seconds * 44;
            Console.WriteLine("Укажите путь до выходного файла (example.mp4)");
            var path = ConsoleF.ReadLine();
            Console.WriteLine("Генерации на выбор: f, t, m, p, c");
            Console.WriteLine("Чтобы сгенерировать что-либо введите формулу генерации");
            Console.WriteLine("Построчно вводите добавляемые алгоритмы генерации и их сочетание");
            Console.WriteLine("Например \nf\n+\nc 210 200 100");
            Console.WriteLine("и e чтобы закончить ввод");
            Console.WriteLine("");
            Console.WriteLine("параметры: \nf пусто, \nt пусто, \nm пусто, \np количествоПланет минРазмер максРазмер \nc R G B");
            var fftR = new FFTGenerator(new WavAudioMonoProvider(16000));
            var fft = fftR.GetFFT(wavname);
            var read = ConsoleF.ReadLine();
            var ib = ImageBase.Create().Config(new ImageSettings(x, y));
            //Временный костыль, понял, что нужно сделать. Потом уберу енам.
            var sign = Sign.Add;
            var zoom = 1d;
            var mandel = new Mandelbrot(x, y).Config(new MandelbrotSettings(zoom, -0.74529, 0.113075, x, y));
            bool isMandelHere = false;
            while (read != "e")
            {
                var split = read.Split();
                var operation = split[0];
                switch (operation)
                {
                    case "f":
                        {
                            if (sign == Sign.Add)
                                ib = ib.Add<Funny>(z => new Funny(x, y).Config(new FunnySettings(fft)));
                            else if (sign == Sign.Multiply)
                                ib = ib.Multiply<Funny>(z => new Funny(x, y).Config(new FunnySettings(fft)));
                            break;
                        }
                    case "c":
                        {
                            var rgb = split.Skip(1).Select(x => int.Parse(x)).ToArray();
                            if (sign == Sign.Add)
                                ib = ib.Add<Constant>(z => new Constant(x, y).Config(new ConstantSettings(Color.FromArgb(rgb[0], rgb[1], rgb[2]))));
                            else if (sign == Sign.Multiply)
                                ib = ib.Multiply<Constant>(z => new Constant(x, y).Config(new ConstantSettings(Color.FromArgb(rgb[0], rgb[1], rgb[2]))));
                            break;
                        }
                    case "t":
                        {
                            if (sign == Sign.Add)
                                ib = ib.Add<ThreeD>(z => new ThreeD(x, y).Config(new ThreeDSettings(fft)));
                            else if (sign == Sign.Multiply)
                                ib = ib.Multiply<ThreeD>(z => new ThreeD(x, y).Config(new ThreeDSettings(fft)));
                            break;
                        }
                    case "m":
                        {
                            isMandelHere = true;
                            if (sign == Sign.Add)
                                ib = ib.Add<Mandelbrot>(x => mandel);
                            else if (sign == Sign.Multiply)
                                ib = ib.Multiply<Mandelbrot>(x => mandel);
                            break;
                        }
                    case "p":
                        {
                            var planetsConf = split.Skip(1).Select(x => int.Parse(x)).ToArray();
                            if (sign == Sign.Add)
                                ib = ib.Add<Planets>(z => new Planets(x, y).Config(new PlanetsSettings(planetsConf[0], planetsConf[1], planetsConf[2], new Random())));
                            else if (sign == Sign.Multiply)
                                ib = ib.Multiply<Planets>(z => new Planets(x, y).Config(new PlanetsSettings(planetsConf[0], planetsConf[1], planetsConf[2], new Random())));
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

                read = ConsoleF.ReadLine();
            }
            var kernel = KernelBuilder.Create();
            kernel.ConfigureFFTGenerator(16000);
            kernel.ConfigureVideoCreator(x, y, 44, "C:\\ff\\bin");
            kernel.ConfigureBitmapProvider("temp", "jpg", count);
            if (!Directory.Exists("temp"))
            {
                Directory.CreateDirectory("temp");
            }

            for (int i = 0; i < count; i++)
            {
                if (i % 20 == 0)
                {
                    Console.WriteLine(i / (double)count);
                }

                if (isMandelHere)
                {
                    zoom = zoom * 0.993;
                    mandel = mandel.Config(new MandelbrotSettings(zoom, mandel.Settings.A, mandel.Settings.B, x, y));
                    mandel.Apply();
                }

                var bitmap = ib.GetBitmap();
                bitmap.Bitmap.SaveJPG100("temp/" + i + ".jpg");
                //bitmap.Dispose();
            }

            var vc = new VideoCreator(new VideoEncoderSettings(x, y, 44, VideoCodec.H265));
            var prov = new BitmapProvider("temp", "jpg", count);
            vc.CreateWithSound(prov.Get(), path, mp3name);
        }
    }
}