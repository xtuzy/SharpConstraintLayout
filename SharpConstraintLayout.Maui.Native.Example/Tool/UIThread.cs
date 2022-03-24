using Microsoft.Maui.Essentials;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpConstraintLayout.Maui.Native.Example.Tool
{
    internal class UIThread
    {
        internal static void Invoke(Action action)
        {
#if WINDOWS
            MainController.Current.DispatcherQueue.TryEnqueue(() =>
            {
                action.Invoke();
            });
#else
            MainThread.InvokeOnMainThreadAsync(() =>
            {
                action.Invoke();
            });
#endif
        }
    }
}
