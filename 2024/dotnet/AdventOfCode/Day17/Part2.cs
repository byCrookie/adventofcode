using System.Text.RegularExpressions;
using AdventOfCode.Days;
using AdventOfCode.Measure;

namespace AdventOfCode.Day17;

[Day(17)]
[Part(2)]
public partial class Part2 : IPart
{
    private const int OpCodeAdv = 0;
    private const int OpCodeBxl = 1;
    private const int OpCodeBst = 2;
    private const int OpCodeJnz = 3;
    private const int OpCodeBxc = 4;
    private const int OpCodeOut = 5;
    private const int OpCodeBdv = 6;
    private const int OpCodeCdv = 7;

    public Task<PartResult> RunAsync(IMeasure measure, string input)
    {
        var computer = ParseComputer(input);
        var a = FindA(computer.Program, computer.Program).Min();
        return Task.FromResult(new PartResult($"{a}", $"Output: {a}"));
    }

    private static IEnumerable<long> FindA(long[] program, long[] output)
    {
        if (output.Length == 0)
        {
            yield return 0;
            yield break;
        }

        foreach (var ahigh in FindA(program, output[1..]))
        {
            for (var alow = 0; alow < 8; alow++)
            {
                var a = ahigh * 8 + alow;
                if (Run(a, program).SequenceEqual(output))
                {
                    yield return a;
                }
            }
        }
    }

    private static List<long> Run(long a, long[] instructions)
    {
        var output = new List<long>();
        var registerA = a;
        var registerB = 0L;
        var registerC = 0L;
        var instructionPointer = 0L;
        while (instructionPointer < instructions.Length)
        {
            var instruction = instructions[instructionPointer];
            var instructionOutput = ExecuteInstruction(instruction, instructions, ref instructionPointer,
                ref registerA, ref registerB, ref registerC);
            if (!instructionOutput.HasValue)
            {
                continue;
            }

            output.Add(instructionOutput.Value);
        }

        return output;
    }

    private static long? ExecuteInstruction(long instruction, long[] instructions, ref long instructionPointer,
        ref long registerA, ref long registerB, ref long registerC)
    {
        switch (instruction)
        {
            case OpCodeAdv:
                var operandAdv = GetComboOperand(instructionPointer, instructions, ref registerA, ref registerB,
                    ref registerC);
                registerA = (long)(registerA / Math.Pow(2, operandAdv.Combo));
                instructionPointer += 2;
                return null;
            case OpCodeBxl:
                var operandBxl = GetComboOperand(instructionPointer, instructions, ref registerA, ref registerB,
                    ref registerC);
                registerB ^= operandBxl.Literal;
                instructionPointer += 2;
                return null;
            case OpCodeBst:
                var operandBst = GetComboOperand(instructionPointer, instructions, ref registerA, ref registerB,
                    ref registerC);
                registerB = operandBst.Combo % 8;
                instructionPointer += 2;
                return null;
            case OpCodeJnz:
                if (registerA == 0)
                {
                    instructionPointer += 2;
                    return null;
                }

                var operandJnz = GetComboOperand(instructionPointer, instructions, ref registerA, ref registerB,
                    ref registerC);
                instructionPointer = operandJnz.Literal;
                return null;
            case OpCodeBxc:
                registerB ^= registerC;
                instructionPointer += 2;
                return null;
            case OpCodeOut:
                var operandOut = GetComboOperand(instructionPointer, instructions, ref registerA, ref registerB,
                    ref registerC);
                var output = operandOut.Combo % 8;
                instructionPointer += 2;
                return output;
            case OpCodeBdv:
                var operandBdv = GetComboOperand(instructionPointer, instructions, ref registerA, ref registerB,
                    ref registerC);
                registerB = (int)(registerA / Math.Pow(2, operandBdv.Combo));
                instructionPointer += 2;
                return null;
            case OpCodeCdv:
                var operandCdv = GetComboOperand(instructionPointer, instructions, ref registerA, ref registerB,
                    ref registerC);
                registerC = (long)(registerA / Math.Pow(2, operandCdv.Combo));
                instructionPointer += 2;
                return null;
            default:
                return null;
        }
    }

    private static (long Literal, long Combo) GetComboOperand(long instructionPointer, long[] instructions,
        ref long registerA, ref long registerB,
        ref long registerC)
    {
        var nextInstructionPointer = instructionPointer + 1;
        if (nextInstructionPointer >= instructions.Length)
        {
            throw new ArgumentException($"No operand at position {nextInstructionPointer}, end of program reached");
        }

        var operand = instructions[nextInstructionPointer];
        var combo = operand switch
        {
            >= 0 and <= 3 => operand,
            4 => registerA,
            5 => registerB,
            6 => registerC,
            7 => 7,
            _ => throw new ArgumentException($"Invalid operand {operand}")
        };

        return (operand, combo);
    }

    private static Computer ParseComputer(string input)
    {
        List<long> program = [];

        foreach (var line in input.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries))
        {
            var match = ProgramRegex().Match(line);
            if (match.Success)
            {
                program.AddRange(match.Groups["Instructions"].Value.Split(",").Select(long.Parse));
            }
        }

        return new Computer(program.ToArray());
    }

    private record struct Computer(long[] Program);

    [GeneratedRegex("Program: (?<Instructions>.*)")]
    private static partial Regex ProgramRegex();
}