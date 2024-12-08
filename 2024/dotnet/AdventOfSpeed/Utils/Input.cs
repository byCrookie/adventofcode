using System.Reflection;
using AdventOfCode.Days;

namespace AdventOfSpeed.Utils;

public static class Input
{
    public static string Path<T>() where T : IPart
    {
        var dayAttribute = typeof(T).GetCustomAttribute<DayAttribute>()!;
        var partAttribute = typeof(T).GetCustomAttribute<PartAttribute>()!;

        var dayId = $"day{dayAttribute.Day.ToString().PadLeft(2, '0')}";
        var partId = $"part{partAttribute.Part.ToString().PadLeft(2, '0')}";

        return System.IO.Path.Combine("data", dayId, partId, "input.txt");
    }
}