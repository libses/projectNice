using System.Drawing;
using Kernel.Domain.Settings;
using Kernel.Domain.Utils;

namespace Kernel.Domain
{
    public class Constant : Renderable<Constant, ConstantSettings>
    {
        public override DirectBitmap GetBitmap()
        {
            var bmp = new DirectBitmap(Width, Height);
            for (int i = 0; i < bmp.Data.Length; i++)
            {
                bmp.Data[i] = ((Color)Settings.Color).ToArgb();
            }

            return bmp;
        }

        public Constant(int width, int height) : base(width, height)
        {
        }
    }
}