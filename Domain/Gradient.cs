using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Gradient : IRenderable
    {
        public Bitmap GetBitmap()
        {
            var xSize = 256;
            var ySize = 256;
            var bmp = new Bitmap(xSize, ySize);
            for (int x = 0; x < xSize; x++)
            {
                for (int y = 0; y < ySize; y++)
                {
                    var dx = x / 256d;
                    var dy = y / 256d;
                    var complex = new Complex(dx, dy);
                    var t = (2 * complex.Phase / Math.PI).ToInt();
                    
                    bmp.SetPixel(x, y, Color.FromArgb(t, t, t));
                }
            }

            return bmp;
        }
    }
}
