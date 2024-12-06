using AdventOfCode.Days;
using AdventOfCode.Measure;

namespace AdventOfCode.Day06;

[Day(6)]
[Part(2)]
public class Part2 : IPart
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

        var loopCount = 0;
        for (var row = 0; row < field.Length; row++)
        {
            for (var col = 0; col < field[row].Length; col++)
            {
                Console.WriteLine(
                    $"Run loop detection for obstacle at {{X: {col.ToString(),-5}, Y: {row.ToString(),-5}}}");

                if (field[row][col] == '.' && field[row][col] != '^')
                {
                    field[row][col] = Obstacle;

                    if (IsLoop(field, GetCurrentPosition(field)))
                    {
                        loopCount++;
                    }

                    field[row][col] = '.';
                }
            }
        }

        return Task.FromResult(
            new PartResult($"{loopCount}", $"Count of additional obstacles that result in loop {loopCount}"));
    }

    private static bool IsLoop(char[][] field, Move start)
    {
        var directions = new List<Direction>
        {
            new(0, -1),
            new(1, 0),
            new(0, 1),
            new(-1, 0)
        };

        var moves = new HashSet<Move>();
        var current = start;
        var directionIndex = directions.IndexOf(current.Direction);

        while (InBounds(field, current.Position))
        {
            // PrintField(field, current);

            if (IsSamePath(moves, current))
            {
                return true;
            }

            if (field[current.Position.Y][current.Position.X] == Obstacle)
            {
                current -= current.Direction;
                // PrintField(field, current);
                directionIndex = (directionIndex + 1) % directions.Count;
                current += directions[directionIndex];
                continue;
            }

            moves.Add(current);
            current += current.Direction;
        }

        return false;
    }

    private static bool IsSamePath(HashSet<Move> moves, Move move)
    {
        return moves.Contains(move);
    }

    private static void PrintField(char[][] field, Move move)
    {
        for (var row = 0; row < field.Length; row++)
        {
            for (var col = 0; col < field[row].Length; col++)
            {
                if (move.Position.X == col && move.Position.Y == row)
                {
                    switch (move.Direction)
                    {
                        case { X: 0, Y: -1 }:
                            Console.Write('^');
                            break;
                        case { X: 1, Y: 0 }:
                            Console.Write('>');
                            break;
                        case { X: 0, Y: 1 }:
                            Console.Write('v');
                            break;
                        case { X: -1, Y: 0 }:
                            Console.Write('<');
                            break;
                        default:
                            throw new InvalidOperationException("Invalid direction");
                    }
                }
                else
                {
                    Console.Write(field[row][col]);
                }
            }

            Console.WriteLine();
        }
    }

    private static bool InBounds(char[][] field, Position position)
    {
        return position.X >= 0 && position.X < field[0].Length && position.Y >= 0 && position.Y < field.Length;
    }

    private static Move GetCurrentPosition(char[][] field)
    {
        for (var row = 0; row < field.Length; row++)
        {
            for (var col = 0; col < field[row].Length; col++)
            {
                if (field[row][col] == '^')
                {
                    return new Move(new Position(col, row), new Direction(0, -1));
                }
            }
        }

        throw new InvalidOperationException("No starting position found");
    }

    private record Move(Position Position, Direction Direction)
    {
        public static Move operator +(Move move, Direction direction)
        {
            return new Move(new Position(move.Position.X + direction.X, move.Position.Y + direction.Y),
                direction);
        }

        public static Move operator -(Move move, Direction direction)
        {
            return new Move(new Position(move.Position.X - direction.X, move.Position.Y - direction.Y),
                direction);
        }
    };

    private record Position(int X, int Y);

    private record Direction(int X, int Y);
}