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
#if __MAUI__
            constraintLayout.Dispatcher.Dispatch(() =>
            {
                action.Invoke();
            });
#elif WINDOWS
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
#elif __IOS__
            CoreFoundation.DispatchQueue.MainQueue.DispatchAsync(() =>
                     {
                         action.Invoke();
                     });
#elif __ANDROID__
            constraintLayout.Post(() =>
            {
                action.Invoke();
            });
#endif
        }
    }
}
