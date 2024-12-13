using System.Text.RegularExpressions;
using AdventOfCode.Days;
using AdventOfCode.Measure;

namespace AdventOfCode.Day13;

[Day(13)]
[Part(2)]
public partial class Part2 : IPart
{
    private const long Addition = 10000000000000;

    public Task<PartResult> RunAsync(IMeasure measure, string input)
    {
        var machines =
            (from machineLines in input.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries).Chunk(3)
                let buttonA = ParseButton(machineLines[0])
                let buttonB = ParseButton(machineLines[1])
                let prize = ParsePrize(machineLines[2])
                select new Machine(buttonA, buttonB, new Prize(X: prize.X + Addition, Y: prize.Y + Addition))).ToList();

        var fewestTokens = 0L;
        foreach (var machine in machines)
        {
            var (won, tokens) = Minimize(machine);
            Console.WriteLine($"Button A: {machine.A.X}, {machine.A.Y}, {machine.A.Tokens}");
            Console.WriteLine($"Button B: {machine.B.X}, {machine.B.Y}, {machine.B.Tokens}");
            Console.WriteLine($"Prize: {machine.Prize.X}, {machine.Prize.Y}");
            Console.WriteLine(won ? $"Won with {tokens} tokens" : "Lost");
            Console.WriteLine();
            fewestTokens += tokens;
        }

        return Task.FromResult(new PartResult($"{fewestTokens}", $"Fewest Tokens to win all prizes: {fewestTokens}"));
    }

    private static (bool won, long tokens) Minimize(Machine machine)
    {
        double aX = machine.A.X, aY = machine.A.Y;
        double bX = machine.B.X, bY = machine.B.Y;
        double pX = machine.Prize.X, pY = machine.Prize.Y;

        var factor = aY / aX;
        var yB = (pY - pX * factor) / (bY - bX * factor);
        var yA = (pY - yB * bY) / aY;

        var y = (yAr: (long)Math.Round(yA), yBr: (long)Math.Round(yB));

        var resultX = (long)(aX * y.yAr + bX * y.yBr);
        var resultY = (long)(aY * y.yAr + bY * y.yBr);

        if (resultX != machine.Prize.X || resultY != machine.Prize.Y)
        {
            return (false, 0);
        }

        return (true, 3 * y.yAr + y.yBr);
    }

    private static Button ParseButton(string line)
    {
        var match = ButtonRegex().Match(line);

        var x = match.Groups["XPlus"].Success
            ? long.Parse(match.Groups["XPlus"].Value)
            : long.Parse(match.Groups["XMinus"].Value);

        var y = match.Groups["YPlus"].Success
            ? long.Parse(match.Groups["YPlus"].Value)
            : long.Parse(match.Groups["YMinus"].Value);

        var type = match.Groups["Type"].Value;
        return new Button(x, y, type == "A" ? 3 : 1);
    }

    private static Prize ParsePrize(string line)
    {
        var match = PrizeRegex().Match(line);

        var x = match.Groups["XPlus"].Success
            ? long.Parse(match.Groups["XPlus"].Value)
            : long.Parse(match.Groups["XMinus"].Value);

        var y = match.Groups["YPlus"].Success
            ? long.Parse(match.Groups["YPlus"].Value)
            : long.Parse(match.Groups["YMinus"].Value);

        return new Prize(x, y);
    }

    private record struct Machine(Button A, Button B, Prize Prize);

    private record struct Button(long X, long Y, long Tokens);

    private record struct Prize(long X, long Y);

    [GeneratedRegex(@"Button (?<Type>A|B): X(\+(?<XPlus>\d*)|(?<XMinus>-\d*)), Y(\+(?<YPlus>\d*)|(?<YMinus>-\d*))")]
    private static partial Regex ButtonRegex();

    [GeneratedRegex(@"Prize: X=((?<XMinus>-\d*)|(?<XPlus>\d*)), Y=((?<YMinus>-\d*)|(?<YPlus>\d*))")]
    private static partial Regex PrizeRegex();
}