using AdventOfCode.Days;
using AdventOfCode.Measure;

namespace AdventOfCode.Day20;

[Day(20)]
[Part(2)]
public class Part2 : IPart
{
    public Task<PartResult> RunAsync(IMeasure measure, string input)
    {
        return Task.FromResult(new PartResult($"{1}", $"Possible Design Combinations: {1}"));
    }
}