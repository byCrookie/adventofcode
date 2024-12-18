﻿using AdventOfCode.Days;
using Microsoft.Extensions.DependencyInjection;

namespace AdventOfCode.Day03;

public static class Module
{
    public static void AddDay03(this IServiceCollection services)
    {
        services.AddTransient<IPart, Day04.Part1>();
        services.AddTransient<IPart, Day04.Part2>();
    }
}