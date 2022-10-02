#if WINDOWS && !__MAUI__
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Walterlv.WeakEvents;

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

        private void RemovedFromWindow(object sender, RoutedEventArgs e)
        {
            this.Loaded -= MovedToWindow;
        }

        public virtual void MovedToWindow(object sender, RoutedEventArgs e)
        {
            WhenAttachedToWindow();
        }
    }

    class FrameworkElementWeakEventManager : WeakEventRelay<FrameworkElement>
    {
        public FrameworkElementWeakEventManager(FrameworkElement eventSource) : base(eventSource)
        {
        }

        private readonly WeakEvent<RoutedEventArgs> _Unloaded = new WeakEvent<RoutedEventArgs>();

        public event RoutedEventHandler Unloaded
        {
            add => Subscribe(o => o.Unloaded += OnUnloaded, () => _Unloaded.Add(value, value.Invoke));
            remove => _Unloaded.Remove(value);
        }

        private void OnUnloaded(object sender, RoutedEventArgs e) => TryInvoke(_Unloaded, sender, e);

        protected override void OnReferenceLost(FrameworkElement source)
        {
            source.Unloaded -= OnUnloaded;
        }
    }
}
#endif