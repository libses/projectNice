using ILGPU.Algorithms.Random;

namespace Kernel.Domain.Settings;

public readonly struct RandomSettings
{
    public readonly RNGView<XorShift128Plus> RngView;
    public readonly int Max;
    public readonly int Min;

    public RandomSettings(int min = int.MinValue, int max = int.MaxValue, Random? r = null)
    {
        r ??= new Random();
        RngView = RNG.Create<XorShift128Plus>(Gpu.GpuSingleton.Gpu, r).GetView(Gpu.GpuSingleton.Gpu.WarpSize);
        Min = min;
        Max = max;
    }
}