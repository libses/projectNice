using System.Drawing;
using Kernel.Services.Interfaces;

namespace Kernel.Services;

public class BitmapProvider : IImageProvider<Bitmap>
{
    private readonly string dirname;
    private readonly string format;
    private readonly int count;
    private Bitmap prev = null;

    public BitmapProvider(string dirname, string format, int count)
    {
        this.dirname = dirname;
        this.format = format;
        this.count = count;
    }

    public IEnumerable<Bitmap> Get()
    {
        for (var i = 0; i < count; i++)
        {
            var bmp = (Bitmap)Image.FromFile($"{dirname}\\{i}.{format}");
            prev?.Dispose();
            prev = bmp;
            yield return (Bitmap)bmp;
        }
            
    }
}