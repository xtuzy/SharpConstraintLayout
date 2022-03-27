using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpConstraintLayout.Maui.DebugTool
{
    /// <summary>
    /// Because VS17.2 have bug, can run net6-ios, so i add this for debug info to show at windows
    /// </summary>
    public class SimpleDebug
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
            System.Diagnostics.Debug.WriteLine(message);
        }

        public static void WriteLine(string tag, string message)
        {
            WriteLine($"{tag}: {message}");
        }
    }
}
