using System.Drawing;
using System.Numerics;
using Kernel.Domain.Settings;
using Kernel.Domain.Utils;

namespace Kernel.Domain
{
    public class Gradient : Renderable<Gradient, GradientSettings>
    {
        public override DirectBitmap GetBitmap()
        {
            var bmp = new DirectBitmap(Width, Height);
            for (var x = 0; x < Width; x++)
            for (var y = 0; y < Height; y++)
            {
                var dx = x / 256d;
                var dy = y / 256d;
                var complex = new Complex(dx, dy);
                var t = (2 * complex.Phase / Math.PI).ToInt();

                bmp.SetPixel(x, y, Color.FromArgb(t, t, t));
            }

            return bmp;
        }

        public Gradient(int width, int height) : base(width, height)
        {
        }
    }
}