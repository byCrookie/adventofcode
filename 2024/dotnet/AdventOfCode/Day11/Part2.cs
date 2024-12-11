using AdventOfCode.Days;
using AdventOfCode.Measure;

namespace AdventOfCode.Day11;

[Day(11)]
[Part(2)]
public class Part2 : IPart
{
    public Task<PartResult> RunAsync(IMeasure measure, string input)
    {
        var sum = 0;
        return Task.FromResult(new PartResult($"{sum}", $"Amount of stones after blinking: {sum}"));
    }
}