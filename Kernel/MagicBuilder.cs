using Ninject;

namespace Kernel;

public static class MagicBuilder
{
    public static StandardKernel ConfigureMagic()
    {
        var kernel = new StandardKernel();
        return kernel;
    }
}