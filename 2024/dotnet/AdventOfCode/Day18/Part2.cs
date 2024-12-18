using AdventOfCode.Days;
using AdventOfCode.Measure;

namespace AdventOfCode.Day18;

[Day(18)]
[Part(2)]
public class Part2 : IPart
{
    private const int Rows = 70;
    private const int Cols = 70;

    // private const int Rows = 6;
    // private const int Cols = 6;

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
        var bytes = input.Split(Environment.NewLine)
            .Select(l => new Position(int.Parse(l.Split(",")[0]), int.Parse(l.Split(",")[1])))
            .ToList();

        var byteIndex = 0;
        while (byteIndex < bytes.Count)
        {
            var field = CreateField(bytes.Take(byteIndex));

            var start = new Position(0, 0);
            var end = new Position(Cols, Rows);

            var path = Dijkstra(
                [new Node(start)],
                [new Node(end)],
                n => GetNeighbors(field, n),
                (_, _) => 1
            );

            if (path is null)
            {
                break;
            }
            
            byteIndex++;
        }

        var invalidByte = bytes.Take(byteIndex).Last();
        return Task.FromResult(new PartResult($"{invalidByte}", $"Preventing byte ({byteIndex}): {invalidByte}"));
    }

    private static void PrintField(char[,] field, List<Node> path)
    {
        for (var y = 0; y <= Rows; y++)
        {
            for (var x = 0; x <= Cols; x++)
            {
                var c = field[y, x];
                if (path.Any(p => p.Position.X == x && p.Position.Y == y))
                {
                    c = 'x';
                }

                Console.Write(c);
            }

            Console.WriteLine();
        }
    }

    private static char[,] CreateField(IEnumerable<Position> bytes)
    {
        var field = new char[Rows + 1, Cols + 1];
        for (var y = 0; y <= Rows; y++)
        {
            for (var x = 0; x <= Cols; x++)
            {
                field[y, x] = Empty;
            }
        }

        foreach (var b in bytes)
        {
            field[b.Y, b.X] = Wall;
        }

        return field;
    }

    private static IEnumerable<Node> GetNeighbors(char[,] field, Node current)
    {
        var directions = new List<Direction> { Up, Down, Left, Right };
        return from direction in directions
            select new Node(current.Position + direction)
            into newPosition
            where newPosition.Position.X >= 0 && newPosition.Position.X < field.GetLength(1) &&
                  newPosition.Position.Y >= 0 &&
                  newPosition.Position.Y < field.GetLength(0)
            where field[newPosition.Position.Y, newPosition.Position.X] != Wall
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