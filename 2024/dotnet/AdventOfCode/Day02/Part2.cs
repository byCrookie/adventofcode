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

        var safeReports = new List<List<int>>();
        foreach (var report in reports)
        {
            var isSafe = IsReportSafe(report.ToArray());
            if (isSafe)
            {
                safeReports.Add(report);
                Console.WriteLine($"Safe report: {string.Join(" ", report)}");
            }
            else
            {
                Console.WriteLine($"Unsafe report: {string.Join(" ", report)}");
            }
        }

        var safeReportsCount = safeReports.Count;
        return Task.FromResult(new PartResult(safeReportsCount.ToString(), $"Safe reports: {safeReportsCount}"));
    }

    private static bool IsReportSafe(int[] report)
    {
        var prevLevel = report[0];
        var lastLevel = report[^1];
        var directionChangeCount = 0;
        var prevAssending = prevLevel < lastLevel;

        for (var i = 1; i < report.Length; i++)
        {
            var currentLevel = report[i];
            var currentAssending = prevLevel < currentLevel;
            var diff = Math.Abs(currentLevel - prevLevel);

            if (prevAssending != currentAssending || diff == 0)
            {
                directionChangeCount++;
            }
            
            if (directionChangeCount > 2)
            {
                // difference is too big
                return false;
            }

            if (diff is > 3 or < 1 && diff != 0)
            {
                // difference is too big
                return false;
            }

            prevLevel = currentLevel;
            prevAssending = currentAssending;
        }

        return true;
    }
}