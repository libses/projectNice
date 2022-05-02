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
        private double centerX = 0; //[-1, 1]
        private double centerY = 0;

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

        public Bitmap GetBitmap(int x, int y, double scale)
        {
            centerX = (x - Width / 2d) / (Width / 2d); // [-1, 1]
            centerY = (y - Height / 2d) / (Height / 2d);
            var bmp = new Bitmap(Width, Height);
            var x2 = Width / 2d;
            var y2 = Height / 2d;
            var x4 = Width / 4d;
            var y4 = Height / 4d;
            for (int xx = 0; xx < Width; xx++)
            {
                for (int yy = 0; yy < Height; yy++)
                {
                    var a = (xx - x2) / x4 * (1 / scale) + centerX;
                    var b = (yy - y2) / y4 * (1 / scale) + centerY;
                    var z = new Complex(0, 0);
                    var c = new Complex(a, b);
                    var (i, inBounds) = CheckPoint(a, b);
                    if (inBounds)
                    {
                        bmp.SetPixel(xx, yy, Color.White);
                    }
                    else if (i > 1)
                    {
                        var r = (int)(350 - Math.Sqrt(i) * 200 % 255);
                        if (r > 255) r = 255;
                        var color = Color.FromArgb(r, 80, 100);
                        bmp.SetPixel(xx, yy, color);
                    }
                    else
                    {
                        bmp.SetPixel(xx, yy, Color.Black);
                    }
                    /*for (int i = 0; i < 256; i++)
                    {
                        z = z * z + c;
                        var modulo = z.Imaginary * z.Imaginary + z.Real * z.Real;
                        if (modulo > 4)
                        {
                            var r = i;
                            if (r < 50)
                                r = r * 5;
                            bmp.SetPixel(xx, yy, Color.FromArgb(r, 80, 100));
                            break;
                        }

                        if (i == 255)
                        {
                            bmp.SetPixel(xx, yy, Color.FromArgb(i, i, i));
                        }
                    }*/
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