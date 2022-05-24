using FFMediaToolkit.Encoding;
using Kernel;
using Kernel.Domain;
using Kernel.Domain.Utils;
using Kernel.Services;
using System;
using System.Drawing;

namespace VideoGenerator
{
    internal class Program
    {
        static void Main(string[] args)
        {
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
            Console.WriteLine("Генерации на выбор: f, t, m, p, c");
            Console.WriteLine("Чтобы сгенерировать что-либо введите формулу генерации");
            var kernel = KernelBuilder.Create();
            kernel.ConfigureFFTGenerator(16000);
            kernel.ConfigureVideoCreator(x, y, 44, "ff\\bin");
            kernel.ConfigureBitmapProvider("temp", "jpg", count);
        }
    }
}