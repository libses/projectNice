using System.Collections.Generic;
using Kernel.Domain.Interfaces;
using Kernel.Services;

namespace VideoGenerator;

public class Builder
{
    private readonly IEnumerable<IRenderable> renderables;
    private ImageBase imageBase;

    public Builder(IEnumerable<IRenderable> renderables, ImageBase imageBase)
    {
        this.renderables = renderables;
        this.imageBase = imageBase;
    }

    public ImageBase Build()
    {
        foreach (var r in renderables)
            imageBase.Add<IRenderable>(_ => r);

        return imageBase;
    }
}