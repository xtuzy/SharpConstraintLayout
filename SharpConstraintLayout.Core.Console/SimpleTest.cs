using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SharpConstraintLayout.Core.Benchmark
{
    internal class SimpleTest
    {
        internal static void AreEqual(double hope, double real, string method, string messages = "")
        {
            Console.WriteLine($"{method} {((int)hope == (int)real ? "Passed" : "Failed")}: {messages}, result: hope { hope} real { real} ");
        }

        internal static void AreEqual(double hope, double real, int detal, string method, string messages = "")
        {
            Console.WriteLine($"{method} {((Math.Abs((int)hope - (int)real)) <= detal ? "Passed" : "Failed")}: {messages}, result: hope { hope} real { real} ");
        }
    }
}