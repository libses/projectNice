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
            centerX = (x - Width / 2d) / (Width / 2d);
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
                    for (int i = 0; i < 256; i++)
                    {
                        z = z * z + c;
                        var modulo = z.Magnitude;
                        if (modulo > 4 || i == 255)
                        {
                            bmp.SetPixel(xx, yy, Color.FromArgb(i, i, i));
                            break;
                        }
                    }
                }
            }

            return bmp;
        }
    }
}