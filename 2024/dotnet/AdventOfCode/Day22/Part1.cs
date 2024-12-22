using AdventOfCode.Days;
using AdventOfCode.Measure;

namespace AdventOfCode.Day22;

[Day(22)]
[Part(1)]
public class Part1 : IPart
{
    private const long Iterations = 2000;

    public Task<PartResult> RunAsync(IMeasure measure, string input)
    {
        var initialSecrets = input.Split(Environment.NewLine).Select(long.Parse).ToArray();
        var iterated = new (long Initial, long Secret)[initialSecrets.Length];

        foreach (var initial in initialSecrets.Select((s, i) => (Index: i, Secret: s)))
        {
            var secret = initial.Secret;

            for (var i = 0; i < Iterations; i++)
            {
                var multiply = secret * 64;
                secret = multiply ^ secret; // mix
                secret %= 16777216; // prune

                var divide = (long)Math.Floor(secret / 32d);
                secret = divide ^ secret; // mix
                secret %= 16777216; // prune;

                var multiply2 = secret * 2048;
                secret = multiply2 ^ secret; // mix
                secret %= 16777216; // prune
            }

            iterated[initial.Index] = (initial.Secret, secret);
        }

        foreach (var (initial, secret) in iterated)
        {
            Console.WriteLine($"{initial}: {secret}");
        }
        
        var sum = iterated.Sum(i => i.Secret);
        return Task.FromResult(new PartResult($"{sum}", $"Sum: {sum}"));
    }
}