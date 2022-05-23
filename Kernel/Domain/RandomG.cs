using System.Drawing;
using Kernel.Domain.Settings;
using Kernel.Domain.Utils;

namespace Kernel.Domain;

public class RandomG : Renderable<RandomG, RandomSettings>
{
    public override DirectBitmap GetBitmap()
    {
        var bmp = new DirectBitmap(Width, Height);
        var r = Settings.Random;
        for (var x = 0; x < Width; x++)
        {
            for (var y = 0; y < Height; y++)
            {
                var R = r.Next(Settings.Start, Settings.End);
                var G = r.Next(Settings.Start, Settings.End);
                var B = r.Next(Settings.Start, Settings.End);
                bmp.SetPixel(x, y, Color.FromArgb(R, G, B));
            }
        }  

        return bmp;
    }

    public RandomG(int width, int height) : base(width, height)
    {
    }
}