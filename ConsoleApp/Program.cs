﻿using AnimatedGif;
using Domain;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int Fact(int n)
            {
                var r = 1;
                for (var i = 1; i < n; i++)
                {
                    r *= i;
                }
                return r;
            }

            var framesCount = 20;
            var imgs = new Bitmap[framesCount];
            var mb = new Mandelbrot(1000, 1000);
            var render = Parallel.For(0, framesCount,
                i =>
                {
                    imgs[i] = mb.GetBitmap(199, 284, 1 + Fact(i)/20d);
                });
            if (render.IsCompleted)
                using (var gif = AnimatedGif.AnimatedGif.Create("output.gif", 20))
                {
                    foreach (var img in imgs)
                    {
                        gif.AddFrame(img, delay: 200, quality: GifQuality.Bit8);
                    }
                }
        //    mb.GetBitmap(199, 284, 50000).Save("aboba.bmp");
        }
    }
}