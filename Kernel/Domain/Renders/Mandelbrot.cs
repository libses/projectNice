using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Numerics;
using ILGPU;
using ILGPU.Runtime;
using Kernel.Domain.Gpu;
using Kernel.Domain.Utils;

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

    public readonly struct MandelbrotCPUSettings
    {
        public MandelbrotCPUSettings(double scale, double a, double b, int width, int height)
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

    public class MandelbrotCPU : Renderable<MandelbrotCPU, MandelbrotCPUSettings>
    {
        private DirectBitmap bmp;
        public MandelbrotCPU(int width, int height) : base(width, height)
        {
            bmp = new DirectBitmap(width, height);;
        }

        public override DirectBitmap GetBitmap()
        {
            for (int i = 0; i < bmp.Data.Length; i++)
            {
                bmp.Data[i] = 0;
            }

            for (int x = 0; x < Settings.Width * Settings.Height; x++)
            {
                var z = Complex.Zero;
                var xxW = x % Settings.Width / (double)Settings.Width;
                var yyW = x / Settings.Width / (double)Settings.Height;
                var c = new Complex((xxW - 0.5) * Settings.Scale + Settings.A,
                    (yyW - 0.5) * Settings.Scale + Settings.B);
                for (int i = 1; i < 256; i++)
                {
                    z = z * z + c;
                    var modulo = z.Imaginary * z.Imaginary + z.Real * z.Real;
                    if (modulo < 4) continue;
                    var r = 255 - i;
                    bmp.Data[x] = r * 256 * 256 * 256 + r * 256 * 256 + r * 256 + r;
                    break;
                }
            }
            

            return bmp;
        }
    }

    [SuppressMessage("ReSharper", "PossibleLossOfFraction")]
    public class Mandelbrot : GpuRenderable<Mandelbrot, MandelbrotSettings>
        /*Renderable<Mandelbrot, MandelbrotSettings>,*/
    {
        public Mandelbrot(int width, int height) : base(new Size(width, height), ComputeFromGpu)
        {
        }

        private static void ComputeFromGpu(Index1D index,
            MandelbrotSettings settings,
            ArrayView1D<Int32, Stride1D.Dense> buffer)
        {
            var z = Complex.Zero;
            var xxW = index.X % settings.Width / (double) settings.Width;
            var yyW = index.X / settings.Width / (double) settings.Height;
            var c = new Complex((xxW - 0.5) * settings.Scale + settings.A,
                (yyW - 0.5) * settings.Scale + settings.B);
            for (int i = 1; i < 256; i++)
            {
                if (i == 255)
                {
                    buffer[index] = 0;
                }

                z = z * z + c;
                var modulo = z.Imaginary * z.Imaginary + z.Real * z.Real;
                if (!(modulo >= 4)) continue;
                var r = 255 - i;
                buffer[index] = r * 256 * 256 * 256 + r * 256 * 256 + r * 256 + r;
                break;
            }
        }
    }
}