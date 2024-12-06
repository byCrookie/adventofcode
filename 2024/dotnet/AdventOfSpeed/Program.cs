using System.Diagnostics;
using AdventOfCode.Day06;
using AdventOfCode.Measure;

var input = await File.ReadAllTextAsync(Path.Combine("data", "day06", "part02", "input.txt"));
var before = Stopwatch.GetTimestamp();
var result = await new Part2().RunAsync(new NoneMeasure(), input);
Console.WriteLine(Stopwatch.GetElapsedTime(before));
Console.WriteLine(result);