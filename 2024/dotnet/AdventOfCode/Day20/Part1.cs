﻿using System.Text;
using AdventOfCode.Days;
using AdventOfCode.Measure;

namespace AdventOfCode.Day20;

[Day(20)]
[Part(1)]
public class Part1 : IPart
{
    private const char Start = 'S';
    private const char End = 'E';
    private const char Wall = '#';
    private const char Empty = '.';

    private static readonly Direction Up = new(0, -1);
    private static readonly Direction Down = new(0, 1);
    private static readonly Direction Left = new(-1, 0);
    private static readonly Direction Right = new(1, 0);

    private static readonly Direction[] Directions = [Up, Down, Left, Right];

    public Task<PartResult> RunAsync(IMeasure measure, string input)
    {
        var field = input
            .Split(Environment.NewLine)
            .Select(line => line.ToCharArray())
            .ToArray();

        var wallsWithoutBorder = field
            .SelectMany((line, y) => line.Select((c, x) => (c, x, y)))
            .Where(t => t.c == Wall)
            .Where(t => t.x != 0 && t.x != field[0].Length - 1 && t.y != 0 && t.y != field.Length - 1)
            .Select(t => (t.x, t.y))
            .ToList();

        Print(field, []);
        var start = FindPosition(field, Start);
        var baseDirection = DirectionToEmptyNeighbour(field, start);
        var basePaths = new List<List<Position>>();
        Walk(field, start, baseDirection, basePaths, [start]);
        var basePath = basePaths.Single();

        foreach (var cheat in wallsWithoutBorder)
        {
            field[cheat.y][cheat.x] = Empty;
            
            var dir = DirectionToEmptyNeighbour(field, start);
            var paths = new List<List<Position>>();
            Walk(field, start, dir, paths, [start]);
            foreach (var path in paths)
            {
                Console.WriteLine($"Cheat: {cheat.x},{cheat.y}, Length: {path.Count}");
            }
            
            field[cheat.y][cheat.x] = Wall;
        }

        return Task.FromResult(new PartResult($"{1}", $"Possible Design Combinations: {1}"));
    }

    private static Direction DirectionToEmptyNeighbour(char[][] field, Position start)
    {
        foreach (var direction in Directions)
        {
            var neighbour = start + direction;
            if (InBounds(field, neighbour) && field[neighbour.Y][neighbour.X] == Empty)
            {
                return direction;
            }
        }

        throw new InvalidOperationException("Could not find empty neighbour");
    }

    private static void Walk(char[][] field, Position position, Direction direction, List<List<Position>> paths,
        List<Position> path)
    {
        if (field[position.Y][position.X] == End)
        {
            path.Add(position);
            paths.Add(path);
            return;
        }

        foreach (var nextDirection in AllowedDirections(direction))
        {
            var newPosition = position + nextDirection;

            if (!InBounds(field, newPosition) || field[newPosition.Y][newPosition.X] == Wall)
            {
                continue;
            }

            path.Add(position);
            Walk(field, newPosition, nextDirection, paths, path.ToList());
        }
    }

    private static Direction[] AllowedDirections(Direction direction)
    {
        if (direction == Up)
        {
            return [Up, Left, Right];
        }

        if (direction == Down)
        {
            return [Down, Left, Right];
        }

        if (direction == Left)
        {
            return [Up, Down, Left];
        }

        if (direction == Right)
        {
            return [Up, Down, Right];
        }

        throw new InvalidOperationException($"Unknown direction {direction}");
    }

    private static bool InBounds(char[][] field, Position pos)
    {
        return pos.X >= 0 && pos.X < field[0].Length && pos.Y >= 0 && pos.Y < field.Length;
    }

    private static void Print(char[][] field, List<Position> path)
    {
        var fieldBuilder = new StringBuilder();

        for (var y = 0; y < field.Length; y++)
        {
            for (var x = 0; x < field[y].Length; x++)
            {
                var c = field[y][x];
                fieldBuilder.Append(path.Any(p => p.X == x && p.Y == y) ? 'x' : c);
            }

            fieldBuilder.AppendLine();
        }

        Console.WriteLine(fieldBuilder.ToString());
    }

    private static Position FindPosition(char[][] field, char c)
    {
        for (var y = 0; y < field.Length; y++)
        {
            for (var x = 0; x < field[y].Length; x++)
            {
                if (field[y][x] == c)
                {
                    return new Position(x, y);
                }
            }
        }

        throw new InvalidOperationException($"Could not find {c}");
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

    private record struct Direction(int X, int Y);
}