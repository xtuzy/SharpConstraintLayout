using SharpConstraintLayout.Maui.DebugTool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpConstraintLayout.Maui.Widget
{
    internal class UIThread
    {
        internal static void Invoke(Action action, ConstraintLayout constraintLayout)
        {
#if WINDOWS
            if (constraintLayout.DispatcherQueue == null)
            {
                SimpleDebug.WriteLine("UIThread.Invoke: ConstraintLayout.DispatcherQueue == null");
            }
            else
            {
                constraintLayout.DispatcherQueue.TryEnqueue(() =>
                {
                    action.Invoke();
                });
            }
#else
            CoreFoundation.DispatchQueue.MainQueue.DispatchAsync(() =>
                     {
                         action.Invoke();
                     });
#endif
        }
    }
}
