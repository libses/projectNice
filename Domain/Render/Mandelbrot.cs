using System.Drawing;
using System.Numerics;

namespace Domain.Render
{
    public record MandelbrotSettings(double Scale, double A, double B);

    public class Mandelbrot : Renderable<Mandelbrot, MandelbrotSettings>
    {
        private static int[] cycle = Enumerable.Range(0, 256).Select(x => x).ToArray();

        public override DirectBitmap GetBitmap()
        {
            var bmp = new DirectBitmap(Width, Height);
            for (var xx = 0; xx < Width; xx++)
            {
                for (var yy = 0; yy < Height; yy++)
                {
                    var z = new Complex(0, 0);
                    var xxW = xx / (double) Width;
                    var yyW = yy / (double) Height;
                    var c = new Complex((xxW - 0.5) * Settings.Scale + Settings.A,
                        (yyW - 0.5) * Settings.Scale + Settings.B);
                    foreach (var i in cycle)
                    {
                        z = z * z + c;
                        var modulo = z.Imaginary * z.Imaginary + z.Real * z.Real;
                        if (!(modulo >= 4)) continue;
                        var r = 255 - i;
                        bmp.SetPixel(xx, yy, Color.FromArgb(r, r, r, r));
                        break;
                    }
                }
            }

            return bmp;
        }

        public Mandelbrot(int width, int height) : base(width, height)
        {
        }
    }
}