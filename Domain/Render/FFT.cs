using System.Drawing;
using Domain.Render;

namespace Domain
{
    public readonly struct FFTSettings
    {
        public readonly double[] vs;
        public readonly double min;
        public readonly double max;
        public readonly double sum;

        public FFTSettings(double[] vs, double min, double max, double sum)
        {
            this.vs = vs;
            this.min = min;
            this.max = max;
            this.sum = sum;
        }
    }
    
    public class FFT : Renderable<FFT, FFTSettings>
    {
        public override DirectBitmap GetBitmap()
        {
            var bmp = new DirectBitmap(Width, Height);
            for (var x = 0; x < Width; x++)
                for (var y = 0; y < Height; y++)
                {
                    var color = ((Settings.sum - Settings.min) / Settings.max) * 255;
                    var icolor = (int)color;
                    bmp.SetPixel(x, y, Color.FromArgb(icolor, icolor, icolor));
                }

            return bmp;
        }

        public FFT(int width, int height) : base(width, height)
        {
        }
    }
}
