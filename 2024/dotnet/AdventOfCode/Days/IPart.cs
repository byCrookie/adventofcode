using AdventOfCode.Measure;

namespace AdventOfCode.Days;

public interface IPart
{
    Task<PartResult> RunAsync(IMeasure measure, string input);
}