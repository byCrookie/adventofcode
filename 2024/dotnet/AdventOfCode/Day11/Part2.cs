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
        var stones = input.Split(' ').Select(long.Parse).ToDictionary(s => s, _ => 1L);

        for (var blink = 0; blink < blinks; blink++)
        {
            var newStones = new Dictionary<long, long>();

            foreach (var (stone, count) in stones)
            {
                if (stone == 0)
                {
                    if (!newStones.TryAdd(1, count))
                    {
                        newStones[1] += count;
                    }

                    continue;
                }

                var length = stone.Equals(0) ? 1L : (long)Math.Log10(stone) + 1L;
                if (length % 2 == 0L)
                {
                    // ReSharper disable once PossibleLossOfFraction
                    var x = (long)Math.Pow(10L, length / 2);
                    var left = stone / x;
                    var right = stone % x;
                    if (!newStones.TryAdd(left, count))
                    {
                        newStones[left] += count;
                    }

                    if (!newStones.TryAdd(right, count))
                    {
                        newStones[right] += count;
                    }

                    continue;
                }

                var newStone = stone * 2024;
                if (!newStones.TryAdd(newStone, count))
                {
                    newStones[newStone] += count;
                }
            }

            stones = newStones;
        }

        var result = stones.Sum(s => s.Value);
        return Task.FromResult(new PartResult($"{result}", $"Amount of stones after blinking: {result}"));
    }
}