using System.Drawing;
using ILGPU;
using ILGPU.Runtime;
using Kernel.Domain.Interfaces;
using Kernel.Domain.Utils;
using static ILGPU.Stride1D;

namespace Kernel.Domain.Gpu;

public abstract class GpuRenderable<TGpuRen, TSettings> : IGpuRenderable<TGpuRen, TSettings>, IDisposable, IRenderable
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

    private DirectBitmap bmp;

    public Size ImageSize { get; }

    public TSettings Settings { get; set; }

    public MemoryBuffer1D<int, Dense>? GetBuffer() => buffer;

    public DirectBitmap GetBitmap()
    {
        if (buffer is null) Apply();
        if (bmp is null) bmp = new DirectBitmap(buffer.GetAsArray1D(), ImageSize.Width, ImageSize.Height);
        else
        {
            bmp.Data = buffer.GetAsArray1D();
        }

        return bmp;
    }

    public TGpuRen CopyToBitmap(DirectBitmap bitmap)
    {
        if (bitmap.ImageSize != ImageSize) throw new ArgumentException($"{bitmap.ImageSize} not equal {ImageSize}");
        buffer.CopyToCPU(bitmap.Data);
        return (this as TGpuRen)!;
    }

    public TGpuRen Apply()
    {
        buffer ??= GpuSingleton.Gpu.Allocate1D<int>(ImageSize.Width * ImageSize.Height);
        Kernel(buffer.IntExtent, Settings, buffer.View);
        return (this as TGpuRen)!;
    }

    public TGpuRen Config(TSettings settings)
    {
        Settings = settings;
        return (this as TGpuRen)!;
    }

    public TGpuRen Add<TGpuRenOther, TSettingsOther>(GpuRenderable<TGpuRenOther, TSettingsOther> other)
        where TSettingsOther : struct where TGpuRenOther : class, IGpuRenderable<TGpuRenOther, TSettingsOther>
    {
        if (buffer is null) Apply();
        if (other.buffer is null) other.Apply();

        GpuOperations.AddKernel(buffer!.IntExtent, buffer.View, other.buffer!.View);

        return (this as TGpuRen)!;
    }

    public TGpuRen Multiply<TGpuRenOther, TSettingsOther>(GpuRenderable<TGpuRenOther, TSettingsOther> other)
        where TSettingsOther : struct where TGpuRenOther : class, IGpuRenderable<TGpuRenOther, TSettingsOther>
    {
        if (buffer is null) Apply();
        if (other.buffer is null) other.Apply();

        GpuOperations.MulKernel(buffer!.IntExtent, buffer.View, other.buffer!.View);

        return (this as TGpuRen)!;
    }

    public void Dispose()
    {
        buffer?.Dispose();
    }

    public DirectBitmap Update(DirectBitmap bitmap)
    {
        throw new NotImplementedException();
    }
}