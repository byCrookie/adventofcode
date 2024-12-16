using AdventOfCode.Days;
using AdventOfCode.Measure;

namespace AdventOfCode.Day16;

[Day(16)]
[Part(2)]
public class Part2 : IPart
{
    public Task<PartResult> RunAsync(IMeasure measure, string input)
    {
        var grid = ReadGrid(input.Split(Environment.NewLine));
        var start = FindStart(grid);
        var result = FindMinimumScore(grid, start, Right);

        var counter = 0;
        var cheapest = result.Item2.GroupBy(r => r.Key.Item1).Select(g => g.OrderBy(b => b.Value).First()).ToList();
        foreach (var entry in cheapest)
        {
            var subResult = FindMinimumScore(grid, entry.Key.Item1, entry.Key.Item2);
            if (subResult.Item1 + entry.Value == result.Item1)
            {
                counter++;
            }
        }
        counter++;
        return Task.FromResult(new PartResult($"{counter}", $"Locations on shortest paths: {counter}"));
    }

    private static Position FindStart(char[,] grid)
    {
        for (var i = 0; i < grid.GetLength(0); i++)
        {
            for (var j = 0; j < grid.GetLength(1); j++)
            {
                if (grid[i, j] != 'S')
                {
                    continue;
                }

                return new Position(i, j);
            }
        }

        throw new InvalidOperationException("Start position not found");
    }

    private static readonly Direction Right = new(0, 1);
    private static readonly Direction Up = new(-1, 0);
    private static readonly Direction Left = new(0, -1);
    private static readonly Direction Down = new(1, 0);

    private static readonly List<Direction> Directions =
    [
        Right,
        Up,
        Left,
        Down
    ];

    private static readonly int[] RotateClockwise = [1, 2, 3, 0];
    private static readonly int[] RotateCounterClockwise = [3, 0, 1, 2];

    private static (int, Dictionary<(Position, Direction), int>) FindMinimumScore(char[,] grid, Position start, Direction startDir)
    {
        var rows = grid.GetLength(0);
        var cols = grid.GetLength(1);

        var queue = new PriorityQueue<(Position, Direction), int>();
        var score = new Dictionary<(Position, Direction), int>();

        for (var i = 0; i < rows; i++)
        {
            for (var j = 0; j < cols; j++)
            {
                for (var d = 0; d < 4; d++)
                {
                    score[(new Position(i, j), Directions[d])] = int.MaxValue;
                }
            }
        }

        queue.Enqueue((start, startDir), 0);
        score[(start, startDir)] = 0;

        while (queue.Count > 0)
        {
            var (pos, dir) = queue.Dequeue();
            var currentScore = score[(pos, dir)];

            if (grid[pos.X, pos.Y] == 'E')
            {
                return (currentScore, score);
            }

            var newPos = pos + dir;
            
            if (IsValidMove(newPos, grid))
            {
                if (currentScore + 1 < score[(newPos, dir)])
                {
                    score[(newPos, dir)] = currentScore + 1;
                    queue.Enqueue((newPos, dir), currentScore + 1);
                }
            }
            
            var newDirClockwise = Directions[RotateClockwise[Directions.IndexOf(dir)]];
            if (currentScore + 1000 < score[(pos, newDirClockwise)])
            {
                score[(pos, newDirClockwise)] = currentScore + 1000;
                queue.Enqueue((pos, newDirClockwise), currentScore + 1000);
            }
            
            var newDirCounterClockwise = Directions[RotateCounterClockwise[Directions.IndexOf(dir)]];
            if (currentScore + 1000 < score[(pos, newDirCounterClockwise)])
            {
                score[(pos, newDirCounterClockwise)] = currentScore + 1000;
                queue.Enqueue((pos, newDirCounterClockwise), currentScore + 1000);
            }
        }
        
        return (-1, score);
    }

    private static bool IsValidMove(Position p, char[,] grid)
    {
        var rows = grid.GetLength(0);
        var cols = grid.GetLength(1);
        return p is { X: >= 0, Y: >= 0 } && p.X < rows && p.Y < cols && grid[p.X, p.Y] != '#';
    }

    private static char[,] ReadGrid(string[] input)
    {
        var rows = input.Length;
        var cols = input[0].Length;
        var grid = new char[rows, cols];

        for (var i = 0; i < rows; i++)
        {
            for (var j = 0; j < cols; j++)
            {
                grid[i, j] = input[i][j];
            }
        }

        return grid;
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