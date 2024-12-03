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
        var regex = MulRegex();
        var sum = 0;
        var dont = false;
        
        foreach (Match match in regex.Matches(input))
        {
            if (match.Groups["dont"].Success)
            {
                dont = true;
                continue;
            }
            
            if (match.Groups["do"].Success)
            {
                dont = false;
                continue;
            }
            
            if (dont)
            {
                continue;
            }
            
            var multiplicand = int.Parse(match.Groups["multiplicand"].Value);
            var multiplier = int.Parse(match.Groups["multiplier"].Value);
            sum += multiplicand * multiplier;
        }

        return Task.FromResult(new PartResult($"{sum}", $"Sum of all multiplications is {sum}"));
    }

    [GeneratedRegex(@"(mul\((?<multiplicand>\d*),(?<multiplier>\d*)\))|(?<dont>don't\(\))|(?<do>do\(\))")]
    private static partial Regex MulRegex();
}