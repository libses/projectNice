using ILGPU.Algorithms.Random;

namespace Kernel.Domain.Settings;

public readonly struct RandomSettings
{
    public readonly RNGView<XorShift128Plus> RngView;
   
    public RandomSettings()
    {
        var r = new Random();
        RngView = RNG.Create<XorShift128Plus>(Gpu.GpuSingleton.Gpu, r).GetView(Gpu.GpuSingleton.Gpu.WarpSize);
    }
    
}