using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpConstraintLayout.Maui
{
    public struct SizeI
    {
        public int Width;
        public int Height;

        public SizeI(int w, int h)
        {
            Width = w;
            Height = h;
        }

        public SizeI(Size size)
        {
            Width = (int)size.Width;
            Height = (int)size.Height;
        }

        public Size ToSize()
        {
            return new Size(Width, Height);
        }
    }
}
