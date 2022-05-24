using System.Diagnostics;
using ILGPU;
using ILGPU.Runtime;
using Kernel.Domain.Utils;

namespace Kernel.Domain
{
    public abstract class CpuRenderable<TContext, TSettings> : Combinable<TContext>
        where TContext : CpuRenderable<TContext, TSettings>
    {
        protected readonly int Width;

        protected readonly int Height;

        protected MemoryBuffer1D<int, Stride1D.Dense>? buffer;

        private readonly TContext? context;


        public TSettings Settings { get; set; }


        protected override MemoryBuffer1D<int, Stride1D.Dense> GetBuffer()
        {
            if (buffer is null || buffer.IsDisposed)
            {
                buffer = Gpu.GpuSingleton.Gpu.Allocate1D(GetBitmap().Data);
            }

            return buffer;
        }

        protected CpuRenderable(int width, int height)
        {
            Height = height;
            Width = width;
            context = this as TContext;
        }

        public TContext Config(TSettings configuration)
        {
            Settings = configuration;
            return context;
        }

        public override DirectBitmap GetBitmap()
        {
            if (buffer is not null && !buffer.IsDisposed)
            {
                var data = buffer.GetAsArray1D();
                buffer.Dispose();
                return new DirectBitmap(data, Width, Height);
            }

            return Process(new DirectBitmap(Width, Height));
        }

        public override DirectBitmap Update(DirectBitmap bitmap)
        {
            if (buffer is not null && !buffer.IsDisposed)
            {
                var data = buffer.GetAsArray1D();
                buffer.Dispose();
                bitmap.Data = data;
                return bitmap;
            }

            return Process(bitmap);
        }

        protected abstract DirectBitmap Process(DirectBitmap bitmap);
    }
}