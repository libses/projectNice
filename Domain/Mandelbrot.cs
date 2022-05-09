using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace Domain
{
    public class Mandelbrot : Renderable
    {
        private double scale = 1;

        private static int[] cycle = Enumerable.Range(0, 256).Select(x => x).ToArray();

        public Mandelbrot(int width, int height) : base(width, height)
        {
        }

        /*public Mandelbrot WithZoom(double scale)
        {
            this.scale = scale;
            return this;
        }

        public Mandelbrot InPosition(int x, int y)
        {
            centerX = (x - Width / 2d) / (Width / 2d);
            centerY = (y - Height / 2d) / (Height / 2d);
            return this;
        }*/

        public DirectBitmap GetBitmap(double scale, double a, double b)
        {
            var bmp = new DirectBitmap(Width, Height);
            for (int xx = 0; xx < Width; xx++)
            {
                for (int yy = 0; yy < Height; yy++)
                {
                    var z = new Complex(0, 0);
                    var xxW = xx / (double)Width;
                    var yyW = yy / (double)Height;
                    var c = new Complex((xxW - 0.5) * scale + a, (yyW - 0.5) * scale + b);
                    //var (i, inBounds) = CheckPoint(a, b);
                    //if (inBounds)
                    //{
                    //    bmp.SetPixel(xx, yy, Color.White);
                    //}
                    //else if (i > 1)
                    //{
                    //    var r = (int)(350 - Math.Sqrt(i) * 200 % 255);
                    //    if (r > 255) r = 255;
                    //    var color = Color.FromArgb(r, 80, 100);
                    //    bmp.SetPixel(xx, yy, color);
                    //}
                    //else
                    //{
                    //    bmp.SetPixel(xx, yy, Color.Black);
                    //}
                    foreach (int i in cycle)
                    {
                        z = z * z + c;
                        var modulo = z.Imaginary * z.Imaginary + z.Real * z.Real;
                        if (modulo >= 4)
                        {
                            var r = 255 - i;
                            bmp.SetPixel(xx, yy, Color.FromArgb(r, r, r, r));
                            break;
                        }
                    }
                }
            }

            return bmp;
        }

        private (int, bool) CheckPoint(double x, double y)
        {
            var z = new Complex(0, 0);
            var c = new Complex(x, y);
            var i = 0;
            var bounds = 2;
            var inBounds = true;
            while (i < 50 && inBounds)
            {
                z = z * z + c;
                i++;
                if (z.Imaginary > bounds)
                {
                    inBounds = false;
                }
            }

            return (i, inBounds);
        }
    }
}