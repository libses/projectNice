namespace Kernel.Domain.Settings;

public readonly struct FFTSettings
{
    public readonly double[] vs;
    public readonly double min;
    public readonly double max;
    public readonly double sum;

    public FFTSettings(double[] vs, double min, double max, double sum)
    {
        this.vs = vs;
        this.min = min;
        this.max = max;
        this.sum = sum;
    }
}