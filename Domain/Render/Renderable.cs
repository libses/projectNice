using System.Diagnostics;
using ILGPU;
using ILGPU.Runtime;

namespace Domain.Render
{
    //TODO     Все операции над картинками -- в видеокарте. IVAN
    //TODO Решить вопрос с аллокацией памяти. IVAN
    
    public abstract class Renderable<TContext, TSettings> : Configurer<TSettings>, IRenderable
        where TContext : Renderable<TContext, TSettings> where TSettings : struct
    {
        public readonly bool IsGpuRenderable;

        protected readonly int Width;

        protected readonly int Height;

        private readonly TContext? context;

        protected readonly Accelerator? Gpu;

        protected readonly Context? GpuContext;

        protected readonly Action<Index1D, TSettings, ArrayView1D<Int32, Stride1D.Dense>>? Kernel;


        public void Dispose()
        {
            Gpu?.Dispose();
            GpuContext?.Dispose();
        }

        protected Renderable(int width, int height,
            Action<Index1D, TSettings, ArrayView1D<Int32, Stride1D.Dense>> action)
        {
            if (!action.Method.IsStatic)
            {
                throw new ArgumentException("Метод должен быть статическим");
            }

            IsGpuRenderable = true;
            GpuContext = Context.CreateDefault();
            Gpu = GpuContext.GetPreferredDevice(false).CreateAccelerator(GpuContext);
            Kernel = Gpu.LoadAutoGroupedStreamKernel(action);
            Width = width;
            Height = height;
            context = this as TContext;
        }

        protected Renderable(int width, int height)
        {
            IsGpuRenderable = false;
            GpuContext = null;
            Gpu = null;
            Kernel = null;

            Height = height;
            Width = width;
            context = this as TContext;
        }

        public TContext Config(TSettings configuration)
        {
            Settings = configuration;
            return context;
        }

        public virtual DirectBitmap GetBitmap()
        {
            CheckGpu();
            using var buffer = Gpu!.Allocate1D<Int32>(Width * Height);
            Kernel!(buffer.IntExtent, Settings, buffer.View);
            return new DirectBitmap(buffer.GetAsArray1D(), Width, Height);
        }
        
        public DirectBitmap Update(DirectBitmap bitmap)
        {
            CheckGpu();
            using var buffer = Gpu!.Allocate1D<Int32>(Width * Height);
            Kernel!(buffer.IntExtent, Settings, buffer.View);
            buffer.CopyToCPU(bitmap.Data);
            return bitmap;
        }

        private void CheckGpu()
        {
            if (!IsGpuRenderable)
                throw new ApplicationException($"{GetType().Name} cannot be used on GPU"
                                               + "\n For use CPU you need override GetBimap method");
        }
    }
}