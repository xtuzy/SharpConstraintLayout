using SharpConstraintLayout.Maui.DebugTool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpConstraintLayout.Maui.Widget;

namespace SharpConstraintLayout.Maui.Native.Example.Tool
{
    internal class UIThread
    {
        internal static void Invoke(Action action, ConstraintLayout constraintLayout)
        {
            constraintLayout.Dispatcher.Dispatch(() =>
            {
                action.Invoke();
            });
        }
    }
}
