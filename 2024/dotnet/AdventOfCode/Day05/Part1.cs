using AdventOfCode.Days;
using AdventOfCode.Measure;

namespace AdventOfCode.Day05;

[Day(5)]
[Part(1)]
public class Part1 : IPart
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

        var ruleSet = new Dictionary<int, List<int>>();
        foreach (var rule in rules)
        {
            if (!ruleSet.TryGetValue(rule.Prev, out var value))
            {
                value = [];
                ruleSet[rule.Prev] = value;
            }

            value.Add(rule.After);
        }

        Console.WriteLine($"Ruleset: {string.Join(", ", ruleSet)}");
        measure.Now("Ordered");

        var validUpdates = new List<List<int>>();
        foreach (var update in updates)
        {
            var valid = true;
            for (var i = 1; i < update.Count; i++)
            {
                var prev = update[i - 1];
                var after = update[i];
                if (ruleSet.TryGetValue(prev, out var value) && value.Contains(after))
                {
                    continue;
                }

                valid = false;
                break;
            }

            if (valid)
            {
                validUpdates.Add(update);
            }
        }

        measure.Now("Validated");

        var sum = validUpdates.Sum(u => u[(int)Math.Floor((double)u.Count / 2)]);
        return Task.FromResult(new PartResult($"{sum}", $"Sum of all multiplications is {sum}"));
    }

    private record Rule(int Prev, int After);
}