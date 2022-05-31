using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Numerics;
using ILGPU;
using ILGPU.Runtime;
using Kernel.Domain.Gpu;

namespace Kernel.Domain
{
    public readonly struct MandelbrotSettings
    {
        public MandelbrotSettings(double scale, double a, double b, int width, int height)
        {
            Scale = scale;
            A = a;
            B = b;
            Width = width;
            Height = height;
        }

        public readonly int Width;
        public readonly int Height;
        public readonly double Scale;
        public readonly double A;
        public readonly double B;
    }

    public class Mandelbrot : GpuRenderable<Mandelbrot, MandelbrotSettings>
    {
        public Mandelbrot(int width, int height) : base(new Size(width, height), ComputeFromGpu)
        {
        }

        private static void ComputeFromGpu(Index1D index,
            MandelbrotSettings settings,
            ArrayView1D<Int32, Stride1D.Dense> buffer)
        {
            var z = Complex.Zero;
            var xxW = index.X % settings.Width / (double)settings.Width;
            var yyW = (double)index.X / settings.Width / settings.Height;
            var c = new Complex((xxW - 0.5) * settings.Scale + settings.A,
                (yyW - 0.5) * settings.Scale + settings.B);
            for (var i = 1; i < 256; i++)
            {
                z = z * z + c;
                if (i != 255 && z.Magnitude < 4) continue;
                var r = 255 - i;
                buffer[index] = r * 256 * 256 * 256 + r * 256 * 256 + r * 256 + r;
                break;
            }
        }
    }
}