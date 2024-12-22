using AdventOfCode.Day01;
using AdventOfCode.Day02;
using AdventOfCode.Day03;
using AdventOfCode.Day04;
using AdventOfCode.Day05;
using AdventOfCode.Day06;
using AdventOfCode.Day07;
using AdventOfCode.Day08;
using AdventOfCode.Day09;
using AdventOfCode.Day10;
using AdventOfCode.Day11;
using AdventOfCode.Day12;
using AdventOfCode.Day13;
using AdventOfCode.Day14;
using AdventOfCode.Day15;
using AdventOfCode.Day16;
using AdventOfCode.Day17;
using AdventOfCode.Day18;
using AdventOfCode.Day19;
using AdventOfCode.Day20;
using AdventOfCode.Day21;
using AdventOfCode.Day22;
using AdventOfCode.Measure;
using Microsoft.Extensions.DependencyInjection;

namespace AdventOfCode;

public static class Module
{
    public static void AddAdventOfCode(this IServiceCollection services)
    {
        services.AddMeasure();
        
        services.AddDay01();
        services.AddDay02();
        services.AddDay03();
        services.AddDay04();
        services.AddDay05();
        services.AddDay06();
        services.AddDay07();
        services.AddDay08();
        services.AddDay09();
        services.AddDay10();
        services.AddDay11();
        services.AddDay12();
        services.AddDay13();
        services.AddDay14();
        services.AddDay15();
        services.AddDay16();
        services.AddDay17();
        services.AddDay18();
        services.AddDay19();
        services.AddDay20();
        services.AddDay21();
        services.AddDay22();
    }
}