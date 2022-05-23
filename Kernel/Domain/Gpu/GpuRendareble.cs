using System.Drawing;
using ILGPU;
using ILGPU.Runtime;
using Kernel.Domain.Utils;
using static ILGPU.Stride1D;

namespace Kernel.Domain.Gpu;

public abstract class GpuRenderable<TGpuRen, TSettings> : IGpuRenderable<TGpuRen, TSettings>, IDisposable
    where TGpuRen : class, IGpuRenderable<TGpuRen, TSettings>
    where TSettings : struct
{

    protected GpuRenderable(Size imageSize, Action<Index1D, TSettings, ArrayView1D<int, Dense>> kernel)
    {
        if (!kernel.Method.IsStatic) throw new ArgumentException("The method should be static");
        ImageSize = imageSize;
        Kernel = GpuSingleton.Gpu.LoadAutoGroupedStreamKernel(kernel);
    }

    public Action<Index1D, TSettings, ArrayView1D<int, Dense>> Kernel { get; }

    private MemoryBuffer1D<int, Dense>? buffer;
    public Size ImageSize { get; }

    public TSettings Settings { get; set; }

    public MemoryBuffer1D<int, Dense>? GetBuffer() => buffer;

    public DirectBitmap ToBitmap()
    {
        if (buffer is null) Apply();
        return new DirectBitmap(buffer.GetAsArray1D(), ImageSize.Width, ImageSize.Height);
    }

    public DirectBitmap CopyToBitmap(DirectBitmap bitmap)
    {
        if (bitmap.ImageSize != ImageSize) throw new ArgumentException($"{bitmap.ImageSize} not equal {ImageSize}");
        buffer.CopyToCPU(bitmap.Data);
        return bitmap;
    }

    public TGpuRen Apply()
    {
        buffer ??= GpuSingleton.Gpu.Allocate1D<int>(ImageSize.Height * ImageSize.Height);
        Kernel(buffer.IntExtent, Settings, buffer.View);
        return (this as TGpuRen)!;
    }

    public TGpuRen WithSettings(TSettings settings)
    {
        Settings = settings;
        return (this as TGpuRen)!;
    }

    public void Dispose()
    {
        buffer?.Dispose();
    }
}