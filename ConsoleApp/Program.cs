using AnimatedGif;
using Domain;
using System;
using System.Drawing;
using System.IO;

namespace ConsoleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //new RandomG().Add(new Gradient()).Multiply(new Constant()).GetBitmap().Save("aboba.bmp");
            //using (var gif = AnimatedGif.AnimatedGif.Create("output.gif", 20))
            //{
            //    for (var i = 0; i < 100; i++)
            //    {
            //        Image img = new RandomG().GetBitmap();
            //        gif.AddFrame(img, delay: -1, quality: GifQuality.Bit8);
            //    }
            //}
            new Mandelbrot(2048, 2048).GetBitmap().Save("aboba.bmp");
        }
    }
}
