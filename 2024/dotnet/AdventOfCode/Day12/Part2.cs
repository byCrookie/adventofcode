using System.Diagnostics;
using AdventOfCode.Days;
using AdventOfCode.Measure;

namespace AdventOfCode.Day12;

[Day(12)]
[Part(2)]
public class Part2 : IPart
{
    private static readonly List<Direction> Directions =
    [
        new(1, 0),
        new(0, 1),
        new(-1, 0),
        new(0, -1)
    ];
    
    private static readonly List<Direction> DirectionsAndDiagonal =
    [
        new(1, 0),
        new(0, 1),
        new(-1, 0),
        new(0, -1),
        new(1, 1),
        new(-1, 1),
        new(-1, -1),
        new(1, -1)
    ];
    
    private static readonly List<Direction> DiagonalDirections =
    [
        new(1, 1),
        new(-1, 1),
        new(-1, -1),
        new(1, -1)
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
            var perimeter = new HashSet<Position>();
            foreach (var position in area.Positions)
            {
                foreach (var direction in DirectionsAndDiagonal)
                {
                    var newPosition = position + direction;
                    if (!InBounds(field, newPosition) || field[newPosition.Y][newPosition.X] != area.Type)
                    {
                        perimeter.Add(newPosition);
                    }
                }
            }
            
            var sides = 1;
            var start = perimeter.ToList()[Random.Shared.Next(0, perimeter.Count)];
            var moved = new HashSet<Position>{start};
            foreach (var direction in Directions.Where(d => perimeter.Contains(start + d)))
            {
                var position = start + direction;
                sides += WalkSides(start, field, position, direction, perimeter, moved, false);
            }
            Console.WriteLine($"{area.Type} {area.Positions.Count} * {sides} = {area.Positions.Count * sides}");

            regions.Add(new Region(area, perimeter, sides));
        }

        var totalPrice = 0;
        foreach (var region in regions)
        {
            var price = region.Area.Positions.Count * region.Sides;
            Console.WriteLine($"{region.Area.Type} {region.Area.Positions.Count} * {region.Sides} = {price}");
            totalPrice += price;
        }

        return Task.FromResult(new PartResult($"{totalPrice}", $"Total price: {totalPrice}"));
    }

    private static int WalkSides(Position start, char[][] field, Position position, Direction previousDirection, HashSet<Position> perimeter,
        HashSet<Position> moved, bool canWalkAgain)
    {
        moved.Add(position);
        
        PrintField(field, perimeter, position);
        
        var nextDirections = Directions.Where(d => perimeter.Contains(position + d) && !moved.Contains(position + d)).ToList();

        var canWalkBack = canWalkAgain;
        if (nextDirections.Count == 0 && canWalkAgain)
        {
            nextDirections = DiagonalDirections.Where(d => perimeter.Contains(position + d)).ToList();
            
            if (nextDirections.Count != 1)
            {
                nextDirections = Directions.Where(d => perimeter.Contains(position + d) && !moved.Contains(position + d)).ToList();
                canWalkBack = false;
            }
        }
        
        var sides = 0;
        foreach (var direction in nextDirections)
        {
            var newPosition = position + direction;
            if (previousDirection != direction)
            {
                sides += 1;
            }
            
            sides += WalkSides(start, field, newPosition, direction, perimeter, moved, canWalkBack);
        }
        
        return sides;
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
    
    private static void PrintField(char[][] field, HashSet<Position> perimeter, Position position)
    {
        Console.Clear();
        
        var xLength = field[0].Length;
        var yLength = field.Length;

        for (var y = -1; y <= yLength; y++)
        {
            for (var x = -1; x <= xLength; x++)
            {
                if (position.X == x && position.Y == y)
                {
                    Console.Write("x");
                }
                else if (perimeter.Contains(new Position(x, y)))
                {
                    Console.Write("#");
                } 
                else if (InBounds(field, new Position(x, y)))
                {
                    Console.Write(field[y][x]);
                }
                else
                {
                    Console.Write(".");
                }
            }
            
            Console.WriteLine();
        }
    }
    
    private record struct Move(Position Position, Direction Direction);

    private record struct Region(Area Area, HashSet<Position> Permimeter, int Sides);

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