using System.Drawing;
using System.Numerics;
using Cudafy;

namespace Domain.Render
{
    public record MandelbrotSettings(float Scale, float A, float B);

    public class Mandelbrot : Renderable<Mandelbrot, MandelbrotSettings>
    {
        private static int[] cycle = Enumerable.Range(0, 256).Select(x => x).ToArray();


        public override DirectBitmap GetBitmap()
        {
            var bmp = new DirectBitmap(Width, Height);
            return DirectBitmap(bmp);
        }

        [Cudafy]
        private DirectBitmap DirectBitmap(DirectBitmap bmp)
        {
            for (var xx = 0; xx < Width; xx++)
            {
                for (var yy = 0; yy < Height; yy++)
                {
                    var z = new ComplexF(0, 0);
                    var xxW = xx / (float) Width;
                    var yyW = yy / (float) Height;
                    var c = new ComplexF((xxW - 0.5f) * Settings.Scale + Settings.A,
                        (yyW - 0.5f) * Settings.Scale + Settings.B);
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