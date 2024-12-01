using Microsoft.Extensions.DependencyInjection;

namespace AdventOfCode.Measure;

public static class MeasureModule
{
    public static void AddMeasure(this IServiceCollection services)
    {
        services.AddTransient<IMeasure, Measure>();
    }
}