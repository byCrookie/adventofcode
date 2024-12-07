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

    public Task<PartResult> RunAsync(IMeasure measure, string input)
    {
        var sum = (
                from line in input.Split(Environment.NewLine)
                select line.Split(":")
                into parts
                let equationValue = long.Parse(parts[0])
                let numbers = parts[1]
                    .Split(" ", StringSplitOptions.RemoveEmptyEntries)
                    .Select(long.Parse)
                    .ToArray()
                where Operations.Aggregate(false,
                    (valid, operation) => valid | Compare(equationValue, numbers[1..], numbers[0], operation))
                select equationValue)
            .Sum();

        return Task.FromResult(new PartResult($"{sum}", $"Sum of valid equation values: {sum}"));
    }

    private static bool Compare(long equationValue, long[] numbers, long result, Func<long, long, long> operation)
    {
        if (numbers.Length == 0)
        {
            return result == equationValue;
        }

        return Operations.Aggregate(false,
            (valid, op) => valid | Compare(equationValue, numbers[1..], operation(result, numbers[0]), op));
    }
}