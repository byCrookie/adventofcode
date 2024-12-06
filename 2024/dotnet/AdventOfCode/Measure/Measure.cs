using System.CommandLine;
using System.Diagnostics;
using Humanizer;

namespace AdventOfCode.Measure;

public class Measure(IConsole console) : IMeasure
{
    private readonly Stopwatch _measure = new();

    public void Start()
    {
        console.WriteLine("Start");
        _measure.Start();
    }

    public void Now(string point)
    {
        _measure.Stop();
        console.WriteLine($"{point} - {_measure.Elapsed.Humanize(2)}");
        _measure.Start();
    }

    public void End()
    {
        _measure.Stop();
        console.WriteLine($"End - {_measure.Elapsed.Humanize(2)}");
    }
}