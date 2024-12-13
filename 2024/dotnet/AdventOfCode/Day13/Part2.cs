﻿using System.Diagnostics;
using AdventOfCode.Days;
using AdventOfCode.Measure;

namespace AdventOfCode.Day13;

[Day(13)]
[Part(2)]
public class Part2 : IPart
{
    public Task<PartResult> RunAsync(IMeasure measure, string input)
    {
        return Task.FromResult(new PartResult($"{1}", $"Total price: {1}"));
    }
}