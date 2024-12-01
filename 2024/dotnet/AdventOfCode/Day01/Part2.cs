using AdventOfCode.Days;
using AdventOfCode.Measure;
using AdventOfCode.Utils;

namespace AdventOfCode.Day01;

[Day(1)]
[Part(2)]
public class Part2 : IPart
{
    public Task<PartResult> RunAsync(IMeasure measure, string input)
    {
        const int capacity = 1000;
        List<int> leftNumbers = [capacity];
        Dictionary<int, int> rightNumberCounts = [];

        foreach (ReadOnlySpan<char> line in input.Trim().SplitLines())
        {
            var splitted = line.Split("   ");

            splitted.MoveNext();
            var leftNumber = int.Parse(line.Slice(splitted.Current.Start.Value,
                splitted.Current.End.Value - splitted.Current.Start.Value));
            leftNumbers.Add(leftNumber);

            splitted.MoveNext();
            var rightNumber = int.Parse(line.Slice(splitted.Current.Start.Value,
                splitted.Current.End.Value - splitted.Current.Start.Value));
            if (!rightNumberCounts.TryAdd(rightNumber, 1))
            {
                rightNumberCounts[rightNumber] += 1;
            }
        }

        measure.Now("Parsed");

        var similarityScore = 0;
        foreach (var leftNumber in leftNumbers)
        {
            if (rightNumberCounts.TryGetValue(leftNumber, out var count))
            {
                similarityScore += leftNumber * count;
            }
        }

        return Task.FromResult(new PartResult(similarityScore.ToString(), $"Similarity score: {similarityScore}"));
    }
}