using System.Drawing;
using Domain.Render;

namespace Domain.Settings;

public readonly struct ConstantSettings
{
    public readonly Pixel Color;

    public ConstantSettings(Pixel color)
    {
        Color = color;
    }

    public ConstantSettings(Color color)
    {
        Color = new Pixel(color);
    }
}