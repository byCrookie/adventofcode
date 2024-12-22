using AdventOfCode.Days;
using AdventOfCode.Measure;

namespace AdventOfCode.Day22;

[Day(22)]
[Part(2)]
public class Part2 : IPart
{
    private const long IterationCount = 2000;

    public Task<PartResult> RunAsync(IMeasure measure, string input)
    {
        var initialSecrets = input.Split(Environment.NewLine).Select(ulong.Parse).ToArray();
        var secrets = new Secret[initialSecrets.Length];

        foreach (var initial in initialSecrets.Select((s, i) => (Index: i, Secret: s)))
        {
            var iterations = new Iteration[IterationCount + 1];
            secrets[initial.Index] = new Secret(initial.Secret, iterations);
            var secret = initial.Secret;

            for (var i = 0; i <= IterationCount; i++)
            {
                var price = (byte)(secret % 10);
                var change = i - 1 >= 0
                    ? (sbyte)(price - secrets[initial.Index].Iterations[i - 1].Price)
                    : (sbyte?)null;
                iterations[i] = new Iteration(secret, price, change);

                var multiply = secret * 64;
                secret = multiply ^ secret; // mix
                secret %= 16777216; // prune

                var divide = (ulong)Math.Floor(secret / 32d);
                secret = divide ^ secret; // mix
                secret %= 16777216; // prune;

                var multiply2 = secret * 2048;
                secret = multiply2 ^ secret; // mix
                secret %= 16777216; // prune
            }
        }

        const sbyte start = -9;
        const sbyte length = 9 + 9 + 1;
        var fourLongChanges = (from i in Enumerable.Range(start, length)
            from j in Enumerable.Range(start, length)
            from k in Enumerable.Range(start, length)
            from l in Enumerable.Range(start, length)
            select (AsString: $"{i}{j}{k}{l}", AsArray: new[] { (sbyte)i, (sbyte)j, (sbyte)k, (sbyte)l })).ToList();

        var secretChanges = new Dictionary<int, string>(secrets.Length);
        foreach (var (index, secret) in secrets.Select((s, i) => (Index: i, Secret: s)))
        {
            secretChanges[index] = string.Join("", secret.Iterations.Skip(1).Select(i => i.Change));
        }

        var sequences = fourLongChanges
            .Where(c => secretChanges.Any(s => s.Value.Contains(c.AsString)))
            .ToDictionary(c => c, _ => 0);

        var counter = 0;
        foreach (var secretChange in secretChanges)
        {
            foreach (var sequence in sequences)
            {
                if (!secretChange.Value.Contains(sequence.Key.AsString))
                {
                    continue;
                }

                var secret = secrets[secretChange.Key];
                sequences[sequence.Key] += Price(sequence.Key.AsArray, secret);
                continue;

                static int Price(sbyte[] sequence, Secret secret)
                {
                    for (var i = 0; i < secret.Iterations.Length; i++)
                    {
                        if (secret.Iterations[i].Change == sequence[0])
                        {
                            var found = true;
                            for (var j = 1; j < sequence.Length; j++)
                            {
                                if (i + j >= secret.Iterations.Length)
                                {
                                    found = false;
                                    break;
                                }

                                if (secret.Iterations[i + j].Change != sequence[j])
                                {
                                    found = false;
                                    break;
                                }
                            }

                            if (found)
                            {
                                return secret.Iterations[i + 3].Price;
                            }
                        }
                    }

                    return 0;
                }
            }

            counter++;
            Console.WriteLine($"{counter}/{secretChanges.Count}");
        }

        var max = sequences.Max(s => s.Value);
        return Task.FromResult(new PartResult($"{max}", $"Max Bananas: {max}"));
    }

    private record struct Secret(ulong Initial, Iteration[] Iterations);

    private record struct Iteration(ulong Secret, byte Price, sbyte? Change);
}