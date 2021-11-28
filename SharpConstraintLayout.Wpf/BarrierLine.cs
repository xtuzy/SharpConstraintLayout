using androidx.constraintlayout.core.widgets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SharpConstraintLayout.Wpf
{
    public class BarrierLine : FrameworkElement
    {
        public Barrier Barrier = new Barrier();
        public enum Side
        {
            Left = 0,
            Right = 1,
            Top = 2,
            Bottom = 3,
        }

        public Side BarrierSide
        {
            get
            {
                return (Side)Barrier.BarrierType;
            }

            set
            {
                Barrier.BarrierType = (int)value;
            }
        }

        public void AddView(FrameworkElement view)
        {
            var parent = Parent is ConstraintLayout ? Parent as ConstraintLayout : throw new ArgumentException($"{this} is not constraintlayout child");
            var widget = parent.GetWidget(view);
            Barrier.add(widget);
        }

        public void AddViews(FrameworkElement[] views)
        {
            var parent = Parent is ConstraintLayout ? Parent as ConstraintLayout : throw new ArgumentException($"{this} is not constraintlayout child");
            foreach (FrameworkElement view in views)
            {
                var widget = parent.GetWidget(view);
                Barrier.add(widget);
            }
        }
    }
}
