using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public abstract class Renderable : IRenderable
    {
        public int Width;
        public int Height;
        public Renderable(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public Bitmap GetBitmap()
        {
            throw new NotImplementedException();
        }
    }
}
