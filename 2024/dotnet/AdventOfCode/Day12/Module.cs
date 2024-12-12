using AdventOfCode.Days;
using Microsoft.Extensions.DependencyInjection;

namespace AdventOfCode.Day12;

public static class Module
{
    public static void AddDay12(this IServiceCollection services)
    {
        services.AddTransient<IPart, Part1>();
        services.AddTransient<IPart, Part2>();
    }
}