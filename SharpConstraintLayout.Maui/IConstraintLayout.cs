using androidx.constraintlayout.core.widgets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpConstraintLayout.Maui
{
    public interface IConstraintLayout:IAbsoluteLayout
    {
        bool DEBUG { set; get; }
        bool TEST { set; get; }
        ConstraintWidgetContainer Root { get; }

        ConstraintWidget GetWidget(UIElement view);
    }
}
