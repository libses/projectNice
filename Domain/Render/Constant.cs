using System.Drawing;

namespace Domain.Render
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