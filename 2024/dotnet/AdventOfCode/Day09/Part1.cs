using System.Diagnostics;
using System.Text;
using AdventOfCode.Days;
using AdventOfCode.Measure;

namespace AdventOfCode.Day09;

[Day(9)]
[Part(1)]
public class Part1 : IPart
{
    public Task<PartResult> RunAsync(IMeasure measure, string input)
    {
        var numbers = input.ToArray().Select(c => $"{c}").Select(int.Parse).ToArray();
        var diskSize = numbers.Sum();
        var diskSpace = new int?[diskSize];

        var isFile = true;
        var fileId = 0;
        var index = 0;
        foreach (var number in numbers)
        {
            if (isFile)
            {
                for (var i = index; i < index + number; i++)
                {
                    diskSpace[i] = fileId;
                }
                
                fileId++;
            }

            isFile = !isFile;
            index += number;
        }

        Console.WriteLine($"Disk size: {diskSize}");
        // Console.WriteLine($"Disk space: {DiskSpaceToString(diskSpace)}");

        var indexToSwap = NextLeftToSwap(diskSpace, diskSize - 1);
        for (var i = 0; i < diskSize && i < indexToSwap; i++)
        {
            if (!diskSpace[i].HasValue)
            {
                diskSpace[i] = diskSpace[indexToSwap];
                diskSpace[indexToSwap] = null;
                indexToSwap = NextLeftToSwap(diskSpace, indexToSwap - 1);
                // Console.WriteLine($"Disk space: {DiskSpaceToString(diskSpace)}");
            }
        }
        
        long checksum = 0;
        for (var i = 0; i < diskSize; i++)
        {
            if (diskSpace[i].HasValue)
            {
                checksum += i * diskSpace[i]!.Value;
            }
        }
        
        return Task.FromResult(new PartResult($"{checksum}", $"Checksum: {checksum}"));
    }

    private static int NextLeftToSwap(int?[] diskSpace, int index)
    {
        for (var i = index; i >= 0; i--)
        {
            if (!diskSpace[i].HasValue) continue;
            return i;
        }

        throw new UnreachableException();
    }

    private static string DiskSpaceToString(int?[] diskSpace)
    {
        var sb = new StringBuilder();
        foreach (var block in diskSpace)
        {
            sb.Append(block.HasValue ? block.Value.ToString() : ".");
        }

        return sb.ToString();
    }
}