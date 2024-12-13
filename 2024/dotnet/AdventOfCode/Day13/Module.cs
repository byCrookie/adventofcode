using AdventOfCode.Days;
using Microsoft.Extensions.DependencyInjection;

namespace AdventOfCode.Day13;

public static class Module
{
    public static void AddDay13(this IServiceCollection services)
    {
        services.AddTransient<IPart, Part1>();
        services.AddTransient<IPart, Part2>();
    }
}