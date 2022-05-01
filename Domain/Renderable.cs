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
        public int x;
        public int y;
        public Renderable(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public Bitmap GetBitmap()
        {
            throw new NotImplementedException();
        }
    }
}
