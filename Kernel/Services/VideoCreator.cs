using System.Drawing;
using System.Drawing.Imaging;
using FFMediaToolkit.Encoding;
using FFMediaToolkit.Graphics;

namespace Kernel.Services;

public class VideoCreator
{
    private readonly VideoEncoderSettings settings;
    
    public VideoCreator(VideoEncoderSettings settings)
    {
        this.settings = settings;
    }
    
    public void Create(IEnumerable<Bitmap> bitmaps, string filename)
    {
        using var file = MediaBuilder
            .CreateContainer(filename)
            .WithVideo(settings)
            .Create();
        
        foreach (var bitmap in bitmaps)
        {
            var rect = new Rectangle(Point.Empty, bitmap.Size);
            var bitLock = bitmap.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            var bitmapData = ImageData.FromPointer(bitLock.Scan0, ImagePixelFormat.Bgr24, bitmap.Size);

            file.Video.AddFrame(bitmapData);

            bitmap.UnlockBits(bitLock);
        }
    }
}