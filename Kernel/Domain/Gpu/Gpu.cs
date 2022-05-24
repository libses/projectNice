using ILGPU;
using ILGPU.Runtime;

namespace Kernel.Domain.Gpu;

public static class GpuSingleton
{
    public static readonly Accelerator Gpu;

    static GpuSingleton()
    {
        var context = Context.Create()
            .EnableAlgorithms()
            .Math(MathMode.Default)
            .AllAccelerators();
#if RELEASE
        Gpu = context.ToContext().GetPreferredDevice(false).CreateAccelerator(context.ToContext());
#endif
#if DEBUG
        context.Debug();
        Gpu = context.ToContext().Devices.First(d => d.AcceleratorType == AcceleratorType.CPU)
            .CreateAccelerator(context.ToContext());
#endif
    }
}