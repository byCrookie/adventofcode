﻿using System.Diagnostics;
using System.Text;
using AdventOfCode.Days;
using AdventOfCode.Measure;

namespace AdventOfCode.Day09;

[Day(9)]
[Part(2)]
public class Part2 : IPart
{
    public Task<PartResult> RunAsync(IMeasure measure, string input)
    {
        var numbers = input.ToArray().Select(c => $"{c}").Select(int.Parse).ToArray();
        var diskSize = numbers.Length;
        var diskSpace = new List<Block>(diskSize);

        var isFile = true;
        var fileId = 0;
        foreach (var number in numbers)
        {
            if (isFile)
            {
                diskSpace.Add(new Block(fileId, number));
                fileId++;
            }
            else
            {
                diskSpace.Add(new Block(null, number));
            }

            isFile = !isFile;
        }

        Console.WriteLine($"Blocks: {diskSpace.Count}");
        Console.WriteLine($"Disk size: {diskSpace.Sum(b => b.Size)}");
        Console.WriteLine($"Disk space: {DiskSpaceToString(diskSpace)}");

        var blocks = diskSpace
            .Select((block, index) => (index, block))
            .Where(b => b.block.Id.HasValue)
            .Reverse()
            .ToList();

        var added = 0;
        foreach (var (blockIndex, block) in blocks)
        {
            foreach (var (freeBlockIndex, freeBlock) in diskSpace
                         .Select((freeBlock, index) => (index, freeBlock))
                         .Where(b => !b.freeBlock.Id.HasValue)
                         .Where(b => b.index < blockIndex + added)
                         .ToList())
            {
                if (block.Size == freeBlock.Size)
                {
                    Console.WriteLine(
                        $"Block ({blockIndex} + {added} = {blockIndex + added}): {block} Free block ({freeBlockIndex}): {freeBlock}");
                    diskSpace[freeBlockIndex] = block;
                    diskSpace[blockIndex + added] = freeBlock;
                    Console.WriteLine($"Disk space: {DiskSpaceToString(diskSpace)}");
                    break;
                }

                if (block.Size < freeBlock.Size)
                {
                    Console.WriteLine(
                        $"Block ({blockIndex} + {added} = {blockIndex + added}): {block} Free block ({freeBlockIndex}): {freeBlock}");
                    diskSpace[freeBlockIndex] = new Block(null, freeBlock.Size - block.Size);
                    diskSpace[blockIndex + added] = block with { Id = null };
                    diskSpace.Insert(freeBlockIndex, block);
                    added++;
                    Console.WriteLine($"Disk space: {DiskSpaceToString(diskSpace)}");
                    break;
                }

                Console.WriteLine(
                    $"Block ({blockIndex} + {added} = {blockIndex + added}): {block} Free block ({freeBlockIndex}): {freeBlock}");
            }
        }

        Console.WriteLine($"Blocks: {diskSpace.Count}");
        Console.WriteLine($"Disk size: {diskSpace.Sum(b => b.Size)}");

        var checksum = Checksum(diskSpace);
        return Task.FromResult(new PartResult($"{checksum}", $"Checksum: {checksum}"));
    }

    private static long Checksum(List<Block> diskSpace)
    {
        long checksum = 0;
        var index = 0;
        foreach (var block in diskSpace)
        {
            if (block.Id.HasValue)
            {
                for (var i = 0; i < block.Size; i++)
                {
                    checksum += index * block.Id.Value;
                    index++;
                }
            }
            else
            {
                index += block.Size;
            }
        }

        return checksum;
    }

    private static string DiskSpaceToString(List<Block> diskSpace)
    {
        var sb = new StringBuilder();
        foreach (var block in diskSpace)
        {
            sb.Append(block.Id.HasValue ? new string($"{block.Id}"[0], block.Size) : new string('.', block.Size));
        }
        
        sb.AppendLine();

        foreach (var (index, block) in diskSpace.Select((b, index) => (index, b)))
        {
            sb.Append(block.Id.HasValue
                ? $"({index}){new string($"{block.Id}"[0], block.Size)}"
                : $"({index}){new string('.', block.Size)}");
        }

        return sb.ToString();
    }

    private record struct Block(int? Id, int Size);
}