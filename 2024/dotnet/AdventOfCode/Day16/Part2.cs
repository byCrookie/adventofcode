using AdventOfCode.Days;
using AdventOfCode.Measure;

namespace AdventOfCode.Day16;

[Day(16)]
[Part(2)]
public class Part2 : IPart
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
            .Select(line => line.ToCharArray().Select(c => c == Empty ? ' ' : c).ToArray())
            .ToArray();

        var start = FindPosition(field, Start);
        var end = FindPosition(field, End);

        var path = Dijkstra(
            [new Node(start, Right)],
            [new Node(end, Right), new Node(end, Up), new Node(end, Down), new Node(end, Left)],
            current => GetNeighbors(field, current),
            Distance
        );

        if (path is null)
        {
            throw new InvalidOperationException("Could not find path");
        }

        for (var i = 1; i < path.Value.Nodes.Count - 1; i++)
        {
            var position = path.Value.Nodes[i];
            field[position.Position.Y][position.Position.X] = 'x';
        }

        var result = string.Join(Environment.NewLine, field.Select(line => new string(line)));
        Console.WriteLine(result);

        var counter = 0;
        var cheapest = path.Value.Costs
            .GroupBy(node => node.Key.Position)
            .Select(group => group.OrderBy(g => g.Value).First())
            .ToList();

        foreach (var entry in cheapest)
        {
            var subpath = Dijkstra(
                [new Node(entry.Key.Position, entry.Key.Direction)],
                [new Node(end, Right), new Node(end, Up), new Node(end, Down), new Node(end, Left)],
                current => GetNeighbors(field, current),
                Distance
            );

            if ((subpath?.Cost + entry.Value).Equals(path.Value.Cost))
            {
                counter++;
            }
        }

        counter++;
        return Task.FromResult(new PartResult($"{counter}", $"Locations on shortest paths: {counter}"));
    }

    private static double Distance(Node current, Node neighbour)
    {
        var cost = 1;

        if (current.Direction != neighbour.Direction)
        {
            cost += 1000;
        }

        return cost;
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

    private static IEnumerable<Node> GetNeighbors(char[][] field, Node current)
    {
        var directions = new List<Direction> { Up, Down, Left, Right };
        return from direction in directions
            select new Node(current.Position + direction, direction)
            into newPosition
            where newPosition.Position.X >= 0 && newPosition.Position.X < field[0].Length &&
                  newPosition.Position.Y >= 0 &&
                  newPosition.Position.Y < field.Length
            where field[newPosition.Position.Y][newPosition.Position.X] != Wall
            select newPosition;
    }

    private static Path<T>? Dijkstra<T>(
        List<T> starts,
        List<T> goals,
        Func<T, IEnumerable<T>> getNeighbors,
        Func<T, T, double> distance) where T : notnull
    {
        var queue = new PriorityQueue<T, double>();
        var costs = new Dictionary<T, double>();
        var previous = new Dictionary<T, T>();

        foreach (var start in starts)
        {
            queue.Enqueue(start, 0);
            costs[start] = 0;
        }

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();

            if (goals.Contains(current))
            {
                var path = new List<T> { current };
                var cost = costs[current];

                while (previous.ContainsKey(current))
                {
                    current = previous[current];
                    path.Add(current);
                }

                path.Reverse();
                return new Path<T>(path, cost, costs);
            }

            foreach (var neighbor in getNeighbors(current))
            {
                var newCost = costs[current] + distance(current, neighbor);

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

    private record struct Path<T>(List<T> Nodes, double Cost, Dictionary<T, double> Costs) where T : notnull;

    private record struct Node(Position Position, Direction Direction);

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