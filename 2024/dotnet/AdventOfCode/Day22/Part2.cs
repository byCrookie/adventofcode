using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using AdventOfCode.Days;
using AdventOfCode.Measure;

namespace AdventOfCode.Day22;

[Day(22)]
[Part(2)]
public partial class Part2 : IPart
{
    public Task<PartResult> RunAsync(IMeasure measure, string input)
    {
        return Task.FromResult(new PartResult($"{1}", $": {1}"));
    }
}