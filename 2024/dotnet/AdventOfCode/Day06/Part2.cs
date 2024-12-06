using AdventOfCode.Days;
using AdventOfCode.Measure;

namespace AdventOfCode.Day06;

[Day(6)]
[Part(2)]
public class Part2 : IPart
{
    private const char Obstacle = '#';

    private static readonly List<Direction> Directions =
    [
        new(0, -1),
        new(1, 0),
        new(0, 1),
        new(-1, 0)
    ];

    public async Task<PartResult> RunAsync(IMeasure measure, string input)
    {
        var field = CreateField(input);
        measure.Now("Field");
        var start = GetCurrentPosition(field);
        measure.Now("Start Position");
        var obstacles = ObstaclesToPlace(field, start);
        measure.Now("Obstacles");

        var loopCount = 0;
        var completed = 0;
        var cursor = Console.GetCursorPosition();
        await Parallel.ForEachAsync(obstacles, new ParallelOptions
        {
            MaxDegreeOfParallelism = Environment.ProcessorCount
        }, (obstacle, _) =>
        {
            if (IsLoop(field, start, obstacle))
            {
                Interlocked.Increment(ref loopCount);
            }

            completed++;
            if (completed % 300 != 0) return ValueTask.CompletedTask;
            Console.SetCursorPosition(cursor.Left, cursor.Top);
            Console.WriteLine($"{100.0 * completed / obstacles.Count:N}%");
            return ValueTask.CompletedTask;
        });

        measure.Now("Finish");

        Console.SetCursorPosition(cursor.Left, cursor.Top);
        Console.WriteLine("100%".PadRight(20));
        return new PartResult($"{loopCount}", $"Count of additional obstacles that result in loop {loopCount}");
    }

    private static List<Position> ObstaclesToPlace(char[][] field, Move start)
    {
        var positions = new HashSet<Position>();
        var current = start;
        var directionIndex = Directions.IndexOf(current.Direction);

        while (InBounds(field, current.Position))
        {
            if (field[current.Position.Y][current.Position.X] == Obstacle)
            {
                current -= current.Direction;
                directionIndex = (directionIndex + 1) % Directions.Count;
                current += Directions[directionIndex];
                continue;
            }

            positions.Add(current.Position);
            current += current.Direction;
        }

        return positions.ToList();
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

    private static bool IsLoop(char[][] field, Move start, Position obstacle)
    {
        var moves = new HashSet<Move>(7000);
        var current = start;
        var directionIndex = Directions.IndexOf(current.Direction);

        while (InBounds(field, current.Position))
        {
            // PrintField(field, current);

            if (moves.Contains(current))
            {
                return true;
            }

            if (field[current.Position.Y][current.Position.X] == Obstacle || current.Position == obstacle)
            {
                current -= current.Direction;
                // PrintField(field, current);
                directionIndex = (directionIndex + 1) % Directions.Count;
                current += Directions[directionIndex];
                continue;
            }

            moves.Add(current);
            current += current.Direction;
        }

        return false;
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

    private record struct Move(Position Position, Direction Direction)
    {
        public static Move operator +(Move move, Direction direction)
        {
            return new Move(
                new Position(move.Position.X + direction.X, move.Position.Y + direction.Y),
                direction);
        }

        public static Move operator -(Move move, Direction direction)
        {
            return new Move(
                new Position(move.Position.X - direction.X, move.Position.Y - direction.Y),
                direction);
        }
    };

    private record struct Position(int X, int Y);

    private record struct Direction(int X, int Y);
}