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
        public Mandelbrot(int x, int y) : base(x, y)
        {
        }

        public Bitmap GetBitmap()
        {
            var bmp = new Bitmap(x, y);
            var x2 = (double)(x / 2);
            var y2 = (double)(y / 2);
            var x4 = (double)(x / 4);
            var y4 = (double)(y / 4);
            for (int xx = 0; xx < x; xx++)
            {
                for (int yy = 0; yy < y; yy++)
                {
                    var a = (xx - x2) / x4;
                    var b = (yy - y2) / y4;
                    var z = new Complex(0, 0);
                    var c = new Complex(a, b);
                    for (int i = 0; i < 256; i++)
                    {
                        z = z * z + c;
                        var modulo = z.Imaginary * z.Imaginary + z.Real + z.Real;
                        if (modulo > 4)
                        {
                            bmp.SetPixel(xx, yy, Color.FromArgb(i, i, i));
                            break;
                        }

                        if (i == 255)
                        {
                            bmp.SetPixel(xx, yy, Color.FromArgb(i, i, i));
                        }
                    }
                }
            }

            return bmp;
        }
    }
}
