using System.Diagnostics;
using ILGPU;
using ILGPU.Runtime;

namespace Domain.Render
{
    public abstract class Renderable<TContext, TSettings> : Configurer<TSettings>, IRenderable
        where TContext : Renderable<TContext, TSettings> where TSettings : struct
    {
        public readonly bool IsGpuRenderable;

        protected readonly int Width;

        protected readonly int Height;

        private readonly TContext? context;

        protected readonly Accelerator? Gpu;

        protected readonly Context? GpuContext;

        protected readonly Action<Index2D, TSettings, ArrayView2D<Pixel, Stride2D.DenseX>>? Kernel;


        public void Dispose()
        {
            Gpu?.Dispose();
            GpuContext?.Dispose();
        }

        protected Renderable(int width, int height,
            Action<Index2D, TSettings, ArrayView2D<Pixel, Stride2D.DenseX>> action)
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
            if (!IsGpuRenderable)
                throw new ApplicationException($"{GetType().Name} cannot be used on GPU"
                                               + "\n For use CPU you need override GetBimap method");

            var sw = new Stopwatch();
            
            sw.Start();
            using var buffer = Gpu!.Allocate2DDenseX<Pixel>(new Index2D(Width, Height));
            Kernel!(buffer.IntExtent, Settings, buffer.View);
                //Gpu.Synchronize();
            sw.Stop();
            Console.WriteLine($"Rendering from {Gpu?.Name}");
            Console.WriteLine("-----> " + sw.Elapsed);

            return DirectBitmap.FromPixelArray(buffer.GetAsArray2D());
        }
    }
}