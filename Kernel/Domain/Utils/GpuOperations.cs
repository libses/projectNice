using System.Runtime.CompilerServices;
using ILGPU;
using ILGPU.Runtime;
using Kernel.Domain.Gpu;

namespace Kernel.Domain.Utils;

public static class GpuOperations
{
    public static readonly Action<Index1D, ArrayView1D<int, Stride1D.Dense>, ArrayView1D<int, Stride1D.Dense>> AddKernel;
    public static readonly Action<Index1D, ArrayView1D<int, Stride1D.Dense>, ArrayView1D<int, Stride1D.Dense>> MulKernel;

    private const uint AMask = 0b11111111_00000000_00000000_00000000;
    private const uint RMask = 0b00000000_11111111_00000000_00000000;
    private const uint GMask = 0b00000000_00000000_11111111_00000000;
    private const uint BMask = 0b00000000_00000000_00000000_11111111;

    static GpuOperations()
    {
        AddKernel =
            GpuSingleton.Gpu
                .LoadAutoGroupedStreamKernel<Index1D, ArrayView1D<int, Stride1D.Dense>,
                    ArrayView1D<int, Stride1D.Dense>>(Add);
        MulKernel =
            GpuSingleton.Gpu
                .LoadAutoGroupedStreamKernel<Index1D, ArrayView1D<int, Stride1D.Dense>,
                    ArrayView1D<int, Stride1D.Dense>>(Mul);
    }


    private static void Mul(Index1D index, ArrayView1D<int, Stride1D.Dense> im1, ArrayView1D<int, Stride1D.Dense> im2)
    {
        var a = Crop(((im1[index] & AMask) >> 24) * ((im2[index] & AMask) >> 24));
        var r = Crop(((im1[index] & RMask) >> 16) * ((im2[index] & RMask) >> 16) );
        var g = Crop(((im1[index] & GMask) >> 8) * ((im2[index] & GMask) >> 8) );
        var b = Crop((im1[index] & BMask) * (im2[index] & BMask));

        im1[index] = (a << 24) | (r << 16) | (g << 8) | b;
    }

    private static void Add(Index1D index, ArrayView1D<int, Stride1D.Dense> im1, ArrayView1D<int, Stride1D.Dense> im2)
    {
        var a = Crop(((im1[index] & AMask) >> 24) + ((im2[index] & AMask) >> 24));
        var r = Crop(((im1[index] & RMask) >> 16) + ((im2[index] & RMask) >> 16) );
        var g = Crop(((im1[index] & GMask) >> 8) + ((im2[index] & GMask) >> 8));
        var b = Crop((im1[index] & BMask) + (im2[index] & BMask));

        im1[index] = (a << 24) | (r << 16) | (g << 8) | b;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int Crop(long l)
    {
        if (l < 0) return 0;
        return l > 255 ? 255 : (int) l;
    }
}