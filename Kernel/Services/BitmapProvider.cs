using System.Drawing;
using Kernel.Services.Interfaces;

namespace Kernel.Services;

public class BitmapProvider : IImageProvider<Bitmap>, IDisposable
{
    private readonly string dirname;
    private readonly string format;
    private readonly int count;
    private readonly List<Bitmap> images = new();

    public BitmapProvider(string dirname, string format, int count)
    {
        this.dirname = dirname;
        this.format = format;
        this.count = count;
    }

    public void Dispose()
    {
        images.ForEach(x => x.Dispose());
    }

    public IEnumerable<Bitmap> Get()
    {
        for (var i = 0; i < count; i++)
        {
            var bmp = (Bitmap)Image.FromFile($"{dirname}\\{i}.{format}");
            images.Add(bmp);
            yield return (Bitmap)bmp;
        }
            
    }
}