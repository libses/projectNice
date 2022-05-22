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
            Graphics.FromImage(bmp.Bitmap).Clear(Settings.Color);

            return bmp;
        }

        public Constant(int width, int height) : base(width, height)
        {
        }
    }
}