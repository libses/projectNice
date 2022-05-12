namespace Domain.Render
{
    public class Constant : Renderable<Constant, ConstantSettings>
    {
        public override DirectBitmap GetBitmap()
        {
            var bmp = new DirectBitmap(Width, Height);
            for (var x = 0; x < Width; x++)
            for (var y = 0; y < Height; y++)
                bmp.SetPixel(x, y, Settings.Color);

            return bmp;
        }

        public Constant(int width, int height) : base(width, height)
        {
        }
    }
}