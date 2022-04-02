// See https://aka.ms/new-console-template for more information
using BenchmarkDotNet.Running;
using SharpConstraintLayout.Core.Benchmark;

/*Console.WriteLine("Hello, World!");
var test = new FlowTest();
for (int i = 0; i < 100; i++)
{
    var data = Console.ReadLine();
    var strs = data.Split(',');
    test.measureFlowWrapNoneMemoryAnalysis(int.Parse(strs[0]), int.Parse(strs[1]));
}*/

//var summary = BenchmarkRunner.Run<FlowTest>();
Console.ReadLine();
//new FlowTest().measureFlowWrapNoneMemoryAnalysis1000Widget(1000, 1000);
//new FlowTest().measureFlowWrapChainMemoryAnalysis100x10Widget();
new FlowTest().Measure_Views_Benchmark();
Console.ReadLine();
GC.Collect();
Console.ReadLine();