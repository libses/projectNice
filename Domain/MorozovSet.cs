using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class MorozovSet
    {
        public Bitmap GetBitmap()
        {
            var m = new Matrix4x4();
            var xSize = 256;
            var ySize = 256;
            var bmp = new Bitmap(xSize, ySize);
            for (int x = 0; x < xSize; x++)
            {
                for (int y = 0; y < ySize; y++)
                {
                    var b1 = x.ToBinary();
                    var b2 = y.ToBinary();
                    m.M11 = b1[0];
                    m.M12 = b1[1];
                    m.M13 = b1[2];
                    m.M14 = b1[3];
                    m.M21 = b1[4];
                    m.M22 = b1[5];
                    m.M23 = b1[6];
                    m.M24 = b1[7];
                    m.M31 = b2[0];
                    m.M32 = b2[1];
                    m.M33 = b2[2];
                    m.M34 = b2[3];
                    m.M41 = b2[4];
                    m.M42 = b2[5];
                    m.M43 = b2[6];
                    m.M44 = b2[7];
                    var det = (int)m.GetDeterminant();
                    bmp.SetPixel(x, y, Color.FromArgb(det));
                }
            }

            return bmp;
        }
    }
}
