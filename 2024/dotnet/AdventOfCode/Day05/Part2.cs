using AdventOfCode.Days;
using AdventOfCode.Measure;

namespace AdventOfCode.Day05;

[Day(5)]
[Part(2)]
public class Part2 : IPart
{
    public Task<PartResult> RunAsync(IMeasure measure, string input)
    {
        var rules = new List<Rule>();
        var updates = new List<List<int>>();

        foreach (var line in input.Split(Environment.NewLine).Where(l => !string.IsNullOrWhiteSpace(l)))
        {
            if (line.Contains('|'))
            {
                rules.Add(new Rule(int.Parse(line.Split('|')[0]), int.Parse(line.Split('|')[1])));
            }
            else
            {
                updates.Add(line.Split(',').Select(int.Parse).ToList());
            }
        }

        measure.Now("Parsed");

        var reorderedUpdates = new List<List<int>>();
        foreach (var update in updates)
        {
            for (var i = 0; i < rules.Count; i++)
            {
                var rule = rules[i];
                var prevIndex = update.IndexOf(rule.Prev);
                var afterIndex = update.IndexOf(rule.After);

                if (prevIndex == -1 || afterIndex == -1)
                {
                    continue;
                }

                if (prevIndex <= afterIndex)
                {
                    continue;
                }

                i = 0;

                update.RemoveAt(prevIndex);
                update.Insert(afterIndex, rule.Prev);

                if (!reorderedUpdates.Contains(update))
                {
                    reorderedUpdates.Add(update);
                }
            }
        }

        var sum = reorderedUpdates.Sum(u => u[(int)Math.Floor((double)u.Count / 2)]);
        return Task.FromResult(new PartResult($"{sum}", $"Sum of all multiplications is {sum}"));
    }

    private record Rule(int Prev, int After);
}