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
#if !ANDROID && !WINDOWS && !IOS && !MACCATALYST
            action.Invoke();
#else
            constraintLayout.Dispatcher.Dispatch(() =>
            {
                action.Invoke();
            });
#endif
#elif WINDOWS        
            if (constraintLayout.DispatcherQueue == null)
            {
                throw new Exception("UIThread.Invoke: ConstraintLayout.DispatcherQueue == null");
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
