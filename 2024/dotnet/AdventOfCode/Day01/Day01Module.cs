using AdventOfCode.Days;
using Microsoft.Extensions.DependencyInjection;

namespace AdventOfCode.Day01;

public static class Day01Module
{
    public static void AddDay01(this IServiceCollection services)
    {
        services.AddTransient<IPart, Part1>();
        services.AddTransient<IPart, Part2>();
    }
}