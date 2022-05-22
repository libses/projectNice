using System.Drawing;

namespace Kernel.Domain;

public readonly struct Pixel
{
    public readonly int R;
    public readonly int G;
    public readonly int B;
    public readonly int A;

    public Pixel(int r, int g, int b, int a)
    {
        R = r;
        G = g;
        B = b;
        A = a;
    }

    public Pixel(Color color)
    {
        R = color.R;
        G = color.G;
        B = color.B;
        A = color.A;
    }

    public static implicit operator Color(Pixel p) => Color.FromArgb(p.A, p.R, p.G, p.B);
}