using AdventOfCode.Days;
using Microsoft.Extensions.DependencyInjection;

namespace AdventOfCode.Day06;

public static class Module
{
    public static void AddDay06(this IServiceCollection services)
    {
        services.AddTransient<IPart, Part1>();
        services.AddTransient<IPart, Part2>();
    }
}