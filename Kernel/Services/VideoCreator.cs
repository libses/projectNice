using System.Drawing;
using System.Drawing.Imaging;
using FFMediaToolkit;
using FFMediaToolkit.Encoding;
using FFMediaToolkit.Graphics;

namespace Domain.Services;

public class VideoCreator
{
    public static string FFmpegPath = @"C:\ff\bin";
    
    private readonly string filename;
    private readonly VideoEncoderSettings settings;
    
    public VideoCreator(string filename, VideoEncoderSettings settings)
    {
        FFmpegLoader.FFmpegPath = FFmpegPath;
        
        this.filename = filename;
        this.settings = settings;
    }
    
    public void Create(IEnumerable<Bitmap> bitmaps)
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