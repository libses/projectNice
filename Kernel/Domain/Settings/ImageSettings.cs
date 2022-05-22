namespace Kernel.Domain.Settings;

public readonly struct ImageSettings
{
    public ImageSettings(int width, int height)
    {
        Width = width;
        Height = height;
    }

    public readonly int Width;
    public readonly int Height;
}