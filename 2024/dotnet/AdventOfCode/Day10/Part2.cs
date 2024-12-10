using AdventOfCode.Days;
using AdventOfCode.Measure;

namespace AdventOfCode.Day10;

[Day(10)]
[Part(2)]
public class Part2 : IPart
{
        private static readonly List<Direction> Directions =
    [
        new(1, 0),
        new(0, 1),
        new(-1, 0),
        new(0, -1)
    ];

    public Task<PartResult> RunAsync(IMeasure measure, string input)
    {
        var field = CreateField(input);
        var trailheads = new List<Position>();
        for (var y = 0; y < field.Length; y++)
        {
            for (var x = 0; x < field[y].Length; x++)
            {
                if (field[y][x] == 0)
                {
                    trailheads.Add(new Position(x, y));
                }
            }
        }

        var sum = 0;
        foreach (var trailhead in trailheads)
        {
            var trailEnds = Trail(field, trailhead);
            var score = trailEnds.Count();
            // Console.WriteLine($"Score: {score}");
            sum += score;
        }

        return Task.FromResult(new PartResult($"{sum}", $"The sum of the scores of all trailheads is: {sum}"));
    }

    private static List<Position> Trail(int[][] field, Position step)
    {
        // PrintField(field, step);

        if (field[step.Y][step.X] == 9)
        {
            return [step];
        }

        var trailEnds = new List<Position>();
        foreach (var direction in Directions)
        {
            var nextStep = step + direction;

            if (!InBounds(field, nextStep))
            {
                continue;
            }

            if (field[nextStep.Y][nextStep.X] - field[step.Y][step.X] == 1)
            {
                var end = Trail(field, nextStep);
                if (end.Count > 0)
                {
                    trailEnds.AddRange(end);
                }
            }
        }

        return trailEnds;
    }

    private static bool InBounds(int[][] field, Position nextStep)
    {
        return nextStep.X >= 0 && nextStep.X < field[0].Length && nextStep.Y >= 0 && nextStep.Y < field.Length;
    }

    private static int[][] CreateField(string input)
    {
        var lines = input.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        var field = new int[lines.Length][];
        for (var i = 0; i < lines.Length; i++)
        {
            field[i] = lines[i].Select(c => int.Parse($"{c}")).ToArray();
        }

        return field;
    }

    private static void PrintField(int[][] field, Position position)
    {
        Console.Clear();

        for (var y = 0; y < field.Length; y++)
        {
            for (var x = 0; x < field[y].Length; x++)
            {
                if (position.X == x && position.Y == y)
                {
                    Console.Write("x");
                }
                else
                {
                    Console.Write(field[y][x]);
                }
            }

            Console.WriteLine();
        }
    }

    private record struct Position(int X, int Y)
    {
        public static Position operator +(Position position, Direction direction)
        {
            return new Position(position.X + direction.X, position.Y + direction.Y);
        }
    }

    private record struct Direction(int X, int Y);
}