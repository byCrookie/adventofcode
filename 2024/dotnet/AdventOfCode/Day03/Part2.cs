using System.Text.RegularExpressions;
using AdventOfCode.Days;
using AdventOfCode.Measure;

namespace AdventOfCode.Day03;

[Day(3)]
[Part(2)]
public partial class Part2 : IPart
{
    public Task<PartResult> RunAsync(IMeasure measure, string input)
    {
        var dont = false;
        var sum = MulRegex().Matches(input)
            .Aggregate(0, (acc, match) =>
            {
                if (match.Groups["dont"].Success) dont = true;
                else if (match.Groups["do"].Success) dont = false;
                else if (!dont)
                    return acc + int.Parse(match.Groups["multiplicand"].Value) *
                        int.Parse(match.Groups["multiplier"].Value);
                return acc;
            });

        return Task.FromResult(new PartResult($"{sum}", $"Sum of all multiplications is {sum}"));
    }

    [GeneratedRegex(@"(mul\((?<multiplicand>\d*),(?<multiplier>\d*)\))|(?<dont>don't\(\))|(?<do>do\(\))")]
    private static partial Regex MulRegex();
}