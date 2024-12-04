using AdventOfCode.Days;
using AdventOfCode.Measure;

namespace AdventOfCode.Day04;

[Day(4)]
[Part(2)]
public class Part2 : IPart
{
    public Task<PartResult> RunAsync(IMeasure measure, string input)
    {
        var lines = input.Split(Environment.NewLine);
        var field = new char[lines.Length][];
        for (var i = 0; i < lines.Length; i++)
        {
            field[i] = lines[i].ToCharArray();
        }

        measure.Now("Parsed");

        var coords = new List<Coord>();
        for (var row = 0; row < field.Length; row++)
        {
            for (var col = 0; col < field[row].Length; col++)
            {
                coords.Add(new Coord(col, row, field[row].Length, field.Length));
            }
        }

        var sum = coords.Sum(coord => Search(field, coord));
        return Task.FromResult(new PartResult($"{sum}", $"Sum of all multiplications is {sum}"));
    }

    private static int Search(char[][] field, Coord coord)
    {
        if (field[coord.Y][coord.X] != 'A')
        {
            return 0;
        }

        for (var row = -1; row <= 1; row++)
        {
            for (var col = -1; col <= 1; col++)
            {
                if (coord.X + row < 0 || coord.X + row >= coord.MaxX || coord.Y + col < 0 ||
                    coord.Y + col >= coord.MaxY)
                {
                    return 0;
                }
            }
        }

        var topLeft = field[coord.Y - 1][coord.X - 1];
        var topRight = field[coord.Y - 1][coord.X + 1];
        var bottomLeft = field[coord.Y + 1][coord.X - 1];
        var bottomRight = field[coord.Y + 1][coord.X + 1];

        if (((topLeft == 'M' && bottomRight == 'S') || (topLeft == 'S' && bottomRight == 'M'))
            && ((topRight == 'M' && bottomLeft == 'S') || (topRight == 'S' && bottomLeft == 'M')))
        {
            return 1;
        }

        return 0;
    }

    private record Coord(int X, int Y, int MaxX, int MaxY);
}