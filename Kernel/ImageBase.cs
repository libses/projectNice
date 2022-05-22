using Kernel.Domain.Interfaces;
using Kernel.Domain.Settings;
using Kernel.Domain.Utils;

namespace Kernel;

public class ImageBase : Configurer<ImageSettings>
{
    private readonly List<(Type, Action<DirectBitmap, DirectBitmap>, Delegate)> items;
    
    private ImageBase()
    {
        items = new List<(Type, Action<DirectBitmap, DirectBitmap>, Delegate)>();
    }

    public static ConfigurationContext<ImageBase, ImageBase, ImageSettings> Create()
    {
        var imageBase = new ImageBase();
        return new ConfigurationContext<ImageBase, ImageBase, ImageSettings>(
            imageBase,
            imageBase
        );
    }

    public ImageBase Add<TRenderable>(Func<TRenderable, TRenderable> renderable)
    where TRenderable : IRenderable
    {
        items.Add(
            (typeof(TRenderable),
                (x, y) => x.Add(y),
                renderable)
        );
        return this;
    }


    public ImageBase Multiply<TRenderable>(Func<TRenderable, TRenderable> renderable)
    where TRenderable : IRenderable
    {
        items.Add(
            (typeof(TRenderable),
                (x, y) => x.Multiply(y),
                renderable)
        );
        return this;
    }

    public DirectBitmap GetBitmap()
    {
        var baseBitmap = new DirectBitmap(Settings.Width, Settings.Height);
        foreach (var (type, action, getter) in items)
        {
            var obj = type
                .GetConstructor(new[] {typeof(int), typeof(int)})?.Invoke(new[]
                {
                    Settings.Width, (object) Settings.Height
                });
            var renderable = getter.DynamicInvoke(obj) as IRenderable;
            action(baseBitmap, renderable.GetBitmap());
        }

        return baseBitmap;
    }
}

public abstract class Configurer<TSettings> : IConfigurer<TSettings>
{
    public TSettings Settings { get; protected set; }

    void IConfigurer<TSettings>.Set(TSettings settings)
    {
        Settings = settings;
    }
}

public class ConfigurationContext<TMainContext, TConfigureContext, TConfiguration>
    where TConfigureContext : IConfigurer<TConfiguration>
{
    private readonly TMainContext mainContext;
    private readonly TConfigureContext configureContext;

    public ConfigurationContext(TMainContext mainContext, TConfigureContext configureContext)
    {
        this.mainContext = mainContext;
        this.configureContext = configureContext;
    }

    public TMainContext Config(TConfiguration configuration)
    {
        configureContext.Set(configuration);
        return mainContext;
    }
}

public interface IConfigurer<in TConfiguration>
{
    void Set(TConfiguration configuration);
}