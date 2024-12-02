using AdventOfCode.Days;
using AdventOfCode.Measure;
using AdventOfCode.Utils;

namespace AdventOfCode.Day02;

[Day(2)]
[Part(1)]
public class Part1 : IPart
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
        var onlyDirection = Math.Sign(report[1] - report[0]);
        for (var i = 1; i < report.Length; i++)
        {
            var difference = report[i] - report[i - 1];

            if (Math.Abs(difference) is > 3 or < 1)
            {
                return false;
            }

            if (Math.Sign(difference) != onlyDirection)
            {
                return false;
            }
        }

        return true;
    }
}