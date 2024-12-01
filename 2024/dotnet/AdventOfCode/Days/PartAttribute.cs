namespace AdventOfCode.Days;

[AttributeUsage(AttributeTargets.Class)]
public class PartAttribute(int part) : Attribute
{
    public int Part { get; } = part;
}