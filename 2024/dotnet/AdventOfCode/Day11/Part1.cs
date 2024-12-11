using AdventOfCode.Days;
using AdventOfCode.Measure;

namespace AdventOfCode.Day11;

[Day(11)]
[Part(1)]
public class Part1 : IPart
{
    public Task<PartResult> RunAsync(IMeasure measure, string input)
    {
        const long blinks = 25;
        var stones = input.Split(' ').Select(long.Parse).Select(n => new Stone(n, $"{n}")).ToList();
        // Console.WriteLine($"Blink 0: {string.Join(", ", stones.Select(s => s.Num))}");

        for (var blink = 0; blink < blinks; blink++)
        {
            var newStones = new List<Stone>();
            
            for (var i = 0; i < stones.Count; i++)
            {
                var stone = stones[i];

                if (stone.Num == 0)
                {
                    newStones.Add(new Stone(1, "1"));
                    continue;
                }

                if (stone.NumString.Length % 2 == 0)
                {
                    var left = long.Parse(stone.NumString[..(stone.NumString.Length / 2)]);
                    var right = long.Parse(stone.NumString[(stone.NumString.Length / 2)..]);
                    newStones.Add(new Stone(left, $"{left}"));
                    newStones.Add(new Stone(right, $"{right}"));
                    continue;
                }

                var number = stone.Num * 2024;
                newStones.Add(new Stone(number, $"{number}"));
            }

            stones = newStones;
            // Console.WriteLine($"Blink {blink + 1}: {string.Join(", ", stones.Select(s => s.Num))}");
            Console.WriteLine($"Blink {blink + 1}: {stones.Count}");
        }
        
        return Task.FromResult(new PartResult($"{stones.Count}", $"Amount of stones after blinking: {stones.Count}"));
    }

    private record struct Stone(long Num, string NumString);
}