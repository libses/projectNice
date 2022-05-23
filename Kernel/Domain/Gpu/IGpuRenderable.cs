using System.Drawing;
using ILGPU;
using ILGPU.Runtime;
using Kernel.Domain.Utils;

namespace Kernel.Domain.Gpu;

public interface IGpuRenderable<TGpuRen, TSettings>
    where TGpuRen : IGpuRenderable<TGpuRen, TSettings>
    where TSettings : struct
{
    Size ImageSize { get; }
    TSettings Settings { get; set; }
    MemoryBuffer1D<int, Stride1D.Dense>? GetBuffer();
    TGpuRen CopyToBitmap(DirectBitmap bitmap);
    DirectBitmap ToBitmap();

    TGpuRen Apply();

    TGpuRen WithSettings(TSettings settings);
}