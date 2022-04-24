#if __ANDROID__
using Android.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpConstraintLayout.Maui.Widget
{
    public partial class ConstraintHelper
    {
        public ConstraintHelper(Context context) : base(context)
        {
            init();
        }

        protected override void OnAttachedToWindow()
        {
            base.OnAttachedToWindow();
            WhenAttachedToWindow();
        }
    }
}
#endif