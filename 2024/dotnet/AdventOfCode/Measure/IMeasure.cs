namespace AdventOfCode.Measure;

public interface IMeasure
{
    public void Start();
    public void Now(string point);
    public void End();
}