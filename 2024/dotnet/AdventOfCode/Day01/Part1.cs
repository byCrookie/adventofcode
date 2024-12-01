using AdventOfCode.Days;
using AdventOfCode.Measure;
using AdventOfCode.Utils;

namespace AdventOfCode.Day01;

[Day(1)]
[Part(1)]
public class Part1 : IPart
{
    public Task<PartResult> RunAsync(IMeasure measure, string input)
    {
        const int capacity = 1000;
        var leftNumbers = new int[capacity];
        var rightNumbers = new int[capacity];

        var index = -1;
        foreach (ReadOnlySpan<char> line in input.Trim().SplitLines())
        {
            index++;
            var splitted = line.Split("   ");

            splitted.MoveNext();
            leftNumbers[index] = int.Parse(line.Slice(splitted.Current.Start.Value,
                splitted.Current.End.Value - splitted.Current.Start.Value));

            splitted.MoveNext();
            rightNumbers[index] = int.Parse(line.Slice(splitted.Current.Start.Value,
                splitted.Current.End.Value - splitted.Current.Start.Value));
        }

        measure.Now("Parsed");

        Array.Sort(leftNumbers, 0, index + 1);
        Array.Sort(rightNumbers, 0, index + 1);

        measure.Now("Sorted");

        var distance = 0;
        for (var i = 0; i <= index; i++)
        {
            distance += Math.Abs(leftNumbers[i] - rightNumbers[i]);
        }

        return Task.FromResult(new PartResult(distance.ToString(), $"Distance: {distance}"));
    }
}