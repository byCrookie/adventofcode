using AdventOfCode.Days;
using AdventOfCode.Measure;

namespace AdventOfCode.Day06;

[Day(6)]
[Part(2)]
public class Part2 : IPart
{
    private const char Obstacle = '#';
    private object _lockObject = new();

    public async Task<PartResult> RunAsync(IMeasure measure, string input)
    {
        var field = CreateField(input);
        measure.Now("Parsed");
        var obstacles = ObstaclesToPlace(field);
        measure.Now("Obstacles to place");

        var loopCount = 0;
        var completed = 0;
        var cursor = Console.GetCursorPosition();
        var tasks = obstacles.Select(obstacle => Task.Run(() =>
        {
            var copy = CopyField(field);
            copy[obstacle.Y][obstacle.X] = Obstacle;

            if (IsLoop(copy, GetCurrentPosition(copy)))
            {
                Interlocked.Increment(ref loopCount);
            }

            lock (_lockObject)
            {
                completed++;
                if (completed % 10 != 0) return;
                Console.SetCursorPosition(cursor.Left, cursor.Top);
                Console.WriteLine($"{100.0 * completed / obstacles.Count:N}%");
            }
        }));

        Console.SetCursorPosition(cursor.Left, cursor.Top);
        Console.WriteLine("100%");
        await Task.WhenAll(tasks);
        return new PartResult($"{loopCount}", $"Count of additional obstacles that result in loop {loopCount}");
    }

    private static List<Position> ObstaclesToPlace(char[][] field)
    {
        var obstacles = new List<Position>();
        for (var row = 0; row < field.Length; row++)
        {
            for (var col = 0; col < field[row].Length; col++)
            {
                if (field[row][col] == '.' && field[row][col] != '^')
                {
                    obstacles.Add(new Position(col, row));
                }
            }
        }

        return obstacles;
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

    private static char[][] CopyField(char[][] field)
    {
        var fieldCopy = new char[field.Length][];
        for (var i = 0; i < field.Length; i++)
        {
            fieldCopy[i] = new char[field[i].Length];
            field[i].CopyTo(fieldCopy[i], 0);
        }

        return fieldCopy;
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
        Thread.Sleep(25);
        Console.Clear();

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