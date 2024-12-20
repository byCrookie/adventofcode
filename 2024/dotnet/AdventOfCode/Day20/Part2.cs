using System.Diagnostics;
using System.Text;
using AdventOfCode.Days;
using AdventOfCode.Measure;

namespace AdventOfCode.Day20;

[Day(20)]
[Part(2)]
public class Part2 : IPart
{
    private const char Start = 'S';
    private const char End = 'E';
    private const char Wall = '#';

    private static readonly Direction Up = new(0, -1);
    private static readonly Direction Down = new(0, 1);
    private static readonly Direction Left = new(-1, 0);
    private static readonly Direction Right = new(1, 0);

    private const int MinSave = 100;

    public Task<PartResult> RunAsync(IMeasure measure, string input)
    {
        var field = input
            .Split(Environment.NewLine)
            .Select(line => line.ToCharArray())
            .ToArray();

        Print(field, []);
        var start = FindPosition(field, Start);
        var end = FindPosition(field, End);

        var path = Dijkstra(
            [new Node(start)],
            [new Node(end)],
            n => GetNeighbors(field, n),
            (_, _) => 1
        );

        if (path is null)
        {
            throw new UnreachableException();
        }

        Console.WriteLine($"Length: {path.Value.Cost}");

        var cheats = new Dictionary<int, int>();
        var index = 1;
        foreach (var node in path.Value.Nodes)
        {
            Console.WriteLine($"Cheat ({index}/{path.Value.Nodes.Count}): {node.Position.X},{node.Position.Y}");
            var costStartToNode = path.Value.Costs[node];

            foreach (var otherNode in path.Value.Nodes)
            {
                var distance = Position.Manhattan(node.Position, otherNode.Position);

                if (distance > 20)
                {
                    continue;
                }

                var costStartToOther = path.Value.Costs[otherNode];
                var costOtherToEnd = path.Value.Costs[new Node(end)] - costStartToOther;
                var cost = costStartToNode + distance + costOtherToEnd;
                var save = (int)(path.Value.Cost - cost);

                if (save < MinSave)
                {
                    continue;
                }

                if (!cheats.TryAdd(save, 1))
                {
                    cheats[save]++;
                }
            }

            index++;
        }

        foreach (var group in cheats.GroupBy(c => c.Key).OrderBy(g => g.Key).ThenByDescending(g => g.Single().Value))
        {
            Console.WriteLine(
                $"- There are {group.Single().Value} cheats that save {group.Key} picoseconds.");
        }

        var cheatsCount = cheats.Sum(c => c.Value);
        return Task.FromResult(new PartResult($"{cheatsCount}",
            $"Cheats saving at least {MinSave} picoseconds: {cheatsCount}"));
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

    private static IEnumerable<Node> GetNeighbors(char[][] field, Node current)
    {
        var directions = new List<Direction> { Up, Down, Left, Right };
        return from direction in directions
            select new Node(current.Position + direction)
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

    private record struct Node(Position Position);

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

        public static int Manhattan(Position p1, Position p2)
        {
            return Math.Abs(p1.X - p2.X) + Math.Abs(p1.Y - p2.Y);
        }
    }

    private record struct Direction(int X, int Y);
}