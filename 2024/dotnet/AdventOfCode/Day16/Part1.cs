using AdventOfCode.Days;
using AdventOfCode.Measure;

namespace AdventOfCode.Day16;

[Day(16)]
[Part(1)]
public class Part1 : IPart
{
    private const char Start = 'S';
    private const char End = 'E';
    private const char Wall = '#';
    private const char Empty = '.';

    private static readonly Direction Up = new('^', 0, -1);
    private static readonly Direction Down = new('v', 0, 1);
    private static readonly Direction Left = new('<', -1, 0);
    private static readonly Direction Right = new('>', 1, 0);

    public Task<PartResult> RunAsync(IMeasure measure, string input)
    {
        return Task.FromResult(new PartResult($"{1}", $"Sum of boxes gps coordinates: {1}"));
    }

    private record struct Position(int X, int Y)
    {
        public static Position operator +(Position p1, Position p2)
        {
            return new Position(p1.X + p2.X, p1.Y + p2.Y);
        }

        public static Position operator -(Position p1, Position p2)
        {
            return new Position(p1.X - p2.X, p1.Y - p2.Y);
        }

        public static Position operator +(Position p1, Direction dir)
        {
            return new Position(p1.X + dir.X, p1.Y + dir.Y);
        }
    }

    private record struct Direction(char Type, int X, int Y);
}