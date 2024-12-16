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

    private static readonly Direction Up = new(0, -1);
    private static readonly Direction Down = new(0, 1);
    private static readonly Direction Left = new(-1, 0);
    private static readonly Direction Right = new(1, 0);

    public Task<PartResult> RunAsync(IMeasure measure, string input)
    {
        var field = input
            .Split(Environment.NewLine)
            .Select(line => line.ToCharArray())
            .ToArray();

        var start = FindPosition(field, Start);
        var end = FindPosition(field, End);

        var path = Dijkstra(
            start,
            end,
            (current, previous) => GetNeighbors(field, current, previous),
            Distance
        );

        if (path is null)
        {
            throw new InvalidOperationException("Could not find path");
        }

        for (var i = 1; i < path.Value.Nodes.Count - 1; i++)
        {
            var position = path.Value.Nodes[i];
            field[position.Y][position.X] = 'x';
        }

        var result = string.Join(Environment.NewLine, field.Select(line => new string(line)));
        Console.WriteLine(result);

        var cost = path.Value.Cost;
        return Task.FromResult(new PartResult($"{cost}", $"Cost of path: {cost}"));
    }

    private static double Distance(Position current, Position neighbour, Position previous)
    {
        var direction = new Direction(
            Math.Sign(current.X - previous.X),
            Math.Sign(current.Y - previous.Y)
        );

        var newDirection = new Direction(
            Math.Sign(neighbour.X - current.X),
            Math.Sign(neighbour.Y - current.Y)
        );

        return direction == newDirection ? 1 : 1001;
    }

    private static Position FindPosition(char[][] field, char type)
    {
        for (var y = 0; y < field.Length; y++)
        {
            for (var x = 0; x < field[y].Length; x++)
            {
                if (field[y][x] == type)
                {
                    return new Position(x, y);
                }
            }
        }

        throw new InvalidOperationException($"Could not find {type} in field");
    }

    private static IEnumerable<Position> GetNeighbors(char[][] field, Position current, Position previous)
    {
        var directions = new List<Direction> { Up, Down, Left, Right };
        var previousDirection = new Direction(
            Math.Sign(current.X - previous.X),
            Math.Sign(current.Y - previous.Y)
        );

        if (previousDirection == Up)
        {
            directions.Remove(Down);
        }
        else if (previousDirection == Down)
        {
            directions.Remove(Up);
        }
        else if (previousDirection == Left)
        {
            directions.Remove(Right);
        }
        else if (previousDirection == Right)
        {
            directions.Remove(Left);
        }

        return from direction in directions
            select current + direction
            into newPosition
            where newPosition.X >= 0 && newPosition.X < field[0].Length && newPosition.Y >= 0 &&
                  newPosition.Y < field.Length
            where field[newPosition.Y][newPosition.X] != Wall
            select newPosition;
    }

    private static Path<T>? Dijkstra<T>(
        T start,
        T goal,
        Func<T, T, IEnumerable<T>> getNeighbors,
        Func<T, T, T, double> distance) where T : notnull
    {
        var queue = new PriorityQueue<T, double>();
        var costs = new Dictionary<T, double>();
        var previous = new Dictionary<T, T>();

        queue.Enqueue(start, 0);
        costs[start] = 0;

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();

            if (current.Equals(goal))
            {
                var path = new List<T> { current };
                var cost = costs[current];

                while (previous.ContainsKey(current))
                {
                    current = previous[current];
                    path.Add(current);
                }

                path.Reverse();
                return new Path<T>(path, cost);
            }

            foreach (var neighbor in getNeighbors(current, previous.GetValueOrDefault(current, current)))
            {
                var newCost = costs[current] + distance(current, neighbor, previous.GetValueOrDefault(current, current));

                if (!costs.ContainsKey(neighbor) || newCost < costs[neighbor])
                {
                    costs[neighbor] = newCost;
                    previous[neighbor] = current;
                    queue.Enqueue(neighbor, newCost);
                }
            }
        }

        return null;
    }

    private record struct Path<T>(List<T> Nodes, double Cost);
    
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