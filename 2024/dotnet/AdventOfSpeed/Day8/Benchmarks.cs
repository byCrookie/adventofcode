using AdventOfCode.Day08;
using AdventOfCode.Measure;
using BenchmarkDotNet.Attributes;

namespace AdventOfSpeed.Day8;

[MemoryDiagnoser]
public class Benchmarks
{
    private readonly IMeasure _measure = new NoneMeasure();
    private readonly string _input = File.ReadAllText(Path.Combine("data", "day08", "part02", "input.txt"));

    [Benchmark(Baseline = true)]
    public Task Benchmark_Part2Optimized()
    {
        return new Part2Optimized().RunAsync(_measure, _input);
    }

    [Benchmark]
    public Task Benchmark_PartTwoOptimized()
    {
        return new Part2OptimizedSpans().RunAsync(_measure, _input);
    }
}