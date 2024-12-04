using AdventOfCode.Days;
using AdventOfCode.Measure;

namespace AdventOfCode.Day04;

[Day(4)]
[Part(1)]
public class Part1 : IPart
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

        var sum = coords.Sum(coord => Search(field, coord, "XMAS".ToCharArray()));
        return Task.FromResult(new PartResult($"{sum}", $"Sum of all multiplications is {sum}"));
    }

    private static int Search(char[][] field, Coord coord, char[] search)
    {
        var directions = new List<(int X, int Y)>();
        for (var row = -1; row <= 1; row++)
        {
            for (var col = -1; col <= 1; col++)
            {
                directions.Add((Math.Sign(col), Math.Sign(row)));
            }
        }

        return directions.Sum(direction => SearchDirection(field, coord, search, direction.X, direction.Y));
    }

    private static int SearchDirection(char[][] field, Coord coord, char[] search, int xMove, int yMove)
    {
        if (field[coord.Y][coord.X] != search[0])
        {
            return 0;
        }

        if (search.Length == 1)
        {
            return 1;
        }

        if (coord.X + xMove < 0 || coord.X + xMove >= coord.MaxX || coord.Y + yMove < 0 ||
            coord.Y + yMove >= coord.MaxY)
        {
            return 0;
        }

        return SearchDirection(field, coord with { X = coord.X + xMove, Y = coord.Y + yMove }, search[1..], xMove,
            yMove);
    }

    private record Coord(int X, int Y, int MaxX, int MaxY);
}