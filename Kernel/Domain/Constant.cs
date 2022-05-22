using System.Drawing;
using ILGPU;
using ILGPU.Runtime;
using Kernel.Domain.Settings;
using Kernel.Domain.Utils;

namespace Kernel.Domain
{
    public class Constant : Renderable<Constant, ConstantSettings>
    {
        public Constant(int width, int height) : base(width, height, FromGpu)
        {
        }

        private static void FromGpu(Index1D index, ConstantSettings settings, ArrayView1D<int, Stride1D.Dense> data)
        {
            data[index] = settings.Color;
        }
    }
}