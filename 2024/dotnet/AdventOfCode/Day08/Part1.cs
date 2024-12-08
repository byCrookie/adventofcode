using AdventOfCode.Days;
using AdventOfCode.Measure;

namespace AdventOfCode.Day08;

[Day(8)]
[Part(1)]
public class Part1 : IPart
{
    public Task<PartResult> RunAsync(IMeasure measure, string input)
    {
        var field = CreateField(input);
        var antennas = new Dictionary<Position, Antenna>();
        for (var y = 0; y < field.Length; y++)
        {
            for (var x = 0; x < field[y].Length; x++)
            {
                if (field[y][x] == '.') continue;
                var position = new Position(x, y);
                antennas.Add(position, new Antenna(position, field[y][x]));
            }
        }

        var antinodes = new HashSet<Position>();
        foreach (var frequency in antennas.Values.GroupBy(antenna => antenna.Frequency))
        {
            foreach (var antenna in frequency.ToList())
            {
                foreach (var otherAntenna in frequency.ToList())
                {
                    var distance = antenna.Position - otherAntenna.Position;
                    if (distance is { X: 0, Y: 0 })
                    {
                        continue;
                    }

                    var antinode1 = antenna.Position + distance * distance.Direction;
                    var antinode2 = otherAntenna.Position - distance * distance.Direction;
                    antinodes.Add(antinode1);
                    antinodes.Add(antinode2);
                }
            }
        }

        var antinodesInBounds = antinodes
            .Where(antinode => InBounds(antinode, field))
            .ToList();

        var antinodesInBoundsAndNotOnAntennas = antinodesInBounds
            .Where(antinode => !antennas.ContainsKey(antinode))
            .ToList();

        foreach (var antinodesInBound in antinodesInBoundsAndNotOnAntennas)
        {
            field[antinodesInBound.Y][antinodesInBound.X] = '#';
        }

        PrintField(field);

        var uniqueAntinodes = antinodesInBounds.Count;
        return Task.FromResult(new PartResult($"{uniqueAntinodes}", $"Unique antinode loactions: {uniqueAntinodes}"));
    }

    private static bool InBounds(Position position, char[][] field)
    {
        return position.X >= 0 && position.X < field[0].Length && position.Y >= 0 && position.Y < field.Length;
    }

    private static char[][] CreateField(string input)
    {
        var lines = input.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        var field = new char[lines.Length][];
        for (var i = 0; i < lines.Length; i++)
        {
            field[i] = lines[i].ToCharArray();
        }

        return field;
    }

    private record struct Antenna(Position Position, char Frequency);

    private record struct Position(int X, int Y)
    {
        public static Distance operator -(Position position1, Position position2)
        {
            return new Distance(Math.Abs(position1.X - position2.X), Math.Abs(position1.Y - position2.Y),
                new Direction(Math.Sign(position1.X - position2.X), Math.Sign(position1.Y - position2.Y)));
        }

        public static Position operator *(Position position, int scalar)
        {
            return new Position(position.X * scalar, position.Y * scalar);
        }

        public static Position operator +(Position position, Direction direction)
        {
            return new Position(position.X + direction.X, position.Y + direction.Y);
        }

        public static Position operator -(Position position, Direction direction)
        {
            return new Position(position.X - direction.X, position.Y - direction.Y);
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

    private record struct Direction(int X, int Y);

    private record struct Distance(int X, int Y, Direction Direction)
    {
        public static Distance operator *(Distance distance, Direction direction)
        {
            return new Distance(distance.X * direction.X, distance.Y * direction.Y, direction);
        }
    }

    private static void PrintField(char[][] field)
    {
        foreach (var row in field)
        {
            foreach (var col in row)
            {
                Console.Write(col);
            }

            Console.WriteLine();
        }
    }
}