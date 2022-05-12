namespace Domain.Render
{
    public abstract class Renderable<TContext, TSettings> : Configurer<TSettings>, IRenderable
    where TContext : Renderable<TContext, TSettings>
    {
        protected readonly int Width;
        protected readonly int Height;
        
        private readonly TContext? context;

        protected Renderable(int width, int height)
        {
            Width = width;
            Height = height;
            context = this as TContext;
        }

        public TContext Config(TSettings configuration)
        {
            Settings = configuration;
            return context;
        }

        public abstract DirectBitmap GetBitmap();
    }
}