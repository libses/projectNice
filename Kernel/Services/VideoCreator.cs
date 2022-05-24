using System.Drawing;
using System.Drawing.Imaging;
using FFMediaToolkit;
using FFMediaToolkit.Audio;
using FFMediaToolkit.Decoding;
using FFMediaToolkit.Encoding;
using FFMediaToolkit.Graphics;
using Kernel.Services.Interfaces;

namespace Kernel.Services;

public class VideoCreator
{
    private readonly VideoEncoderSettings settings;
    
    public VideoCreator(VideoEncoderSettings settings)
    {
        this.settings = settings;
        FFmpegLoader.FFmpegPath = @"C:\ff\bin";
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

    public void CreateWithSound(IEnumerable<Bitmap> bitmaps, string filename, IWavAudioProvider provider, string audioPath)
    {
        var mf = MediaFile.Open(audioPath);
        var aes = new AudioEncoderSettings(mf.Audio.Info.SampleRate, mf.Audio.Info.NumChannels, AudioCodec.MP3);
        aes.SampleFormat = mf.Audio.Info.SampleFormat;
        aes.SamplesPerFrame = mf.Audio.Info.SamplesPerFrame;
        aes.Bitrate = (int)mf.Info.Bitrate;
        var codec = mf.Audio.Info.CodecName;
        using var file = MediaBuilder
            .CreateContainer(filename)
            .WithAudio(aes)
            .WithVideo(settings)
            .Create();

        foreach (var bitmap in bitmaps)
        {
            if (mf.Audio.TryGetNextFrame(out var frame))
            {
                var rect = new Rectangle(Point.Empty, bitmap.Size);
                var bitLock = bitmap.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
                var bitmapData = ImageData.FromPointer(bitLock.Scan0, ImagePixelFormat.Bgr24, bitmap.Size);

                file.Video.AddFrame(bitmapData);
                file.Audio.AddFrame(frame);
                bitmap.UnlockBits(bitLock);
            }
        }
    }
}