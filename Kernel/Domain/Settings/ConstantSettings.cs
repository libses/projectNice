using System.Drawing;

namespace Kernel.Domain.Settings;

public readonly struct ConstantSettings
{
    public readonly Int32 Color;

    public ConstantSettings(Int32 color)
    {
        Color = color;
    }

    public ConstantSettings(Color color)
    {
        Color = color.ToArgb();
    }
}