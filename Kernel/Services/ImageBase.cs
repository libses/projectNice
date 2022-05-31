using Kernel.Domain.Interfaces;
using Kernel.Domain.Settings;
using Kernel.Domain.Utils;

namespace Kernel.Services;

public class ImageBase : Configurer<ImageSettings>
{
    private readonly List<(Type type, Action<DirectBitmap, DirectBitmap> action, Delegate getter)> items;
    private bool isFirstTime = true;
    private readonly List<IRenderable> renderables = new List<IRenderable>();

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
        if (isFirstTime)
        {
            foreach (var (type, action, getter) in items)
            {
                var obj = type
                    .GetConstructor(new[] { typeof(int), typeof(int) })?.Invoke(new[]
                    {
                    Settings.Width, (object) Settings.Height
                    });
                var renderable = getter.DynamicInvoke(obj) as IRenderable;
                renderables.Add(renderable);
                action(baseBitmap, renderable.GetBitmap());
            }

            isFirstTime = false;
        }
        else
        {
            for (int i = 0; i < renderables.Count; i++)
            {
                var renderable = renderables[i];
                var action = items[i].action;
                action(baseBitmap, renderable.GetBitmap());
            }
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