using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SharpConstraintLayout.Maui.Native.Example.Tool
{
    internal class SimpleTest
    {
        internal static void AreEqual(double hope, double real, string method, string messages = "")
        {
           System.Diagnostics.Trace.WriteLine($"{method} {((int)hope == (int)real ? "Passed" : "Failed")}: {messages}, result: hope { hope} real { real} ");
        }
    }
}