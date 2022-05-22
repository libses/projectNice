﻿using System.Drawing;
using System.Numerics;
using ILGPU;
using ILGPU.Runtime;
using Kernel.Domain.Gpu;
using Kernel.Domain.Settings;
using Kernel.Domain.Utils;

namespace Kernel.Domain
{
    public class Gradient : GpuRenderable<Gradient, Size>
    {
        private static void ComputeFromGpu(Index1D index,
            Size size,
            ArrayView1D<Int32, Stride1D.Dense> buffer)
        {
            var x = index.X % size.Width;
            var y = index.X / size.Width;

            var dx = x / 256d;
            var dy = y / 256d;

            var complex = new Complex(dx, dy);
            var res = (2 * complex.Phase / Math.PI).ToInt();

            buffer[index] = res;
        }

        public Gradient(int width, int height) : base(new Size(width, height), ComputeFromGpu)
        {
        }
    }
}