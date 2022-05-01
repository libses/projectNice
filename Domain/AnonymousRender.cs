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
        private Bitmap bitmap;
        public AnonymousRender(Bitmap source)
        {
            bitmap = source;
        }

        public Bitmap GetBitmap()
        {
            return bitmap;
        }
    }
}
