using System.Text.Json.Nodes;
using BmwSales.Mcp.Abstractions;
using BmwSales.Mcp.Data;

namespace BmwSales.Mcp.Tools;

public sealed class GetTopModelsTool : ITool
{
    public string Name => "getTopModels";
    public string Description => "Top models with optional year, page, pageSize.";
    public JsonNode? InputSchema => JsonNode.Parse("""
    {
      "type":"object",
      "properties":{
        "year":{"type":"integer","nullable":true},
        "page":{"type":"integer","minimum":1,"default":1},
        "pageSize":{"type":"integer","minimum":1,"maximum":100,"default":10}
      },
      "required":["page","pageSize"],
      "additionalProperties":false
    }
    """);

    private readonly ISalesRepository _repo;
    public GetTopModelsTool(ISalesRepository repo) => _repo = repo;

    public async Task<object> CallAsync(JsonNode? arguments, CancellationToken ct)
    {
        int? year = arguments?["year"]?.GetValue<int?>();
        int page = arguments?["page"]?.GetValue<int?>() ?? 1;
        int pageSize = arguments?["pageSize"]?.GetValue<int?>() ?? 10;

        var (items, total) = await _repo.GetTopModelsAsync(year, page, pageSize, ct);
        return new { items, total, page, pageSize };
    }
}
