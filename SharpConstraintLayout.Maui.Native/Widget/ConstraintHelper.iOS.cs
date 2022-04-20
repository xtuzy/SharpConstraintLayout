using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpConstraintLayout.Maui.Widget
{
    public partial class ConstraintHelper
    {
        public ConstraintHelper() : base()
        {
            init();
        }

        bool isOnAttachedToWindow = true;
        public override void MovedToWindow()
        {
            base.MovedToWindow();
            if (isOnAttachedToWindow)
            {
                WhenAttachedToWindow();
                isOnAttachedToWindow = false;
            }
        }
    }
}
