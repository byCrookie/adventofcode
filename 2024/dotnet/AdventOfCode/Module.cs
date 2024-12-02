using AdventOfCode.Day01;
using AdventOfCode.Day02;
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
    }
}