using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using AdventOfCode.Days;
using AdventOfCode.Measure;

namespace AdventOfCode.Day21;

[Day(21)]
[Part(1)]
public partial class Part1 : IPart
{
    private const char Empty = ' ';

    private static readonly Direction Up = new('^', 0, -1);
    private static readonly Direction Down = new('v', 0, 1);
    private static readonly Direction Left = new('<', -1, 0);
    private static readonly Direction Right = new('>', 1, 0);

    private static readonly List<Direction> Directions = [Left, Up, Down, Right];

    public Task<PartResult> RunAsync(IMeasure measure, string input)
    {
        var codes = input.Split(Environment.NewLine).Select(l => l).ToArray();
        Console.WriteLine(string.Join(Environment.NewLine, codes));

        var numericKeyPad = new[]
        {
            new[] { '7', '8', '9' },
            new[] { '4', '5', '6' },
            new[] { '1', '2', '3' },
            new[] { Empty, '0', 'A' }
        };

        var buttonsNumbericKeyPad = ButtonsOnKeyPad(numericKeyPad);
        var numericKeyPadWays = KeyPadWays(buttonsNumbericKeyPad, numericKeyPad, Directions);

        Console.WriteLine("Numeric Key Pad");
        foreach (var padWay in numericKeyPadWays.OrderBy(k => k.Key.From).ThenBy(k => k.Key.To))
        {
            Console.WriteLine(
                $"| {padWay.Key.From} - {padWay.Key.To}: {string.Join("", padWay.Value.Steps.Select(d => d.Symbol))}");
        }

        var directionalKeyPad = new[]
        {
            new[] { Empty, '^', 'A' },
            new[] { '<', 'v', '>' }
        };

        var buttonsDirectionalKeyPad = ButtonsOnKeyPad(directionalKeyPad);
        var directionalKeyPadWays = KeyPadWays(buttonsDirectionalKeyPad, directionalKeyPad, Directions);

        Console.WriteLine("Directional Key Pad");
        foreach (var padWay in directionalKeyPadWays.OrderBy(k => k.Key.From).ThenBy(k => k.Key.To))
        {
            Console.WriteLine(
                $"| {padWay.Key.From} - {padWay.Key.To}: {string.Join("", padWay.Value.Steps.Select(d => d.Symbol))}");
        }

        var solve = new Dictionary<string, char[]>();
        foreach (var code in codes)
        {
            var totalKeyPadWay = new List<char>();

            var fromNumericalKeyPad = 'A';
            foreach (var c in code)
            {
                if (fromNumericalKeyPad != c)
                {
                    var numericKeyPadWay = numericKeyPadWays[(fromNumericalKeyPad, c)];
                    totalKeyPadWay.AddRange(numericKeyPadWay.Steps.Select(d => d.Symbol));
                }

                totalKeyPadWay.Add('A');
                fromNumericalKeyPad = c;
            }

            const int amountOfDirectionalKeyPads = 2;
            for (var i = 0; i < amountOfDirectionalKeyPads; i++)
            {
                var fromDirectionalKeyPad = 'A';
                var totalDirectionalKeyPadWay = new List<char>();
                foreach (var symbol in totalKeyPadWay)
                {
                    if (fromDirectionalKeyPad != symbol)
                    {
                        var directionalKeyPadWay = directionalKeyPadWays[(fromDirectionalKeyPad, symbol)];
                        totalDirectionalKeyPadWay.AddRange(directionalKeyPadWay.Steps.Select(d => d.Symbol));
                    }

                    totalDirectionalKeyPadWay.Add('A');
                    fromDirectionalKeyPad = symbol;
                }

                totalKeyPadWay = totalDirectionalKeyPadWay.ToList();
                totalDirectionalKeyPadWay.Clear();
            }

            solve[code] = totalKeyPadWay.ToArray();
        }

        var expected = new Dictionary<string, string>
        {
            { "029A", "<vA<AA>>^AvAA<^A>A<v<A>>^AvA^A<vA>^A<v<A>^A>AAvA^A<v<A>A>^AAAvA<^A>A" },
            { "980A", "<v<A>>^AAAvA^A<vA<AA>>^AvAA<^A>A<v<A>A>^AAAvA<^A>A<vA>^A<A>A" },
            { "179A", "<v<A>>^A<vA<A>>^AAvAA<^A>A<v<A>>^AAvA^A<vA>^AA<A>A<v<A>A>^AAAvA<^A>A" },
            { "456A", "<v<A>>^AA<vA<A>>^AAvAA<^A>A<vA>^A<A>A<vA>^A<A>A<v<A>A>^AAvA<^A>A" },
            { "379A", "<v<A>>^AvA^A<vA<AA>>^AAvA<^A>AAvA^A<vA>^AA<A>A<v<A>A>^AAAvA<^A>A" }
        };

        var output = new StringBuilder();
        foreach (var (code, way) in solve)
        {
            output.AppendLine($"Mine      {code}: {string.Join("", way)} {way.Length}");

            if (expected.TryGetValue(code, out var expectedWay))
            {
                output.AppendLine($"Expected: {code}: {string.Join("", expectedWay)} {expectedWay.Length}");
            }
        }

        Console.WriteLine(output);

        var sum = 0;
        foreach (var (code, way) in solve)
        {
            var number = int.Parse(NumberRegex().Match(code).Value);
            sum += number * way.Length;
        }

        return Task.FromResult(new PartResult($"{sum}", $": {sum}"));
    }

