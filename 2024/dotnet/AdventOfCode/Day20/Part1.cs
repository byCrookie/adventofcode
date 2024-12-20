using System.Diagnostics;
using System.Text;
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

        var cheatedLengths = new List<int>();
        var index = 1;
        foreach (var cheat in wallsWithoutBorder)
        {
            Console.WriteLine($"Cheat ({index}/{wallsWithoutBorder.Count}): {cheat.x},{cheat.y}");
            
            field[cheat.y][cheat.x] = Empty;

            var cheatedPath = Dijkstra(
                [new Node(start)],
                [new Node(end)],
                n => GetNeighbors(field, n),
                (_, _) => 1
            );

            if (cheatedPath is null)
            {
                throw new UnreachableException();
            }

            cheatedLengths.Add((int)cheatedPath.Value.Cost);
            // Console.WriteLine($"Cheat: {cheat.x},{cheat.y}, Length: {cheatedPath.Value.Cost}");

            field[cheat.y][cheat.x] = Wall;
            index++;
        }

        // var grouped = cheatedLengths.GroupBy(c => c);
        // foreach (var group in grouped.OrderByDescending(g => g.Count()).ThenBy(g => path.Value.Cost - g.Key))
        // {
        //     Console.WriteLine(
        //         $"- There are {group.Count()} cheats that save {path.Value.Cost - group.Key} picoseconds.");
        // }

        var cheats = cheatedLengths.Count(l => path.Value.Cost - l >= 100);
        return Task.FromResult(new PartResult($"{cheats}", $"Cheats saving at least 100 picoseconds: {cheats}"));
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

    private static void Walk(char[][] field, Position position, Direction direction, List<int> paths, int path)
    {
        if (field[position.Y][position.X] == End)
        {
            paths.Add(path + 1);
            return;
        }

        foreach (var nextDirection in AllowedDirections(direction))
        {
            var newPosition = position + nextDirection;

            if (!InBounds(field, newPosition) || field[newPosition.Y][newPosition.X] == Wall)
            {
                continue;
            }

            Walk(field, newPosition, nextDirection, paths, path + 1);
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
                return new Path<T>(path, cost);
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

    private record struct Path<T>(List<T> Nodes, double Cost);

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
    }

    private record struct Direction(int X, int Y);
}