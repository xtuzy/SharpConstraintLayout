using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiWrapPanel.DebugTool
{
    /// <summary>
    /// Because VS17.2 have bug, can't run net6-ios, so i add this for debug info to show at windows.
    /// When release, use Trace output log.
    /// </summary>
    internal class SimpleDebug
    {
        static MessageClient client = new MessageClient("192.168.0.144", 399);

        public static void WriteLine(string message)
        {
#if __IOS__
            client.SendMessage(message + "~iOS" + $"~{DateTime.Now}" + "\n");//~ split Log,Platform,Time
#elif ANDROID
            client.SendMessage(message + "~Android" + $"~{DateTime.Now}" + "\n");//~ split Log,Platform,Time
#else
            client.SendMessage(message + "~Windows" + $"~{DateTime.Now}" + "\n");//~ split Log,Platform,Time      
#endif
#if DEBUG
            System.Diagnostics.Debug.WriteLine(message);
#else
            Trace.WriteLine(message, "SharpConstraintLayout");
#endif
        }

        public static void WriteLine(string tag, string message)
        {
            WriteLine($"{tag}: {message}");
        }
    }
}
