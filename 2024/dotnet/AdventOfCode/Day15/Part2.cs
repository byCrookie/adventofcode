using AdventOfCode.Days;
using AdventOfCode.Measure;

namespace AdventOfCode.Day15;

[Day(15)]
[Part(2)]
public partial class Part2 : IPart
{
    private const char Robot = '@';
    private const char Box = 'O';
    private const char BoxStart = '[';
    private const char BoxEnd = ']';
    private const char Wall = '#';
    private const char Empty = '.';
    
    private static Direction Up => new('^', 0, -1);
    private static Direction Down => new('v', 0, 1);
    private static Direction Left => new('<', -1, 0);
    private static Direction Right => new('>', 1, 0);
    
    private const int PrintIndex = 19999;

    public Task<PartResult> RunAsync(IMeasure measure, string input)
    {
        var (robot, field, moves) = Parse(input);
        Console.WriteLine($"Moves ({moves.Count})");
        PrintField(field);
        
        foreach (var (move,index) in moves.Select((m,i) => (m, i)))
        {
            if (index >= PrintIndex)
            {
                Console.Clear();
                Console.WriteLine($"Move ({index}/{moves.Count}): {move.Type}");
                PrintField(field);
            }
            robot = MoveRobot(robot, move, field, index);
        }

        var boxesInCorrectPosition = new List<Position>();
        for (var y = 0; y < field.Length; y++)
        {
            for (var x = 0; x < field[y].Length; x++)
            {
                if (field[y][x] == BoxStart)
                {
                    boxesInCorrectPosition.Add(new Position(x, y));
                }
            }
        }

        var sum = boxesInCorrectPosition.Sum(box => box.X + 100 * box.Y);
        return Task.FromResult(new PartResult($"{sum}", $"Sum of boxes gps coordinates: {sum}"));
    }

    private static Position MoveRobot(Position robot, Direction move, char[][] field, int index)
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
            .TakeWhile(p => (field[p.Y][p.X] == BoxStart || field[p.Y][p.X] == BoxEnd) && field[p.Y][p.X] != Wall && field[p.Y][p.X] != Empty).ToArray();
        var emptyInFront = positionsInFront.Cast<Position?>()
            .FirstOrDefault(p => field[p!.Value.Y][p.Value.X] == Empty, null);

        if (emptyInFront is not null && IsHorizontal(move))
        {
            foreach (var box in boxesInFront.Reverse())
            {
                if (field[box.Y][box.X] == BoxStart)
                {
                    field[box.Y][box.X] = Empty;
                    field[box.Y + move.Y][box.X + move.X] = BoxStart;
                }
                else
                {
                    field[box.Y][box.X] = Empty;
                    field[box.Y + move.Y][box.X + move.X] = BoxEnd;
                }
            }

            field[robot.Y][robot.X] = Empty;
            field[newRobot.Y][newRobot.X] = Robot;
            return newRobot;
        }

        if (IsVertical(move))
        {
            var positionsOfBoxes = FindBoxesInFront(field, robot, move).Distinct().ToArray();
            var positionsOfBoxesWithNothingInFront = positionsOfBoxes
                .Where(p => field[p.Y + move.Y][p.X + move.X] == Empty 
                            || field[p.Y + move.Y][p.X + move.X] == BoxStart
                            || field[p.Y + move.Y][p.X + move.X] == BoxEnd).ToArray();
            if (positionsOfBoxesWithNothingInFront.Length != positionsOfBoxes.Length)
            {
                return robot;
            }
            
            var orderedBoxes = positionsOfBoxesWithNothingInFront
                .OrderBy(p => p.Y).ThenBy(p => p.X).ToArray();

            if (move == Down)
            {
                orderedBoxes = orderedBoxes.Reverse().ToArray();
            }
            
            foreach (var box in orderedBoxes)
            {
                if (field[box.Y][box.X] == BoxStart)
                {
                    field[box.Y][box.X] = Empty;
                    field[box.Y + move.Y][box.X + move.X] = BoxStart;
                }
                else if (field[box.Y][box.X] == BoxEnd)
                {
                    field[box.Y][box.X] = Empty;
                    field[box.Y + move.Y][box.X + move.X] = BoxEnd;
                } else {
                    throw new InvalidOperationException($"Unknown box type: {field[box.Y][box.X]}");
                }

                // if (index >= PrintIndex)
                // {
                //     Console.Clear();
                //     Console.WriteLine($"Move ({index}/{positionsOfBoxes.Length}): {move.Type}");
                //     PrintField(field);
                // }
            }
            
            field[robot.Y][robot.X] = Empty;
            field[newRobot.Y][newRobot.X] = Robot;
            return newRobot;
        }

        return robot;
    }

    private static List<Position> FindBoxesInFront(char[][] field, Position robot, Direction move)
    {
        var boxesInFront = new List<Position>();
        var position = robot + move;
        if (field[position.Y][position.X] == BoxStart)
        {
            boxesInFront.Add(position);
            boxesInFront.Add(position + Right);
            var boxesInFrontOfBoxStart = FindBoxesInFront(field, position, move);
            var boxesInFrontOfBoxEnd = FindBoxesInFront(field, position + Right, move);
            boxesInFront.AddRange(boxesInFrontOfBoxStart);
            boxesInFront.AddRange(boxesInFrontOfBoxEnd);
        }
        
        if (field[position.Y][position.X] == BoxEnd)
        {
            boxesInFront.Add(position);
            boxesInFront.Add(position + Left);
            var boxesInFrontOfBoxStart = FindBoxesInFront(field, position, move);
            var boxesInFrontOfBoxEnd = FindBoxesInFront(field, position + Left, move);
            boxesInFront.AddRange(boxesInFrontOfBoxStart);
            boxesInFront.AddRange(boxesInFrontOfBoxEnd);
        }
        
        return boxesInFront;
    }

    private static bool IsVertical(Direction direction)
    {
        return direction.Y != 0;
    }
    
    private static bool IsHorizontal(Direction direction)
    {
        return direction.X != 0;
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
        var field = new List<List<char>>();
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
                var lineStretched = new List<char>();
                foreach (var c in line.ToCharArray())
                {
                    switch (c)
                    {
                        case Robot:
                            lineStretched.Add(Robot);
                            lineStretched.Add(Empty);
                            continue;
                        case Empty:
                            lineStretched.Add(Empty);
                            lineStretched.Add(Empty);
                            continue;
                        case Box:
                            lineStretched.Add(BoxStart);
                            lineStretched.Add(BoxEnd);
                            continue;
                        case Wall:
                            lineStretched.Add(Wall);
                            lineStretched.Add(Wall);
                            continue;
                        default:
                            throw new InvalidOperationException($"Unknown character: {c}");
                    }
                }

                field.Add(lineStretched);
                continue;
            }

            moves.AddRange(line.ToCharArray().Select(moveChar => moveChar switch
            {
                '^' => Up,
                'v' => Down,
                '<' => Left,
                '>' => Right,
                _ => throw new InvalidOperationException($"Unknown move character: {moveChar}")
            }));
        }

        for (var y = 0; y < field.Count; y++)
        {
            for (var x = 0; x < field[y].Count; x++)
            {
                var position = new Position(x, y);
                switch (field[y][x])
                {
                    case Empty:
                    case Wall:
                    case BoxStart:
                    case BoxEnd:
                        break;
                    case Robot:
                        robot = position;
                        break;
                    default:
                        throw new InvalidOperationException($"Unknown field character: {field[y][x]}");
                }
            }
        }

        return ((Position)robot!, field.Select(l => l.ToArray()).ToArray(), moves);
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