using System.CommandLine;
using System.CommandLine.IO;
using System.Reflection;
using AdventOfCode;
using AdventOfCode.Days;
using AdventOfCode.Measure;
using Microsoft.Extensions.DependencyInjection;

var dayOption = new Option<int>(["--day", "-d"], "The day to run");
dayOption.FromAmong(Enumerable.Range(1, 24).Select(i => $"{i}").ToArray());
var partOption = new Option<int>(["--part", "-p"], "The part to run");
partOption.FromAmong(Enumerable.Range(1, 2).Select(i => $"{i}").ToArray());
var sampleOption = new Option<bool>(["--sample", "-s"], "Run the sample input");

var rootCommand = new RootCommand
{
    dayOption,
    partOption,
    sampleOption
};

rootCommand.SetHandler(async ctx =>
{
    var dayToRun = ctx.ParseResult.GetValueForOption(dayOption);
    var partToRun = ctx.ParseResult.GetValueForOption(partOption);
    var shouldRunSample = ctx.ParseResult.GetValueForOption(sampleOption);

    var services = new ServiceCollection();
    services.AddAdventOfCode();
    services.AddSingleton(ctx.Console);
    var serviceProvider = services.BuildServiceProvider();

    var measure = serviceProvider.GetRequiredService<IMeasure>();
    var parts = serviceProvider.GetServices<IPart>().Select(p => new
    {
        Part = p,
        DayAttribute = p.GetType().GetCustomAttribute<DayAttribute>(),
        PartAttribute = p.GetType().GetCustomAttribute<PartAttribute>()
    }).ToList();

    if (parts.Any(p => p.DayAttribute is null || p.PartAttribute is null))
    {
        ctx.Console.Error.WriteLine("Not all parts have Day or Part attribute");
        ctx.ExitCode = 1;
        return;
    }

    var matchedParts = parts
        .Where(p => p.DayAttribute!.Day == dayToRun && p.PartAttribute!.Part == partToRun)
        .Select(p => p.Part)
        .ToList();

    switch (matchedParts.Count)
    {
        case 0:
            ctx.Console.Error.WriteLine("Part not found");
            ctx.ExitCode = 1;
            return;
        case > 1:
            ctx.Console.Error.WriteLine("Multiple parts found");
            ctx.ExitCode = 1;
            return;
    }

    var part = matchedParts.First();
    const string data = "data";
    var dayId = $"day{dayToRun.ToString().PadLeft(2, '0')}";
    var partId = $"part{partToRun.ToString().PadLeft(2, '0')}";

    var inputPath = shouldRunSample
        ? Path.Combine(data, dayId, partId, "sample.input.txt")
        : Path.Combine(data, dayId, partId, "input.txt");

    var outputPath = shouldRunSample
        ? Path.Combine(data, dayId, partId, "sample.output.txt")
        : Path.Combine(data, dayId, partId, "output.txt");

    var input = await File.ReadAllTextAsync(inputPath);
    var output = await File.ReadAllTextAsync(outputPath);

    ctx.Console.WriteLine(shouldRunSample
        ? $"Running sample of day {dayToRun} part {partToRun}"
        : $"Running day {dayToRun} part {partToRun}");

    measure.Start();
    var result = await part.RunAsync(measure, input);
    measure.End();

    if (!string.IsNullOrWhiteSpace(result.Message) && result.Value != output)
    {
        ctx.Console.WriteLine(result.ToString());
        ctx.Console.Error.WriteLine($"Value '{result.Value}' does not match value '{output}'");
        ctx.ExitCode = 1;
        return;
    }

    ctx.Console.WriteLine(result.Message);
    ctx.ExitCode = 0;
});

return await rootCommand.InvokeAsync(args);