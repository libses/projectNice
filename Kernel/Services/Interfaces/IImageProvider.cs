namespace Kernel.Services.Interfaces;

public interface IImageProvider<out TItem>
{
    IEnumerable<TItem> Get();
}