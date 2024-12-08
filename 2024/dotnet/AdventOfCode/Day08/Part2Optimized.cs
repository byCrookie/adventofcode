using AdventOfCode.Days;
using AdventOfCode.Measure;

namespace AdventOfCode.Day08;

[Day(8)]
[Part(2)]
public class Part2Optimized : IPart
{
    public Task<PartResult> RunAsync(IMeasure measure, string input)
    {
        var field = CreateField(input);
        var frequencies = new Dictionary<char, List<Position>>();
        for (var y = 0; y < field.Length; y++)
        {
            for (var x = 0; x < field[y].Length; x++)
            {
                if (field[y][x] == '.') continue;
                if (!frequencies.TryGetValue(field[y][x], out var value))
                {
                    frequencies.Add(field[y][x], [new Position(x, y)]);
                }
                else
                {
                    value.Add(new Position(x, y));
                }
            }
        }

        var antinodes = new HashSet<Position>();
        foreach (var frequency in frequencies)
        {
            var antennaPairs = frequency.Value
                .SelectMany(_ => frequency.Value, (antenna1, antenna2) => (antenna1, antenna2))
                .Where(pair => pair.antenna1 != pair.antenna2);

            foreach (var pair in antennaPairs)
            {
                var distance = pair.antenna1 - pair.antenna2;

                var antinode1 = pair.antenna1 + distance;
                while (InBounds(antinode1, field))
                {
                    antinodes.Add(antinode1);
                    antinode1 += distance;
                }

                var antinode2 = pair.antenna1 - distance;
                while (InBounds(antinode2, field))
                {
                    antinodes.Add(antinode2);
                    antinode2 -= distance;
                }
            }
        }

        return Task.FromResult(new PartResult($"{antinodes.Count}", $"Unique antinode loactions: {antinodes.Count}"));
    }

    private static bool InBounds(Position position, char[][] field)
    {
        return position.X >= 0 && position.X < field[0].Length && position.Y >= 0 && position.Y < field.Length;
    }

    private static char[][] CreateField(string input)
    {
        var lines = input.Split(Environment.NewLine);
        var field = new char[lines.Length][];
        for (var i = 0; i < lines.Length; i++)
        {
            field[i] = lines[i].ToCharArray();
        }

        return field;
    }

    private record struct Position(int X, int Y)
    {
        public static Distance operator -(Position position1, Position position2)
        {
            return new Distance(position1.X - position2.X, position1.Y - position2.Y);
        }

        public static Position operator +(Position position, Distance distance)
        {
            return new Position(position.X + distance.X, position.Y + distance.Y);
        }

        public static Position operator -(Position position, Distance distance)
        {
            return new Position(position.X - distance.X, position.Y - distance.Y);
        }
    }

    private record struct Distance(int X, int Y);
}