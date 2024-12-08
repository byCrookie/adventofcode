using AdventOfCode.Day08;
using AdventOfCode.Measure;
using AdventOfSpeed.Utils;
using BenchmarkDotNet.Attributes;

namespace AdventOfSpeed.Day8;

[MemoryDiagnoser]
public class Benchmarks
{
    private readonly IMeasure _measure = new NoneMeasure();
    private readonly string _input = File.ReadAllText(Input.Path<Part2Optimized>());

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