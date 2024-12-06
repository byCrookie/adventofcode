using AdventOfCode.Days;
using AdventOfCode.Measure;

namespace AdventOfCode.Day06;

[Day(6)]
[Part(2)]
public class Part2 : IPart
{
    public Task<PartResult> RunAsync(IMeasure measure, string input)
    {
        var sum = 1;
        return Task.FromResult(new PartResult($"{sum}", $"Sum of all multiplications is {sum}"));
    }
}