using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Domain.Render;
using Domain.Services;
using FFMediaToolkit.Encoding;

namespace ConsoleApp
{ 
    internal class Program
    {
        static void Main(string[] args)
        {
            var x = 1920;
            var y = 1080;
            var mandel = new Mandelbrot(x, y);
            var counter = 0;
            for (double i = 1d; i > 0.1d; i *= 0.993)
            {
                var bmp = mandel
                    .Config(new MandelbrotSettings(i, -0.74529, 0.113075, x, y))
                    .GetBitmap()
                    .Bitmap;

                bmp.SaveJPG100(String.Format("temp\\{0}.jpg", counter));
                bmp.Dispose();

                counter++;
            }

            var creator = new VideoCreator(@"C:\videos\example.mp4",
                new VideoEncoderSettings(1920, 1080, 44, VideoCodec.H265));

            var provider = new BitmapProvider("temp", "jpg", counter);
            creator.Create(provider.Get());
        }
    }

    public static class BitmapExtensions
    {
        public static void SaveJPG100(this Bitmap bmp, string filename)
        {
            EncoderParameters encoderParameters = new EncoderParameters(1);
            encoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100L);
            bmp.Save(filename, GetEncoder(ImageFormat.Jpeg), encoderParameters);
        }

        public static void SaveJPG100(this Bitmap bmp, Stream stream)
        {
            EncoderParameters encoderParameters = new EncoderParameters(1);
            encoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100L);
            bmp.Save(stream, GetEncoder(ImageFormat.Jpeg), encoderParameters);
        }

        public static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();

            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }

            return null;
        }
    }
}