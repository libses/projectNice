﻿using System.Drawing;
using ILGPU;
using ILGPU.Runtime;
using Kernel.Domain.Utils;

namespace Kernel.Domain.Gpu;

public interface IGpuRenderable<TGpuRen, TSettings>
    where TGpuRen : IGpuRenderable<TGpuRen, TSettings>
    where TSettings : unmanaged
{
    Action<Index1D, TSettings, ArrayView1D<int, Stride1D.Dense>>? Kernel { get; }
    Size ImageSize { get; }
    TSettings Settings { get; set; }
    MemoryBuffer1D<int, Stride1D.Dense>? GetBuffer();
    DirectBitmap ToBitmap();
    DirectBitmap CopyToBitmap(DirectBitmap bitmap);

    TGpuRen Apply();

    TGpuRen WithSettings(TSettings settings);
}