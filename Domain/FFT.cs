using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class FFT : Renderable
    {
        public FFT(int width, int height) : base(width, height)
        {
        }

        public Bitmap GetBitmap(double[] vs, double min, double max)
        {
            var bmp = new Bitmap(Width, Height);
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    var color = ((vs.Sum() - min) / max) * 255;
                    var icolor = (int)color;
                    bmp.SetPixel(x, y, Color.FromArgb(icolor, icolor, icolor));
                }
            }

            return bmp;
        }
    }
}
