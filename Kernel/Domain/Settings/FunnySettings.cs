namespace Kernel.Domain.Settings;

public struct FunnySettings
{
    public readonly IReadOnlyList<double[]> Fft;

    public FunnySettings(List<double[]> fft)
    {
        Fft = fft;
    }
}