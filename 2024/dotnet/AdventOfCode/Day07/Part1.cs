using AdventOfCode.Days;
using AdventOfCode.Measure;

namespace AdventOfCode.Day07;

[Day(7)]
[Part(1)]
public class Part1 : IPart
{
    private static readonly List<Func<long, long, long>> Operations =
    [
        (a, b) => a + b,
        (a, b) => a * b
    ];

    public async Task<PartResult> RunAsync(IMeasure measure, string input)
    {
        var validTests = new List<long>();
        foreach (var line in input.Split(Environment.NewLine))
        {
            var parts = line.Split(":");
            var comp = long.Parse(parts[0]);
            var numbers = parts[1].Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToArray();
            var valid = false;
            foreach (var op in Operations)
            {
                 valid |= Compare(comp, numbers, 1, numbers[0], op);
            }
            
            if (valid)
            {
                validTests.Add(comp);
            }
        }

        var sum = validTests.Sum();
        return new PartResult($"{sum}", $"Count of additional obstacles that result in loop {sum}");
    }

    private static bool Compare(long comp, long[] numbers, long index, long result, Func<long, long, long> operation)
    {
        if (index == numbers.Length)
        {
            if (result == comp)
            {
                return true;
            }

            return false;
        }

        var current = numbers[index];
        result = operation(result, current);
        var valid = false;
        foreach (var op in Operations)
        {
            valid |= Compare(comp, numbers, index + 1, result, op);
        }

        return valid;
    }
}