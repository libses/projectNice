using ILGPU;
using ILGPU.Runtime;
using Kernel.Domain.Interfaces;

namespace Kernel.Domain.Utils;

public abstract class Combinable<T> : IRenderable
    where T : Combinable<T>
{
    protected abstract MemoryBuffer1D<int, Stride1D.Dense> GetBuffer();

    public T Add<TOther>(Combinable<TOther> other)
        where TOther : Combinable<TOther>
    {
        GpuOperations.AddKernel(GetBuffer().IntExtent, other.GetBuffer().View, other.GetBuffer().View);
        return (this as T)!;
    }

    public T Multiply<TOther>(Combinable<TOther> other)
        where TOther : Combinable<TOther>
    {
        GpuOperations.MulKernel(GetBuffer().IntExtent, other.GetBuffer().View, other.GetBuffer().View);
        return (this as T)!;
    }

    public abstract DirectBitmap GetBitmap();
    public abstract DirectBitmap Update(DirectBitmap bitmap);
}