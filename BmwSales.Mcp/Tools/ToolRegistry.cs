using BmwSales.Mcp.Abstractions;

namespace BmwSales.Mcp.Tools;

public sealed class ToolRegistry : IToolRegistry
{
    private readonly IReadOnlyDictionary<string, ITool> _map;

    public ToolRegistry(IEnumerable<ITool> tools)
        => _map = tools.ToDictionary(t => t.Name, StringComparer.OrdinalIgnoreCase);

    public IReadOnlyCollection<ITool> GetAll() => _map.Values.ToArray();

    public ITool? GetByName(string name) => _map.TryGetValue(name, out var t) ? t : null;
}
