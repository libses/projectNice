using System.Drawing;
using System.Numerics;

namespace Domain.Render
{
    public class Gradient : Renderable<Gradient, GradientSettings>
    {
        public override DirectBitmap GetBitmap()
        {
            var bmp = new DirectBitmap(Width, Height);
            for (var x = 0; x < Width; x++)
            for (var y = 0; y < Height; y++)
            {
                var dx = x / 256f;
                var dy = y / 256f;
                var complex = new ComplexF(dx, dy);
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