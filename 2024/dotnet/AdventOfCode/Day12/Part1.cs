using System.Diagnostics;
using AdventOfCode.Days;
using AdventOfCode.Measure;

namespace AdventOfCode.Day12;

[Day(12)]
[Part(1)]
public class Part1 : IPart
{
    private static readonly List<Direction> Directions =
    [
        new(1, 0),
        new(0, 1),
        new(-1, 0),
        new(0, -1)
    ];

    public Task<PartResult> RunAsync(IMeasure measure, string input)
    {
        var field = CreateField(input);
        var areas = new List<Area>();
        for (var y = 0; y < field.Length; y++)
        {
            for (var x = 0; x < field[y].Length; x++)
            {
                var start = new Position(x, y);
                var positionsInArea = new HashSet<Position> { start };
                DiscoverArea(field, start, positionsInArea);
                var area = new Area(field[y][x], [..positionsInArea]);
                if (positionsInArea.Count != area.Positions.Count)
                {
                    throw new UnreachableException(
                        "The positions in the area are not the same as the discovered positions.");
                }

                areas.Add(area);
            }
        }

        var distinctAreas = new List<Area>();
        foreach (var area in areas.Where(area =>
                     !distinctAreas.Any(oa => area.Type == oa.Type && oa.Positions.Overlaps(area.Positions))))
        {
            distinctAreas.Add(area);
        }

        var regions = new List<Region>();
        foreach (var area in distinctAreas)
        {
            var perimeter = new List<Position>();
            foreach (var position in area.Positions)
            {
                foreach (var direction in Directions)
                {
                    var newPosition = position + direction;
                    if (!InBounds(field, newPosition) || field[newPosition.Y][newPosition.X] != area.Type)
                    {
                        perimeter.Add(newPosition);
                    }
                }
            }

            regions.Add(new Region(area, perimeter));
        }

        var totalPrice = 0;
        foreach (var region in regions)
        {
            var price = region.Area.Positions.Count * region.Permimeter.Count;
            Console.WriteLine($"{region.Area.Positions.Count} * {region.Permimeter.Count} = {price}");
            totalPrice += price;
        }

        return Task.FromResult(new PartResult($"{totalPrice}", $"Total price: {totalPrice}"));
    }

    private static void DiscoverArea(char[][] field, Position position, HashSet<Position> areaPositions)
    {
        var type = field[position.Y][position.X];
        foreach (var direction in Directions)
        {
            var newPosition = position + direction;
            if (InBounds(field, newPosition) && field[newPosition.Y][newPosition.X] == type &&
                areaPositions.Add(newPosition))
            {
                DiscoverArea(field, newPosition, areaPositions);
            }
        }
    }

    private static bool InBounds(char[][] field, Position position)
    {
        return position.X >= 0 && position.X < field[0].Length && position.Y >= 0 && position.Y < field.Length;
    }

    private static char[][] CreateField(string input)
    {
        var lines = input.Split(Environment.NewLine);
        var field = new char[lines.Length][];
        for (var y = 0; y < lines.Length; y++)
        {
            field[y] = lines[y].ToCharArray();
        }

        return field;
    }

    private record struct Region(Area Area, List<Position> Permimeter);

    private record struct Area(char Type, HashSet<Position> Positions);

    private record struct Position(int X, int Y)
    {
        public static Position operator +(Position position, Direction direction)
        {
            return new Position(position.X + direction.X, position.Y + direction.Y);
        }
    }

    private record struct Direction(int X, int Y);
}