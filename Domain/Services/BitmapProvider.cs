using System.Drawing;

namespace Domain.Services;

public class BitmapProvider : IImageProvider<Bitmap>
{
    private readonly string dirname;
    private readonly string format;
    private readonly int count;

    public BitmapProvider(string dirname, string format, int count)
    {
        this.dirname = dirname;
        this.format = format;
        this.count = count;
    }
    
    public IEnumerable<Bitmap> Get()
    {
        for (var i = 0; i < count; i++)
            yield return (Bitmap)Image.FromFile($"{dirname}\\{i}.{format}");
    }
}