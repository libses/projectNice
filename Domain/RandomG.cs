using System.Drawing;

namespace Domain
{
    public class RandomG : IRenderable
    {
        public DirectBitmap GetBitmap()
        {
            var r = new Random();
            var xSize = 1500;
            var ySize = 1500;
            var bmp = new DirectBitmap(xSize, ySize);
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
