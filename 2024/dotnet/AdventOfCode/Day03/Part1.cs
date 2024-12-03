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
        var regex = MulRegex();
        var sum = 0;

        foreach (Match match in regex.Matches(input))
        {
            var multiplicand = int.Parse(match.Groups["multiplicand"].Value);
            var multiplier = int.Parse(match.Groups["multiplier"].Value);
            sum += multiplicand * multiplier;
        }

        return Task.FromResult(new PartResult($"{sum}", $"Sum of all multiplications is {sum}"));
    }

    [GeneratedRegex(@"mul\((?<multiplicand>\d*),(?<multiplier>\d*)\)")]
    private static partial Regex MulRegex();
}