using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class AnonymousRender : IRenderable
    {
        private DirectBitmap bitmap;
        public AnonymousRender(DirectBitmap source)
        {
            bitmap = source;
        }

        public DirectBitmap GetBitmap()
        {
            return bitmap;
        }
    }
}
