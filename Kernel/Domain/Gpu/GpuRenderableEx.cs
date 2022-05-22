using System.Runtime.CompilerServices;
using ILGPU;
using ILGPU.Runtime;

namespace Kernel.Domain.Gpu;

public static class GpuRenderableEx
{
    private static Action<Index1D, ArrayView1D<int, Stride1D.Dense>, ArrayView1D<int, Stride1D.Dense>> addKernel;
    private static Action<Index1D, ArrayView1D<int, Stride1D.Dense>, ArrayView1D<int, Stride1D.Dense>> mulKernel;

    private const uint AMask = 0b11111111_00000000_00000000_00000000;
    private const uint RMask = 0b00000000_11111111_00000000_00000000;
    private const uint GMask = 0b00000000_00000000_11111111_00000000;
    private const uint BMask = 0b00000000_00000000_00000000_11111111;

    static GpuRenderableEx()
    {
        addKernel =
            GpuSingleton.Gpu
                .LoadAutoGroupedStreamKernel<Index1D, ArrayView1D<int, Stride1D.Dense>,
                    ArrayView1D<int, Stride1D.Dense>>(Add);
        mulKernel =
            GpuSingleton.Gpu
                .LoadAutoGroupedStreamKernel<Index1D, ArrayView1D<int, Stride1D.Dense>,
                    ArrayView1D<int, Stride1D.Dense>>(Mul);
    }

    public static TGpuRen
        Multiply<TGpuRen, TSettings, TGpuRenOther, TSettingsOther>(
            this IGpuRenderable<TGpuRen, TSettings> th,
            IGpuRenderable<TGpuRenOther, TSettingsOther> other)
        where TGpuRenOther : IGpuRenderable<TGpuRenOther, TSettingsOther>
        where TGpuRen : class, IGpuRenderable<TGpuRen, TSettings>
        where TSettingsOther : unmanaged
        where TSettings : unmanaged
    {
        CheckSizes(th, other);
        CheckAndInitBuffers(th, other);

        Mul(th.GetBuffer()!.IntExtent, th.GetBuffer()!.View, other.GetBuffer()!.View);

        return (th as TGpuRen)!;
    }

    public static TGpuRen
        Add<TGpuRen, TSettings, TGpuRenOther, TSettingsOther>(
            this IGpuRenderable<TGpuRen, TSettings> th,
            IGpuRenderable<TGpuRenOther, TSettingsOther> other)
        where TGpuRenOther : IGpuRenderable<TGpuRenOther, TSettingsOther>
        where TGpuRen : class, IGpuRenderable<TGpuRen, TSettings>
        where TSettingsOther : unmanaged
        where TSettings : unmanaged
    {
        CheckSizes(th, other);
        CheckAndInitBuffers(th, other);

        Add(th.GetBuffer()!.IntExtent, th.GetBuffer()!.View, other.GetBuffer()!.View);

        return (th as TGpuRen)!;
    }

    private static void CheckSizes<TGpuRen, TSettings, TGpuRenOther, TSettingsOther>(
        IGpuRenderable<TGpuRen, TSettings> th, IGpuRenderable<TGpuRenOther, TSettingsOther> other)
        where TGpuRen : class, IGpuRenderable<TGpuRen, TSettings>
        where TSettings : unmanaged
        where TGpuRenOther : IGpuRenderable<TGpuRenOther, TSettingsOther>
        where TSettingsOther : unmanaged
    {
        if (th.ImageSize != other.ImageSize)
            throw new InvalidOperationException($"{th.ImageSize} not equal {other.ImageSize}");
    }

    private static void
        CheckAndInitBuffers<TGpuRen, TSettings, TGpuRenOther, TSettingsOther>(IGpuRenderable<TGpuRen, TSettings> th,
            IGpuRenderable<TGpuRenOther, TSettingsOther> other)
        where TGpuRenOther : IGpuRenderable<TGpuRenOther, TSettingsOther>
        where TGpuRen : class, IGpuRenderable<TGpuRen, TSettings>
        where TSettingsOther : unmanaged
        where TSettings : unmanaged
    {
        if (th.GetBuffer() is null) th.Apply();
        if (other.GetBuffer() is null) other.Apply();
    }

    private static void Mul(Index1D index, ArrayView1D<int, Stride1D.Dense> im1, ArrayView1D<int, Stride1D.Dense> im2)
    {
        var a = Crop(((im1[index] & AMask) >> 12) * ((im2[index] & AMask) >> 12));
        var r = Crop(((im1[index] & RMask) >> 8) * ((im2[index] & RMask) >> 8));
        var g = Crop(((im1[index] & GMask) >> 4) * ((im2[index] & GMask) >> 4));
        var b = Crop((im1[index] & BMask) * (im2[index] & BMask));

        im1[index] = (a << 12) | (r << 8) | (g << 4) | b;
    }

    private static void Add(Index1D index, ArrayView1D<int, Stride1D.Dense> im1, ArrayView1D<int, Stride1D.Dense> im2)
    {
        var a = Crop(((im1[index] & AMask) >> 12) + ((im2[index] & AMask) >> 12));
        var r = Crop(((im1[index] & RMask) >> 8) + ((im2[index] & RMask) >> 8));
        var g = Crop(((im1[index] & GMask) >> 4) + ((im2[index] & GMask) >> 4));
        var b = Crop((im1[index] & BMask) + (im2[index] & BMask));

        im1[index] = (a << 12) | (r << 8) | (g << 4) | b;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int Crop(long l)
    {
        if (l < 0) return 0;
        return l > 255 ? 255 : (int) l;
    }
}