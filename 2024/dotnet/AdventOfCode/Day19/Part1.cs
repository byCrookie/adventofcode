using AdventOfCode.Days;
using AdventOfCode.Measure;

namespace AdventOfCode.Day19;

[Day(19)]
[Part(1)]
public class Part1 : IPart
{
    public Task<PartResult> RunAsync(IMeasure measure, string input)
    {
        var (patterns, designs) = Parse(input);
        var count = designs.Count(d => CanDesignBeBuilt(d, [..patterns.Where(d.Contains)]));
        return Task.FromResult(new PartResult($"{count}", $"Possible Designs: {count}"));
    }

    private static bool CanDesignBeBuilt(string design, HashSet<string> patterns)
    {
        var memory = new Dictionary<int, bool>();
        return BuildDesign(0, design, patterns, memory);
    }

    private static bool BuildDesign(int index, string design, HashSet<string> patterns, Dictionary<int, bool> memory)
    {
        if (index == design.Length)
        {
            return true;
        }

        if (memory.TryGetValue(index, out var result))
        {
            return result;
        }

        if (patterns
            .Where(pattern => index + pattern.Length <= design.Length)
            .Where(pattern => pattern == design.Substring(index, pattern.Length))
            .Any(pattern => BuildDesign(index + pattern.Length, design, patterns, memory)))
        {
            memory[index] = true;
            return true;
        }

        memory[index] = false;
        return false;
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