using ILGPU;
using ILGPU.Runtime;

namespace Kernel.Domain.Gpu;

public static class GpuSingleton
{
    public static readonly Accelerator Gpu;

    static GpuSingleton()
    {
        var context = Context.CreateDefault();
        Gpu = context.GetPreferredDevice(false).CreateAccelerator(context);
    }
}