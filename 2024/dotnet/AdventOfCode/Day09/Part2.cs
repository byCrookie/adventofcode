using AdventOfCode.Days;
using AdventOfCode.Measure;

namespace AdventOfCode.Day09;

[Day(9)]
[Part(2)]
public class Part2 : IPart
{
    public Task<PartResult> RunAsync(IMeasure measure, string input)
    {
        var checksum = 0;
        return Task.FromResult(new PartResult($"{checksum}", $"Checksum: {checksum}"));
    }
}