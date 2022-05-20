﻿using System.Diagnostics;
using System.Drawing;
using System.Numerics;
using ILGPU;
using ILGPU.Runtime;

namespace Domain.Render
{
    public readonly struct MandelbrotSettings
    {
        public MandelbrotSettings(double scale, double a, double b, int width, int height)
        {
            Scale = scale;
            A = a;
            B = b;
            Width = width;
            Height = height;
        }

        public readonly int Width;
        public readonly int Height;
        public readonly double Scale;
        public readonly double A;
        public readonly double B;
    }

    public class Mandelbrot : Renderable<Mandelbrot, MandelbrotSettings>
    {
        private static int[] cycle = Enumerable.Range(0, 256).Select(x => x).ToArray();

        // public override DirectBitmap GetBitmap()
        // {
        //     var bmp = new DirectBitmap(Width, Height);
        //     var sw = new Stopwatch();
        //     sw.Start();
        //     for (var xx = 0; xx < Width; xx++)
        //     {
        //         for (var yy = 0; yy < Height; yy++)
        //         {
        //             var z = new Complex(0, 0);
        //             var xxW = xx / (double) Width;
        //             var yyW = yy / (double) Height;
        //             var c = new Complex((xxW - 0.5) * Settings.Scale + Settings.A,
        //                 (yyW - 0.5) * Settings.Scale + Settings.B);
        //             foreach (var i in cycle)
        //             {
        //                 z = z * z + c;
        //                 var modulo = z.Imaginary * z.Imaginary + z.Real * z.Real;
        //                 if (!(modulo >= 4)) continue;
        //                 var r = 255 - i;
        //                 bmp.SetPixel(xx, yy, Color.FromArgb(r, r, r, r));
        //                 break;
        //             }
        //         }
        //     }
        //     sw.Stop();
        //     Console.WriteLine($"Rendering from CPU\n{sw.Elapsed}");
        //     return bmp;
        // }

        public Mandelbrot(int width, int height) : base(width, height, ComputeFromGpu)
        {
        }

        private static void ComputeFromGpu(Index2D index,
            MandelbrotSettings settings,
            ArrayView2D<Pixel, Stride2D.DenseX> buffer)
        {
            var z = new Complex(0, 0);
            var xxW = index.X / (double) settings.Width;
            var yyW = index.Y / (double) settings.Height;
            var c = new Complex((xxW - 0.5) * settings.Scale + settings.A,
                (yyW - 0.5) * settings.Scale + settings.B);
            for (int i = 1; i < 256; i++)
            {
                z = z * z + c;
                var modulo = z.Imaginary * z.Imaginary + z.Real * z.Real;
                if (!(modulo >= 4)) continue;
                var r = 255 - i;
                buffer[index] = new Pixel(r, r, r, r);
                break;
            }
        }
    }
}