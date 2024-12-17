using System.Text.RegularExpressions;
using AdventOfCode.Days;
using AdventOfCode.Measure;

namespace AdventOfCode.Day17;

[Day(17)]
[Part(1)]
public partial class Part1 : IPart
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
        var (registerA, registerB, registerC, instructions) = ParseComputer(input);

        var instructionPointer = 0;
        var output = new List<int>();
        while (instructionPointer < instructions.Length)
        {
            Console.WriteLine($"Instructionpointer: {instructionPointer}");

            var instruction = instructions[instructionPointer];
            var instructionOutput = ExecuteInstruction(instruction, instructions, ref instructionPointer,
                ref registerA, ref registerB, ref registerC);
            if (instructionOutput.HasValue)
            {
                Console.WriteLine($"Output: {instructionOutput.Value}");
                output.Add(instructionOutput.Value);
            }

            Console.WriteLine($"A: {registerA} B: {registerB} C:{registerC}");
            Console.WriteLine($"Total output: {string.Join(",", output)}");
        }

        var result = string.Join(",", output);
        return Task.FromResult(new PartResult($"{result}", $"Output: {result}"));
    }

    private static int? ExecuteInstruction(int instruction, int[] instructions, ref int instructionPointer,
        ref int registerA, ref int registerB, ref int registerC)
    {
        switch (instruction)
        {
            case OpCodeAdv:
                var operandAdv = GetComboOperand(instructionPointer, instructions, ref registerA, ref registerB,
                    ref registerC);
                registerA = (int)(registerA / Math.Pow(2, operandAdv.Combo));
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
                registerC = (int)(registerA / Math.Pow(2, operandCdv.Combo));
                instructionPointer += 2;
                return null;
            default:
                return null;
        }
    }

    private static (int Literal, int Combo) GetComboOperand(int instructionPointer, int[] instructions,
        ref int registerA, ref int registerB,
        ref int registerC)
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
        var registerA = 0;
        var registerB = 0;
        var registerC = 0;
        List<int> program = [];

        foreach (var line in input.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries))
        {
            var match = RegisterRegex().Match(line);
            if (match.Success)
            {
                var value = int.Parse(match.Groups["Value"].Value);
                switch (match.Groups["Register"].Value)
                {
                    case "A":
                        registerA = value;
                        break;
                    case "B":
                        registerB = value;
                        break;
                    case "C":
                        registerC = value;
                        break;
                }
            }

            match = ProgramRegex().Match(line);
            if (match.Success)
            {
                program.AddRange(match.Groups["Instructions"].Value.Split(",").Select(int.Parse));
            }
        }

        return new Computer(registerA, registerB, registerC, program.ToArray());
    }

    private record struct Computer(int RegisterA, int RegisterB, int RegisterC, int[] Program);

    [GeneratedRegex(@"Register (?<Register>A|B|C): (?<Value>\d+)")]
    private static partial Regex RegisterRegex();

    [GeneratedRegex("Program: (?<Instructions>.*)")]
    private static partial Regex ProgramRegex();
}