using System.Text.Json.Nodes;
using BmwSales.Mcp.Abstractions;
using BmwSales.Mcp.Data;

namespace BmwSales.Mcp.Tools;

public sealed class GetRegionalMixTool : ITool
{
    public string Name => "getRegionalMix";
    public string Description => "Regional sales distribution with optional year.";
    public JsonNode? InputSchema => JsonNode.Parse("""
    { "type":"object","properties":{"year":{"type":"integer","nullable":true}},"additionalProperties":false }
    """);

    private readonly ISalesRepository _repo;
    public GetRegionalMixTool(ISalesRepository repo) => _repo = repo;

    public async Task<object> CallAsync(JsonNode? arguments, CancellationToken ct)
    {
        int? year = arguments?["year"]?.GetValue<int?>();
        var items = await _repo.GetRegionalMixAsync(year, ct);
        return new { items };
    }
}
