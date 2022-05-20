using System.Drawing;
using Domain.Settings;
using ILGPU;
using ILGPU.Runtime;

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

        public Constant(int width, int height) : base(width, height,Apply)
        {
        }

        private static void Apply(Index2D arg1, ConstantSettings arg2, ArrayView2D<Pixel, Stride2D.DenseX> arg3)
        {
            throw new NotImplementedException();
        }
    }
}