using AdventOfCode.Days;
using AdventOfCode.Measure;

namespace AdventOfCode.Day23;

[Day(23)]
[Part(1)]
public class Part1 : IPart
{
    public Task<PartResult> RunAsync(IMeasure measure, string input)
    {
        var edges = input.Split(Environment.NewLine)
            .Select(l => l.Split("-"))
            .SelectMany(p => new List<Edge> { new(p[0], p[1]), new(p[1], p[0]) })
            .ToList();

        var nodes = edges.Select(e => e.From).Union(edges.Select(e => e.To)).ToHashSet();
        var graph = nodes.ToDictionary(n => n, n => edges.Where(e => e.From == n).Select(e => e.To).ToList());

        var groups = new List<List<string>>();
        foreach (var node in nodes)
        {
            Groups(3, [node], graph[node], graph, groups);
        }
        
        var distinctGroups = groups.Select(g => string.Join("-", g.OrderBy(n => n))).Distinct().ToList();
        var result = distinctGroups.Count;
        return Task.FromResult(new PartResult($"{result}", $"Groups of 3 with at least one t: {result}"));
    }
    
    private static void Groups(int groupSize, List<string> nodes, List<string> connectedNodes, Dictionary<string, List<string>> graph, List<List<string>> groups)
    {
        var linkBackNodes = connectedNodes.Where(connectedNode => nodes.All(n => graph[connectedNode].Contains(n))).ToList();
        
        if (nodes.Count == groupSize)
        {
            if (nodes.Any(n => n.StartsWith('t')))
            {
                groups.Add(nodes);
            }
            
            return;
        }
        
        if (linkBackNodes.Count == 0)
        {
            return;
        }
        
        foreach (var connectedNode in linkBackNodes)
        {
            Groups(groupSize, nodes.Append(connectedNode).ToList(), graph[connectedNode], graph, groups);
        }
    }

    private record struct Edge(string From, string To);
}