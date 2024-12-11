using AdventOfCode.Days;
using AdventOfCode.Measure;

namespace AdventOfCode.Day11;

[Day(11)]
[Part(2)]
public class Part2 : IPart
{
    public Task<PartResult> RunAsync(IMeasure measure, string input)
    {
        const long blinks = 75;
        var stones = input.Split(' ').Select(long.Parse);
        // Console.WriteLine($"Blink 0: {string.Join(", ", stones.Select(s => s.Num))}");

        for (var blink = 0; blink < blinks; blink++)
        {
            stones = stones.SelectMany(stone =>
            {
                if (stone == 0)
                {
                    return [1L];
                }

                var length = stone.Equals(0) ? 1L : (long)Math.Log10(stone) + 1L;
                if (length % 2 == 0L)
                {
                    // ReSharper disable once PossibleLossOfFraction
                    var x = (long)Math.Pow(10L, length / 2);
                    var left = stone / x;
                    var right = stone % x;
                    return [left, right];
                }

                return new[] { stone * 2024 };
            });

            // Console.WriteLine($"Blink {blink + 1}: {string.Join(", ", stones.Select(s => s.Num))}");
            // Console.WriteLine($"Blink {blink + 1}: {stones.Count()}");
            // Console.WriteLine($"Blink {blink + 1}");
        }

        var count = stones.Count();
        return Task.FromResult(new PartResult($"{count}", $"Amount of stones after blinking: {count}"));
    }
}