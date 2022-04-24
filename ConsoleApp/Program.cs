using Domain;
using System;

namespace ConsoleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var random = new RandomG();
            var ms = new MorozovSet();
            var first = random.GetBitmap();
            var second = random.GetBitmap();
            var third = random.GetBitmap();
            var bmp = ms.GetBitmap();
            bmp.Save("out.bmp");
        }
    }
}
