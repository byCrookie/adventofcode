using System.Diagnostics;
using AdventOfCode.Day15;
using AdventOfCode.Days;
using AdventOfCode.Measure;
using AdventOfSpeed.Utils;

// BenchmarkRunner.Run<Benchmarks>();
// return;

await RunAsync<Part2>();
return;

async Task RunAsync<T>() where T : IPart
{
    var path = Input.Path<T>();
    var part = Activator.CreateInstance<T>();
    var before = Stopwatch.GetTimestamp();
    var input = await File.ReadAllTextAsync(path);
    var result = await part.RunAsync(new NoneMeasure(), input);
    Console.WriteLine(Stopwatch.GetElapsedTime(before));
    Console.WriteLine(result);
}