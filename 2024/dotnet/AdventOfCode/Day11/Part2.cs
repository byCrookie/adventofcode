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
        var stones = input.Split(' ').Select(long.Parse).ToList();
        // Console.WriteLine($"Blink 0: {string.Join(", ", stones.Select(s => s.Num))}");

        for (var blink = 0; blink < blinks; blink++)
        {
            var newStones = new List<long>();

            for (var i = 0; i < stones.Count; i++)
            {
                var stone = stones[i];

                if (stone == 0)
                {
                    newStones.Add(1);
                    continue;
                }

                var stoneString = stone.ToString();
                if (stoneString.Length % 2 == 0)
                {
                    var left = long.Parse(stoneString[..(stoneString.Length / 2)]);
                    var right = long.Parse(stoneString[(stoneString.Length / 2)..]);
                    newStones.Add(left);
                    newStones.Add(right);
                    continue;
                }

                newStones.Add(stone * 2024);
            }

            stones = newStones;
            // Console.WriteLine($"Blink {blink + 1}: {string.Join(", ", stones.Select(s => s.Num))}");
            Console.WriteLine($"Blink {blink + 1}: {stones.Count}");
        }

        return Task.FromResult(new PartResult($"{stones.Count}", $"Amount of stones after blinking: {stones.Count}"));
    }
}