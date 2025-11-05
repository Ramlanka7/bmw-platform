using System.Text.Json.Nodes;
using BmwSales.Mcp.Abstractions;
using BmwSales.Mcp.Data;

namespace BmwSales.Mcp.Tools;

public sealed class GetYearTotalsTool : ITool
{
    public string Name => "getYearTotals";
    public string Description => "Return total BMW sales per year.";
    public JsonNode? InputSchema => JsonNode.Parse("""{ "type":"object","properties":{},"additionalProperties":false }""");

    private readonly ISalesRepository _repo;
    public GetYearTotalsTool(ISalesRepository repo) => _repo = repo;

    public async Task<object> CallAsync(JsonNode? arguments, CancellationToken ct)
    {
        var rows = await _repo.GetYearTotalsAsync(ct);
        return new { items = rows };
    }
}
