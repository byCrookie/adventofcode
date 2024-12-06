﻿using AdventOfCode.Measure;
using BenchmarkDotNet.Attributes;

namespace AdventOfCode.Day03;

[MemoryDiagnoser(false)]
public class Benchmarks
{
    private readonly IMeasure _measure = new NoneMeasure();

    [Benchmark]
    public Task Benchmark_PartOne()
    {
        return new Day04.Part1().RunAsync(_measure, "");
    }

    [Benchmark]
    public Task Benchmark_PartTwo()
    {
        return new Day04.Part2().RunAsync(_measure, "");
    }
}