using System.Drawing;
using System.Drawing.Imaging;

namespace Kernel.Domain.Utils;

public static class BitmapExtensions
{
    public static void SaveJPG100(this Bitmap bmp, string filename)
    {
        var encoderParameters = new EncoderParameters(1);
        encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, 100L);
        bmp.Save(filename, GetEncoder(ImageFormat.Jpeg), encoderParameters);
    }

    private static ImageCodecInfo GetEncoder(ImageFormat format)
        => ImageCodecInfo.GetImageDecoders().FirstOrDefault(codec => codec.FormatID == format.Guid);
}