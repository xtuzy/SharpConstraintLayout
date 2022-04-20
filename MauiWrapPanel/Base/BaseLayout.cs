using Microsoft.Maui.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiWrapPanel.Base
{
    public partial class BaseLayout
    {
        public static bool MEASURE = false;
        public static bool DEBUG = false;
        protected virtual Size MauiMeasure(Size avaliableSize)
        {
            throw new NotImplementedException();
        }

        protected virtual Size MauiLayout(Rect rect)
        {
            throw new NotImplementedException();
        }
    }
}
