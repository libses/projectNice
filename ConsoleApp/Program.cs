using FFMediaToolkit.Encoding;
using Kernel;
using Kernel.Domain;
using Kernel.Domain.Utils;
using Kernel.Services;

namespace ConsoleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var x = 1920;
            var y = 1080;
            var mandel = new Mandelbrot(x, y);
            var counter = 0;
            for (var i = 1d; i > 0.1d; i *= 0.993)
            {
                var bmp = mandel
                    .Config(new MandelbrotSettings(i, -0.74529, 0.113075, x, y))
                    .GetBitmap()
                    .Bitmap;

                bmp.SaveJPG100($"temp\\{counter}.jpg");
                counter++;
            }
        }
    }
}