using System.Text.Json.Nodes;

namespace BmwSales.Mcp.Abstractions;

public interface ITool
{
    string Name { get; }
    string Description { get; }
    JsonNode? InputSchema { get; }
    Task<object> CallAsync(JsonNode? arguments, CancellationToken ct);
}
