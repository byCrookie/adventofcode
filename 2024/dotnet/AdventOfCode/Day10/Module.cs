using AdventOfCode.Days;
using Microsoft.Extensions.DependencyInjection;

namespace AdventOfCode.Day10;

public static class Module
{
    public static void AddDay10(this IServiceCollection services)
    {
        services.AddTransient<IPart, Part1>();
        services.AddTransient<IPart, Part2>();
    }
}