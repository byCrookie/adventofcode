using System.Text.RegularExpressions;
using AdventOfCode.Days;
using AdventOfCode.Measure;

namespace AdventOfCode.Day13;

[Day(13)]
[Part(1)]
public partial class Part1 : IPart
{
    public Task<PartResult> RunAsync(IMeasure measure, string input)
    {
        var machines = new List<Machine>();
        foreach (var machineLines in input.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries).Chunk(3))
        {
            var buttonA = ParseButton(machineLines[0]);
            var buttonB = ParseButton(machineLines[1]);
            var prize = ParsePrize(machineLines[2]);

            machines.Add(new Machine(buttonA, buttonB, prize));
        }
        
        var fewestTokens = 0;
        foreach (var machine in machines)
        {
            var presses = FewestPressesToTarget(machine.A, machine.B, machine.Prize);
            var tokens = presses.Sum(b => b.Tokens);
            Console.WriteLine($"Button A: {machine.A.X}, {machine.A.Y}, {machine.A.Tokens}");
            Console.WriteLine($"Button B: {machine.B.X}, {machine.B.Y}, {machine.B.Tokens}");
            Console.WriteLine($"Prize: {machine.Prize.X}, {machine.Prize.Y}");
            var pressesA = presses.Count(b => b == machine.A);
            Console.WriteLine($"A: Presses={pressesA}, Tokens={pressesA * machine.A.Tokens}");
            var pressesB = presses.Count(b => b == machine.B);
            Console.WriteLine($"B: Presses={pressesB}, Tokens={pressesB * machine.B.Tokens}");
            Console.WriteLine($"Total: Presses={presses.Count}, Tokens={tokens}");
            Console.WriteLine();

            fewestTokens += tokens;
        }

        return Task.FromResult(new PartResult($"{fewestTokens}", $"Fewest Tokens to win all prizes: {fewestTokens}"));
    }

    private static List<Button> FewestPressesToTarget(Button a, Button b, Prize prize)
    {
        var pressesWithA = Backtrack(a, b, prize);
        var pressesWithB = Backtrack(b, a, prize);

        return pressesWithA.Sum(p => p.Tokens) <= pressesWithB.Sum(p => p.Tokens)
            ? pressesWithA
            : pressesWithB;
    }

    private static List<Button> Backtrack(Button one, Button two, Prize prize)
    {
        var position = new Position(0, 0);

        var onePresses = new Stack<Button>();
        var twoPresses = new Stack<Button>();

        for (var i = 0; i < Math.Min(prize.X / one.X, prize.Y / one.Y); i++)
        {
            onePresses.Push(one);
            position += one;
        }

        while (true)
        {
            var newPosition = position;

            var distanceToPrize = new Position(prize.X - position.X, prize.Y - position.Y);
            var twoPressesToPrize = Math.Min(distanceToPrize.X / two.X, distanceToPrize.Y / two.Y);
            for (var i = 0; i < twoPressesToPrize; i++)
            {
                newPosition += two;
            }

            if (newPosition == new Position(prize.X, prize.Y))
            {
                for (var i = 0; i < twoPressesToPrize; i++)
                {
                    twoPresses.Push(two);
                }

                position = newPosition;

                break;
            }

            if (onePresses.Count == 0)
            {
                break;
            }

            onePresses.Pop();
            position -= one;
        }

        return position != new Position(prize.X, prize.Y) ? [] : onePresses.Concat(twoPresses).ToList();
    }

    private static Button ParseButton(string line)
    {
        var match = ButtonRegex().Match(line);

        var x = match.Groups["XPlus"].Success
            ? int.Parse(match.Groups["XPlus"].Value)
            : int.Parse(match.Groups["XMinus"].Value);

        var y = match.Groups["YPlus"].Success
            ? int.Parse(match.Groups["YPlus"].Value)
            : int.Parse(match.Groups["YMinus"].Value);

        var type = match.Groups["Type"].Value;
        return new Button(x, y, type == "A" ? 3 : 1);
    }

    private static Prize ParsePrize(string line)
    {
        var match = PrizeRegex().Match(line);

        var x = match.Groups["XPlus"].Success
            ? int.Parse(match.Groups["XPlus"].Value)
            : int.Parse(match.Groups["XMinus"].Value);

        var y = match.Groups["YPlus"].Success
            ? int.Parse(match.Groups["YPlus"].Value)
            : int.Parse(match.Groups["YMinus"].Value);

        return new Prize(x, y);
    }

    private record struct Machine(Button A, Button B, Prize Prize);

    private record struct Button(int X, int Y, int Tokens);

    private record struct Prize(int X, int Y);

    private record struct Position(int X, int Y)
    {
        public static Position operator +(Position position, Button button)
        {
            return new Position(position.X + button.X, position.Y + button.Y);
        }

        public static Position operator -(Position position, Button button)
        {
            return new Position(position.X - button.X, position.Y - button.Y);
        }
    };

    [GeneratedRegex(@"Button (?<Type>A|B): X(\+(?<XPlus>\d*)|(?<XMinus>-\d*)), Y(\+(?<YPlus>\d*)|(?<YMinus>-\d*))")]
    private static partial Regex ButtonRegex();

    [GeneratedRegex(@"Prize: X=((?<XMinus>-\d*)|(?<XPlus>\d*)), Y=((?<YMinus>-\d*)|(?<YPlus>\d*))")]
    private static partial Regex PrizeRegex();
}