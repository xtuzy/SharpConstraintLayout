#if __MAUI__
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Walterlv.WeakEvents;
using FrameworkElement = Microsoft.Maui.Controls.View;
namespace SharpConstraintLayout.Maui.Widget
{
    public partial class ConstraintHelper
    {
        public ConstraintHelper() : base()
        {
            init();
            this.Loaded += MovedToWindow;
            new FrameworkElementWeakEventManager(this).Unloaded += RemovedFromWindow;
        }

        private void MovedToWindow(object? sender, EventArgs e)
        {
            WhenAttachedToWindow();
        }

        private void RemovedFromWindow(object sender, EventArgs e)
        {
            this.Loaded -= MovedToWindow;
        }
    }

    class FrameworkElementWeakEventManager : WeakEventRelay<FrameworkElement>
    {
        public FrameworkElementWeakEventManager(FrameworkElement eventSource) : base(eventSource)
        {
        }

        private readonly WeakEvent<EventArgs> _Unloaded = new WeakEvent<EventArgs>();

        public event EventHandler Unloaded
        {
            add => Subscribe(o => o.Unloaded += OnUnloaded, () => _Unloaded.Add(value, value.Invoke));
            remove => _Unloaded.Remove(value);
        }

        private void OnUnloaded(object sender, EventArgs e) => TryInvoke(_Unloaded, sender, e);

        protected override void OnReferenceLost(FrameworkElement source)
        {
            source.Unloaded -= OnUnloaded;
        }
    }
}
#endif
