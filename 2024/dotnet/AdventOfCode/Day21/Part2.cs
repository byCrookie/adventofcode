using System.Diagnostics;
using System.Text;
using AdventOfCode.Days;
using AdventOfCode.Measure;

namespace AdventOfCode.Day21;

[Day(21)]
[Part(2)]
public class Part2 : IPart
{
    private static readonly Direction Up = new('^', 0, -1);
    private static readonly Direction Down = new('v', 0, 1);
    private static readonly Direction Left = new('<', -1, 0);
    private static readonly Direction Right = new('>', 1, 0);

    public Task<PartResult> RunAsync(IMeasure measure, string input)
    {
        return Task.FromResult(new PartResult($"{1}", $": {1}"));
    }

    private record struct Direction(char Symbol, int X, int Y);
}