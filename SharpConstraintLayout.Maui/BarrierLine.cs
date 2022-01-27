using Barrier = androidx.constraintlayout.core.widgets.Barrier;

namespace SharpConstraintLayout.Maui
{
    /// <summary>
    /// A BarrierLine takes multiple views
    /// </summary>
    public class BarrierLine : FrameworkElement
    {

        public readonly Barrier Barrier = new Barrier();

        public enum Side
        {
            Left,
            Right,
            Top,
            Bottom,
        }

        /// <summary>
        /// At this side set barrier
        /// </summary>
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

        /// <summary>
        /// Set barrier at view.<br/>
        /// it not add to visualtree, just add in flow to calculate layout.So you also should add view to ConstraintLayout.
        /// </summary>
        /// <param name="view"></param>
        /// <exception cref="ArgumentException"></exception>
        public void AddView(FrameworkElement view)
        {
            var parent = Parent is ConstraintLayout ? Parent as ConstraintLayout : throw new ArgumentException($"{this} is not constraintlayout child");
            var widget = parent.GetWidget(view);
            Barrier.add(widget);
        }

        /// <summary>
        /// Set barrier at views.<br/>
        /// it not add to visualtree, just add in flow to calculate layout.So you also should add view to ConstraintLayout.
        /// </summary>
        /// <param name="views"></param>
        /// <exception cref="ArgumentException"></exception>
        public void AddViews(params FrameworkElement[] views)
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
