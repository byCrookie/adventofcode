using System.Text.RegularExpressions;
using AdventOfCode.Days;
using AdventOfCode.Measure;

namespace AdventOfCode.Day14;

[Day(14)]
[Part(2)]
public partial class Part2 : IPart
{
    private const int Rows = 103;

    private const int Columns = 101;

    // private const int Rows = 7;
    // private const int Columns = 11;
    private const int Seconds = 1000;

    public Task<PartResult> RunAsync(IMeasure measure, string input)
    {
        var robots = new List<Robot>();
        foreach (var line in input.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries))
        {
            var robot = ParseRobot(line);
            robots.Add(robot);
        }

        var cursorPositionSeconds = Console.GetCursorPosition();
        var cursorPositionField = cursorPositionSeconds with { Top = cursorPositionSeconds.Top + 1 };
        var field = new char[Rows, Columns];
        for (var i = 0; i < Rows; i++)
        {
            for (var j = 0; j < Columns; j++)
            {
                field[i, j] = ' ';
            }
        }

        Console.SetCursorPosition(cursorPositionSeconds.Left, cursorPositionSeconds.Top);
        Console.Write($"Seconds: {Seconds}");
        for (var i = 0; i < Rows; i++)
        {
            for (var j = 0; j < Columns; j++)
            {
                Console.SetCursorPosition(cursorPositionField.Left + j, cursorPositionField.Top + i);
                Console.Write(field[i, j]);
            }
        }

        for (var i = 0; i < Seconds; i++)
        {
            var robotsAfterSeconds = new List<Robot>();
            foreach (var robot in robots)
            {
                Console.SetCursorPosition(cursorPositionField.Left + robot.Position.X, cursorPositionField.Top + robot.Position.Y);
                Console.Write(' ');

                var newX = robot.Position.X + robot.Velocity.X * i;
                var newY = robot.Position.Y + robot.Velocity.Y * i;

                var wrapX = newX % Columns;
                var wrapY = newY % Rows;

                var validX = wrapX < 0 ? wrapX + Columns : wrapX;
                var validY = wrapY < 0 ? wrapY + Rows : wrapY;

                var position = new Position(validX, validY);
                robotsAfterSeconds.Add(robot with { Position = position });

                Console.SetCursorPosition(cursorPositionField.Left + validX, cursorPositionField.Top + validY);
                Console.Write('X');
            }
            
            Console.SetCursorPosition(cursorPositionSeconds.Left, cursorPositionSeconds.Top);
            Console.Write($"Seconds: {i}".PadRight(20));
            robots = robotsAfterSeconds;
        }

        var easterEggAfterSeconds = 0;
        return Task.FromResult(new PartResult($"{easterEggAfterSeconds}", $"Safety factor: {easterEggAfterSeconds}"));
    }

    private static void PrintField(List<Robot> robotsAfterSeconds)
    {
        var field = new int[Rows, Columns];
        for (var i = 0; i < Rows; i++)
        {
            for (var j = 0; j < Columns; j++)
            {
                field[i, j] = 0;
            }
        }

        foreach (var robot in robotsAfterSeconds)
        {
            field[robot.Position.Y, robot.Position.X]++;
        }

        for (var i = 0; i < Rows; i++)
        {
            for (var j = 0; j < Columns; j++)
            {
                if (field[i, j] == 0)
                {
                    Console.Write(".");
                    continue;
                }

                Console.Write(field[i, j]);
            }

            Console.WriteLine();
        }
    }

    private static Robot ParseRobot(string line)
    {
        var match = ButtonRegex().Match(line);

        var px = match.Groups["PXMinus"].Success
            ? int.Parse(match.Groups["PXMinus"].Value)
            : int.Parse(match.Groups["PXPlus"].Value);

        var py = match.Groups["PYMinus"].Success
            ? int.Parse(match.Groups["PYMinus"].Value)
            : int.Parse(match.Groups["PYPlus"].Value);

        var vx = match.Groups["VXMinus"].Success
            ? int.Parse(match.Groups["VXMinus"].Value)
            : int.Parse(match.Groups["VXPlus"].Value);

        var vy = match.Groups["VYMinus"].Success
            ? int.Parse(match.Groups["VYMinus"].Value)
            : int.Parse(match.Groups["VYPlus"].Value);

        return new Robot(new Position(px, py), new Velocity(vx, vy));
    }

    private record struct Robot(Position Position, Velocity Velocity);

    private record struct Velocity(int X, int Y);

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
    }

    [GeneratedRegex(
        @"p=((?<PXMinus>-\d*)|(?<PXPlus>\d*)),((?<PYMinus>-\d*)|(?<PYPlus>\d*)) v=((?<VXMinus>-\d*)|(?<VXPlus>\d*)),((?<VYMinus>-\d*)|(?<VYPlus>\d*))")]
    private static partial Regex ButtonRegex();
}