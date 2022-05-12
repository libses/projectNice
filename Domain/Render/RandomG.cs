using System.Drawing;

namespace Domain.Render
{
    public class RandomG : Renderable<RandomG, RandomSettings>
    {
        public override DirectBitmap GetBitmap()
        {
            var bmp = new DirectBitmap(Width, Height);
            for (var x = 0; x < Width; x++)
            for (var y = 0; y < Height; y++)
                bmp.SetPixel(
                    x,
                    y,
                    Color.FromArgb(Settings.Random.Next(Settings.Start, Settings.End))
                );

            return bmp;
        }

        public RandomG(int width, int height) : base(width, height)
        {
        }
    }
}