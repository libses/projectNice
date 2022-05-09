using System.Drawing;

namespace Domain
{
    public interface IRenderable
    {
        DirectBitmap GetBitmap();
    }
}
