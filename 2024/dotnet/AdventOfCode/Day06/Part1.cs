using AdventOfCode.Days;
using AdventOfCode.Measure;

namespace AdventOfCode.Day06;

[Day(6)]
[Part(1)]
public class Part1 : IPart
{
    private const char Obstacle = '#';

    public Task<PartResult> RunAsync(IMeasure measure, string input)
    {
        var lines = input.Split(Environment.NewLine);
        var field = new char[lines.Length][];
        for (var i = 0; i < lines.Length; i++)
        {
            field[i] = lines[i].ToCharArray();
        }

        measure.Now("Parsed");

        var directions = new List<Direction>
        {
            new(0, -1),
            new(1, 0),
            new(0, 1),
            new(-1, 0)
        };

        var positions = new List<Position>();
        var current = GetCurrentPosition(field);

        while (InBounds(field, current.Position))
        {
            if (field[current.Position.Y][current.Position.X] == Obstacle)
            {
                current -= current.Direction;
                var nextDirectionIndex = (directions.IndexOf(current.Direction) + 1) % directions.Count;
                current = current with { Direction = directions[nextDirectionIndex] };
                continue;
            }

            positions.Add(current.Position);
            current += current.Direction;
        }

        var distinctPositions = positions.Distinct().Count();
        return Task.FromResult(
            new PartResult($"{distinctPositions}", $"Count of visited positions {distinctPositions}"));
    }

    private static bool InBounds(char[][] field, Position position)
    {
        return position.X >= 0 && position.X < field[0].Length && position.Y >= 0 && position.Y < field.Length;
    }

    private static Current GetCurrentPosition(char[][] field)
    {
        for (var row = 0; row < field.Length; row++)
        {
            for (var col = 0; col < field[row].Length; col++)
            {
                if (field[row][col] == '^')
                {
                    return new Current(new Position(col, row), new Direction(0, -1));
                }
            }
        }

        throw new InvalidOperationException("No starting position found");
    }

    private record Current(Position Position, Direction Direction)
    {
        public static Current operator +(Current current, Direction direction)
        {
            return new Current(new Position(current.Position.X + direction.X, current.Position.Y + direction.Y),
                direction);
        }

        public static Current operator -(Current current, Direction direction)
        {
            return new Current(new Position(current.Position.X - direction.X, current.Position.Y - direction.Y),
                direction);
        }
    };

    private record Position(int X, int Y);

    private record Direction(int X, int Y);
}