using System.Text.RegularExpressions;
using AdventOfCode.Days;
using AdventOfCode.Measure;

namespace AdventOfCode.Day03;

[Day(3)]
[Part(1)]
public partial class Part1 : IPart
{
    public Task<PartResult> RunAsync(IMeasure measure, string input)
    {
        var sum = MulRegex().Matches(input)
            .Aggregate(0, (acc, match) => acc + int.Parse(match.Groups["multiplicand"].Value)
                * int.Parse(match.Groups["multiplier"].Value));

        return Task.FromResult(new PartResult($"{sum}", $"Sum of all multiplications is {sum}"));
    }

    [GeneratedRegex(@"mul\((?<multiplicand>\d*),(?<multiplier>\d*)\)")]
    private static partial Regex MulRegex();
}