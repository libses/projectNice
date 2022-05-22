using FFMediaToolkit;
using FFMediaToolkit.Encoding;
using Kernel.Services;
using Kernel.Services.Interfaces;
using Ninject;

namespace Kernel;

public class KernelBuilder
{
    public IKernel Kernel { get; private set; }

    private KernelBuilder()
    {
        Kernel = new StandardKernel();
    }
    
    public static KernelBuilder Create()
    {
        var builder = new KernelBuilder();
        builder.Kernel.Bind<ImageBase>().ToSelf().InSingletonScope();

        return builder;
    }

    public KernelBuilder ConfigureVideoCreator(int width, int height, int fps, string ffmpegPath)
    {
        FFmpegLoader.FFmpegPath = ffmpegPath;
        Kernel.Bind<VideoEncoderSettings>()
            .ToConstant(new VideoEncoderSettings(width, height, fps, VideoCodec.H265));
        Kernel.Bind<VideoCreator>().ToSelf().InSingletonScope();

        return this;
    }

    public KernelBuilder ConfigureFFTGenerator(double multiplier)
    {
        Kernel.Bind<IWavAudioProvider>().To<WavAudioMonoProvider>()
            .InSingletonScope()
            .WithConstructorArgument("multiplier", multiplier);

        Kernel.Bind<FFTGenerator>().ToSelf();

        return this;
    }
}