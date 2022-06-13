using System.Drawing;
using ILGPU;
using ILGPU.Runtime;
using Kernel.Domain.Gpu;
using Kernel.Domain.Settings;
using Kernel.Domain.Utils;

namespace Kernel.Domain;

public class RandomG : GpuRenderable<RandomG, RandomSettings>
{
    public RandomG(int width, int height) : base(new Size(width, height), ComputeFromGpu)
    {
    }

    private static void ComputeFromGpu(Index1D index, RandomSettings settings, ArrayView1D<int, Stride1D.Dense> data)
    {
        data[index] = settings.RngView.Next();
    }
}