using AdventOfCode.Days;
using AdventOfCode.Measure;

namespace AdventOfCode.Day19;

[Day(19)]
[Part(2)]
public class Part2 : IPart
{
    public Task<PartResult> RunAsync(IMeasure measure, string input)
    {
        var (patterns, designs) = Parse(input);
        var combinations = designs.Sum(d => CanDesignBeBuilt(d, patterns));
        return Task.FromResult(new PartResult($"{combinations}", $"Possible Design Combinations: {combinations}"));
    }

    private static long CanDesignBeBuilt(string design, HashSet<string> patterns)
    {
        Console.WriteLine(design);
        var memory = new Dictionary<long, long>();
        return BuildDesign(0, design, patterns.Where(design.Contains).ToArray(), memory);
    }

    private static long BuildDesign(int index, ReadOnlySpan<char> design, string[] patterns,
        Dictionary<long, long> memory)
    {
        if (index >= design.Length)
        {
            return 1;
        }

        if (memory.TryGetValue(index, out var result))
        {
            return result;
        }

        var counter = 0L;
        foreach (var pattern in patterns)
        {
            if (index + pattern.Length > design.Length)
            {
                continue;
            }

            if (pattern.AsSpan().SequenceEqual(design.Slice(index, pattern.Length)))
            {
                counter += BuildDesign(index + pattern.Length, design, patterns, memory);
            }
        }

        memory[index] = counter;
        return counter;
    }

    private static (HashSet<string> patterns, List<string> designs) Parse(string input)
    {
        var patterns = new HashSet<string>();
        var designs = new List<string>();

        var patternMode = true;
        foreach (var line in input.Split(Environment.NewLine))
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                patternMode = false;
                continue;
            }

            if (patternMode)
            {
                patterns.UnionWith(line.Split(", "));
            }
            else
            {
                designs.Add(line);
            }
        }

        return (patterns, designs);
    }
}