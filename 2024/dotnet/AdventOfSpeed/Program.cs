using System.Diagnostics;
using AdventOfCode.Day08;
using AdventOfCode.Measure;
using AdventOfSpeed.Day8;
using BenchmarkDotNet.Running;

// BenchmarkRunner.Run<Benchmarks>();
// return;

var before = Stopwatch.GetTimestamp();
var input = await File.ReadAllTextAsync(Path.Combine("data", "day08", "part02", "input.txt"));
var result = await new Part2Optimized().RunAsync(new NoneMeasure(), input);
Console.WriteLine(Stopwatch.GetElapsedTime(before));
Console.WriteLine(result);