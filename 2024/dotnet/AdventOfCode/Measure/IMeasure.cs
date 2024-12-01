namespace AdventOfCode.Measure;

public interface IMeasure
{
    public void Start();
    public void Restart();
    public void Stop();

    public void Now(string point);
}