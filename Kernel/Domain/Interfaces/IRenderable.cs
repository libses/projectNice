using Kernel.Domain.Utils;

namespace Kernel.Domain.Interfaces;

public interface IRenderable
{
    DirectBitmap GetBitmap();
    DirectBitmap Update(DirectBitmap bitmap);
}