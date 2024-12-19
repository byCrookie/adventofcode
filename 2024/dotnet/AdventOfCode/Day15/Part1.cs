using AdventOfCode.Days;
using AdventOfCode.Measure;

namespace AdventOfCode.Day15;

[Day(15)]
[Part(1)]
public class Part1 : IPart
{
    private const char Robot = '@';
    private const char Box = 'O';
    private const char Wall = '#';
    private const char Empty = '.';

    public Task<PartResult> RunAsync(IMeasure measure, string input)
    {
        var (robot, field, moves) = Parse(input);
        PrintField(field);

        foreach (var move in moves)
        {
            robot = MoveRobot(robot, move, field);
            // Console.Clear();
            // Console.WriteLine($"Move: {move.Type}");
            // PrintField(field);
        }

        var boxesInCorrectPosition = new List<Position>();
        for (var y = 0; y < field.Length; y++)
        {
            for (var x = 0; x < field[y].Length; x++)
            {
                if (field[y][x] == Box)
                {
                    boxesInCorrectPosition.Add(new Position(x, y));
                }
            }
        }

        var sum = boxesInCorrectPosition.Sum(box => box.X + 100 * box.Y);
        return Task.FromResult(new PartResult($"{sum}", $"Sum of boxes gps coordinates: {sum}"));
    }

    private static Position MoveRobot(Position robot, Direction move, char[][] field)
    {
        var newRobot = robot + move;
        if (!InBounds(field, newRobot) || field[newRobot.Y][newRobot.X] == Wall)
        {
            return robot;
        }

        if (field[newRobot.Y][newRobot.X] == Empty)
        {
            field[robot.Y][robot.X] = Empty;
            field[newRobot.Y][newRobot.X] = Robot;
            return newRobot;
        }

        var positionsInFront = new List<Position>();
        var positionInFront = robot + move;
        while (InBounds(field, positionInFront) && field[positionInFront.Y][positionInFront.X] != Wall)
        {
            positionsInFront.Add(positionInFront);
            positionInFront += move;
        }

        var boxesInFront = positionsInFront
            .TakeWhile(p => field[p.Y][p.X] == Box && field[p.Y][p.X] != Wall && field[p.Y][p.X] != Empty).ToArray();
        var emptyInFront = positionsInFront.Cast<Position?>()
            .FirstOrDefault(p => field[p!.Value.Y][p.Value.X] == Empty, null);

        if (emptyInFront is not null)
        {
            foreach (var box in boxesInFront.Reverse())
            {
                field[box.Y][box.X] = Empty;
                field[box.Y + move.Y][box.X + move.X] = Box;
            }

            field[robot.Y][robot.X] = Empty;
            field[newRobot.Y][newRobot.X] = Robot;
            return newRobot;
        }

        return robot;
    }

    private static bool InBounds(char[][] field, Position position)
    {
        return position.X >= 0 && position.X < field[0].Length && position.Y >= 0 && position.Y < field.Length;
    }

    private static void PrintField(char[][] field)
    {
        foreach (var row in field)
        {
            Console.WriteLine(row);
        }
    }

    private static (Position robot, char[][] field, List<Direction> moves) Parse(string input)
    {
        Position? robot = null;
        var field = new List<char[]>();
        var moves = new List<Direction>();
        var parseField = true;
        foreach (var line in input.Split(Environment.NewLine))
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                parseField = false;
                continue;
            }

            if (parseField)
            {
                field.Add(line.ToCharArray());
                continue;
            }

            moves.AddRange(line.ToCharArray().Select(moveChar => moveChar switch
            {
                '^' => new Direction('^', 0, -1),
                'v' => new Direction('v', 0, 1),
                '<' => new Direction('<', -1, 0),
                '>' => new Direction('>', 1, 0),
                _ => throw new InvalidOperationException($"Unknown move character: {moveChar}")
            }));
        }

        for (var y = 0; y < field.Count; y++)
        {
            for (var x = 0; x < field[y].Length; x++)
            {
                var position = new Position(x, y);
                switch (field[y][x])
                {
                    case Empty:
                    case Wall:
                    case Box:
                        break;
                    case Robot:
                        robot = position;
                        break;
                    default:
                        throw new InvalidOperationException($"Unknown field character: {field[y][x]}");
                }
            }
        }

        return ((Position)robot!, field.ToArray(), moves);
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