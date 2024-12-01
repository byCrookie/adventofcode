using System.CommandLine;
using System.Diagnostics;
using Humanizer;

namespace AdventOfCode.Measure;

public class Measure(IConsole console) : IMeasure
{
    private readonly Stopwatch _measure = new();

    public void Start()
    {
        _measure.Start();
    }

    public void Stop()
    {
        _measure.Stop();
    }

    public void Restart()
    {
        _measure.Restart();
    }

    public void Now(string point)
    {
        _measure.Stop();
        console.WriteLine($"{point} - {_measure.Elapsed.Humanize()}");
        _measure.Start();
    }
}