    private static Dictionary<(char From, char To), Way> KeyPadWays(List<char> buttonsNumbericKeyPad,
        char[][] numericKeyPad, List<Direction> directions)
    {
        var numericKeyPadWays = new Dictionary<(char From, char To), Way>();

        foreach (var button in buttonsNumbericKeyPad)
        {
            foreach (var otherButton in buttonsNumbericKeyPad)
            {
                if (button == otherButton)
                {
                    continue;
                }

                var from = FindPosition(numericKeyPad, button);
                var to = FindPosition(numericKeyPad, otherButton);
                var path = Dijkstra(
                    directions.Select(d => new Node(from, d)).ToList(),
                    directions.Select(d => new Node(to, d)).ToList(),
                    n => GetNeighbors(numericKeyPad, n, directions),
                    (prev, curr) =>
                    {
                        var sum = 1;

                        if (directions.IndexOf(prev.Direction) > directions.IndexOf(curr.Direction))
                        {
                            sum += 1;
                        }

                        if (prev.Direction != curr.Direction)
                        {
                            sum += 1;
                        }

                        return sum;
                    }
                );

                if (path is null)
                {
                    throw new UnreachableException();
                }

                numericKeyPadWays[(button, otherButton)] = new Way(button, otherButton,
                    path.Value.Nodes.Skip(1).Select(n => n.Direction).ToArray());
            }
        }

        return numericKeyPadWays;
    }

    private static List<char> ButtonsOnKeyPad(char[][] keyPad)
    {
        var buttons = new List<char>();
        foreach (var t in keyPad)
        {
            buttons.AddRange(t.Where(c => c != Empty));
        }

        return buttons;
    }

    private record struct Way(char From, char To, Direction[] Steps);

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

    private static IEnumerable<Node> GetNeighbors(char[][] field, Node current, List<Direction> directions)
    {
        return from direction in directions
            select new Node(current.Position + direction, direction)
            into newPosition
            where newPosition.Position.X >= 0 && newPosition.Position.X < field[0].Length &&
                  newPosition.Position.Y >= 0 &&
                  newPosition.Position.Y < field.Length
            where field[newPosition.Position.Y][newPosition.Position.X] != Empty
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

    private record struct Direction(char Symbol, int X, int Y);

    [GeneratedRegex(@"\d*", RegexOptions.Compiled)]
    private static partial Regex NumberRegex();
}