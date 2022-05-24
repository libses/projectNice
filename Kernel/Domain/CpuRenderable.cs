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

        private readonly TContext? context;

        public TSettings Settings { get; set; }


        protected override MemoryBuffer1D<int, Stride1D.Dense> GetBuffer()
        {
            var data = GetBitmap().Data;
        return Gpu.GpuSingleton.Gpu.Allocate1D(data);
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

        public abstract override DirectBitmap GetBitmap();

        public abstract override DirectBitmap Update(DirectBitmap bitmap);
    }
}