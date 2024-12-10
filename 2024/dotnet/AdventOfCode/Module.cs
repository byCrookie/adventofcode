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
    }
}