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
    private const int Seconds = 10000;

    public Task<PartResult> RunAsync(IMeasure measure, string input)
    {
        var robots = new List<Robot>();
        foreach (var line in input.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries))
        {
            var robot = ParseRobot(line);
            robots.Add(robot);
        }

        var overallMaxDensity = 0;
        var secondWithMaxDensity = 0;
        for (var second = 0; second < Seconds; second++)
        {
            robots = RobotsAfterSeconds(robots, second);
            var field = FloodfillField(robots);
            var density = 0;
            for (var row = 0; row < Rows; row++)
            {
                for (var column = 0; column < Columns; column++)
                {
                    density += Floodfill(field, column, row);
                }
            }

            if (density > overallMaxDensity)
            {
                overallMaxDensity = density;
                secondWithMaxDensity = second;
            }
        }

        PrintField(RobotsAfterSeconds(robots, 8281));
        return Task.FromResult(new PartResult($"{secondWithMaxDensity}", $"Easter egg after: {secondWithMaxDensity}s"));
    }

    private static int[,] FloodfillField(List<Robot> robots)
    {
        var field = new int[Rows, Columns];
        for (var i = 0; i < Rows; i++)
        {
            for (var j = 0; j < Columns; j++)
            {
                field[i, j] = 0;
            }
        }

        foreach (var robot in robots)
        {
            field[robot.Position.Y, robot.Position.X] = 1;
        }

        return field;
    }

    private static int Floodfill(int[,] field, int x, int y)
    {
        if (x < 0 || x >= Columns || y < 0 || y >= Rows || field[y, x] == 0)
        {
            return 0;
        }

        field[y, x] = 0;
        return 1 + Floodfill(field, x + 1, y) + Floodfill(field, x - 1, y) + Floodfill(field, x, y + 1) +
               Floodfill(field, x, y - 1);
    }

    private static List<Robot> RobotsAfterSeconds(List<Robot> previous, int seconds)
    {
        var next = new List<Robot>();
        foreach (var robot in previous)
        {
            var newX = robot.Position.X + robot.Velocity.X * seconds;
            var newY = robot.Position.Y + robot.Velocity.Y * seconds;

            var wrapX = newX % Columns;
            var wrapY = newY % Rows;

            var validX = wrapX < 0 ? wrapX + Columns : wrapX;
            var validY = wrapY < 0 ? wrapY + Rows : wrapY;

            var position = new Position(validX, validY);
            next.Add(robot with { Position = position });
        }

        return next;
    }

    private static void PrintField(List<Robot> robotsAfterSeconds)
    {
        var field = new char[Rows, Columns];
        for (var i = 0; i < Rows; i++)
        {
            for (var j = 0; j < Columns; j++)
            {
                field[i, j] = ' ';
            }
        }

        foreach (var robot in robotsAfterSeconds)
        {
            field[robot.Position.Y, robot.Position.X] = '#';
        }

        for (var i = 0; i < Rows; i++)
        {
            for (var j = 0; j < Columns; j++)
            {
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