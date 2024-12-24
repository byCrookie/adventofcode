using AdventOfCode.Days;
using AdventOfCode.Measure;

namespace AdventOfCode.Day23;

[Day(23)]
[Part(2)]
public class Part2 : IPart
{
    public Task<PartResult> RunAsync(IMeasure measure, string input)
    {
        var edges = input.Split(Environment.NewLine)
            .Select(l => l.Split("-"))
            .SelectMany(p => new List<Edge> { new(p[0], p[1]), new(p[1], p[0]) })
            .ToList();

        var nodes = edges.Select(e => e.From).Union(edges.Select(e => e.To)).ToHashSet();
        var graph = nodes.ToDictionary(n => n,
            n => new HashSet<string>(edges.Where(e => e.From == n).Select(e => e.To)));

        var largestGroup = BronKerboschMaxClique(graph);

        var result = string.Join(",", largestGroup.OrderBy(n => n));
        return Task.FromResult(new PartResult($"{result}", $"Password: {result}"));
    }

    // https://en.wikipedia.org/wiki/Bron%E2%80%93Kerbosch_algorithm
    private static HashSet<string> BronKerboschMaxClique(Dictionary<string, HashSet<string>> graph)
    {
        var maxClique = new HashSet<string>();
        var potential = new HashSet<string>(graph.Keys);
        var excluded = new HashSet<string>();

        BronKerbosch([], potential, excluded, ref maxClique, graph);
        return maxClique ?? [];
    }

    private static void BronKerbosch(HashSet<string> clique, HashSet<string> candidates, HashSet<string> skip,
        ref HashSet<string>? maxClique, Dictionary<string, HashSet<string>> graph)
    {
        if (candidates.Count == 0 && skip.Count == 0)
        {
            if (clique.Count > maxClique?.Count)
            {
                maxClique = [..clique];
            }

            return;
        }
        
        var pivot = candidates.Union(skip)
            .MaxBy(v => candidates.Count(c => graph[v].Contains(c))) ?? candidates.First();
        
        var pivotNonNeighbors = candidates.Where(v => !graph[pivot].Contains(v));
        foreach (var vertex in pivotNonNeighbors)
        {
            var vertexNeighbors = graph[vertex];
            clique.Add(vertex);
            
            BronKerbosch(
                clique,
                candidates.Intersect(vertexNeighbors).ToHashSet(),
                skip.Intersect(vertexNeighbors).ToHashSet(), ref maxClique, graph);

            clique.Remove(vertex);
            candidates.Remove(vertex);
            skip.Add(vertex);
        }
    }

    private record struct Edge(string From, string To);
}