using AdventOfCode.Days;
using AdventOfCode.Measure;
using AdventOfCode.Utils;

namespace AdventOfCode.Day02;

[Day(2)]
[Part(2)]
public class Part2 : IPart
{
    public Task<PartResult> RunAsync(IMeasure measure, string input)
    {
        var reports = new List<List<int>>();
        foreach (ReadOnlySpan<char> line in input.Trim().SplitLines())
        {
            var splitted = line.Split(" ");
            var report = new List<int>();
            while (splitted.MoveNext())
            {
                var level = int.Parse(line.Slice(splitted.Current.Start.Value,
                    splitted.Current.End.Value - splitted.Current.Start.Value));
                report.Add(level);
            }

            reports.Add(report);
        }

        measure.Now("Parsed");

        var safeReportsCount = reports.Count(r => IsReportSafe(r.ToArray()));
        return Task.FromResult(new PartResult(safeReportsCount.ToString(), $"Safe reports: {safeReportsCount}"));
    }

    private static bool IsReportSafe(int[] report)
    {
        var directionChanges = 0;
        var prevDirection = Math.Sign(report[^1] - report[0]);
        for (var i = 1; i < report.Length; i++)
        {
            var difference = report[i] - report[i - 1];

            if (Math.Abs(difference) is (> 3 or < 1) and not 0)
            {
                return false;
            }

            var direction = difference > 0 ? 1 : -1;
            if (direction != prevDirection || difference == 0)
            {
                directionChanges++;
                if (directionChanges > 2)
                {
                    return false;
                }
            }

            prevDirection = direction;
        }

        return true;
    }
}