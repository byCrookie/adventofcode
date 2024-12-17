using AdventOfCode.Days;
using AdventOfCode.Measure;

namespace AdventOfCode.Day17;

[Day(17)]
[Part(2)]
public class Part2 : IPart
{
    public Task<PartResult> RunAsync(IMeasure measure, string input)
    {
        return Task.FromResult(new PartResult($"{1}", $"Cost of path: {1}"));
    }
}