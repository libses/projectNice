namespace Domain.Services;

public interface IImageProvider<out TItem>
{
    IEnumerable<TItem> Get();
}