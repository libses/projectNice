using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class RandomG : IRenderable
    {
        public Bitmap GetBitmap()
        {
            var r = new Random();
            var xSize = 256;
            var ySize = 256;
            var bmp = new Bitmap(xSize, ySize);
            for (int x = 0; x < xSize; x++)
            {
                for (int y = 0; y < ySize; y++)
                {
                    bmp.SetPixel(x, y, Color.FromArgb(r.Next(int.MinValue, int.MaxValue)));
                }
            }

            return bmp;
        }
    }
}
