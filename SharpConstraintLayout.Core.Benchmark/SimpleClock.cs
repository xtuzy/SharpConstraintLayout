using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
//https://stackoverflow.com/questions/969290/exact-time-measurement-for-performance-testing
namespace SharpConstraintLayout.Core.Benchmark
{
    public class SimpleClock
    {
        interface IStopwatch
        {
            bool IsRunning { get; }
            TimeSpan Elapsed { get; }

            void Start();
            void Stop();
            void Reset();
        }

        class TimeWatch : IStopwatch
        {
            Stopwatch stopwatch = new Stopwatch();

            public TimeSpan Elapsed
            {
                get { return stopwatch.Elapsed; }
            }

            public bool IsRunning
            {
                get { return stopwatch.IsRunning; }
            }

            public TimeWatch()
            {
                if (!Stopwatch.IsHighResolution)
                    throw new NotSupportedException("Your hardware doesn't support high resolution counter");

                //prevent the JIT Compiler from optimizing Fkt calls away
                long seed = Environment.TickCount;

                //use the second Core/Processor for the test
                Process.GetCurrentProcess().ProcessorAffinity = new IntPtr(2);

                //prevent "Normal" Processes from interrupting Threads
                Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;

                //prevent "Normal" Threads from interrupting this thread
                Thread.CurrentThread.Priority = ThreadPriority.Highest;
            }

            public void Start()
            {
                stopwatch.Start();
            }

            public void Stop()
            {
                stopwatch.Stop();
            }

            public void Reset()
            {
                stopwatch.Reset();
            }
        }

        class CpuWatch : IStopwatch
        {
            TimeSpan startTime;
            TimeSpan endTime;
            bool isRunning;

            public TimeSpan Elapsed
            {
                get
                {
                    if (IsRunning)
                        throw new NotImplementedException("Getting elapsed span while watch is running is not implemented");

                    return endTime - startTime;
                }
            }

            public bool IsRunning
            {
                get { return isRunning; }
            }

            public void Start()
            {
                startTime = Process.GetCurrentProcess().TotalProcessorTime;
                isRunning = true;
            }

            public void Stop()
            {
                endTime = Process.GetCurrentProcess().TotalProcessorTime;
                isRunning = false;
            }

            public void Reset()
            {
                startTime = TimeSpan.Zero;
                endTime = TimeSpan.Zero;
            }
        }

        public static string BenchmarkTime(Action action, int iterations = 100)
        {
            return Benchmark<TimeWatch>(action, iterations);
        }

        static string Benchmark<T>(Action action, int iterations) where T : IStopwatch, new()
        {
            //clean Garbage
            GC.Collect();

            //wait for the finalizer queue to empty
            GC.WaitForPendingFinalizers();

            //clean Garbage
            GC.Collect();

            //warm up
            action();

            var stopwatch = new T();
            var timings = new double[1];
            for (int i = 0; i < timings.Length; i++)
            {
                stopwatch.Reset();
                stopwatch.Start();
                for (int j = 0; j < iterations; j++)
                    action();
                stopwatch.Stop();
                timings[i] = stopwatch.Elapsed.TotalMilliseconds / iterations;
                Debug.WriteLine(timings[i] + " ms");
            }

            return ("\tNormalized Mean: " + timings.NormalizedMean().ToString("0.000") + " ms\n")
                  + "\tStandard    Dev: " + timings.StandardDeviation().ToString("0.000") + " ms";
        }

        public static void BenchmarkCpu(Action action, int iterations = 100)
        {
            Benchmark<CpuWatch>(action, iterations);
        }

    }

    public static class ICollectionExtension
    {
        public static double NormalizedMean(this ICollection<double> values)
        {
            if (values.Count == 0)
                return double.NaN;

            var deviations = values.Deviations().ToArray();
            var meanDeviation = deviations.Sum(t => Math.Abs(t.Item2)) / values.Count;
            return deviations.Where(t => t.Item2 > 0 || Math.Abs(t.Item2) <= meanDeviation).Average(t => t.Item1);
        }

        public static IEnumerable<Tuple<double, double>> Deviations(this ICollection<double> values)
        {
            if (values.Count == 0)
                yield break;

            var avg = values.Average();
            foreach (var d in values)
                yield return Tuple.Create(d, avg - d);
        }

        /// <summary>
        /// https://stackoverflow.com/questions/895929/how-do-i-determine-the-standard-deviation-stddev-of-a-set-of-values
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static double StandardDeviation(this IEnumerable<double> values)
        {
            double standardDeviation = 0;

            if (values.Any())
            {
                // Compute the average.     
                double avg = values.Average();

                // Perform the Sum of (value-avg)_2_2.      
                double sum = values.Sum(d => Math.Pow(d - avg, 2));

                // Put it all together.      
                standardDeviation = Math.Sqrt((sum) / (values.Count() - 1));
            }

            return standardDeviation;
        }
    }
}
