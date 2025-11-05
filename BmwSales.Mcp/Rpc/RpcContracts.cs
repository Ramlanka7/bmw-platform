using System.Text.Json;
using System.Text.Json.Serialization;

namespace BmwSales.Mcp.Rpc;

// JSON-RPC 2.0
public record RpcRequest(
    [property: JsonPropertyName("jsonrpc")] string Jsonrpc,
    [property: JsonPropertyName("id")] string? Id,
    [property: JsonPropertyName("method")] string Method,
    [property: JsonPropertyName("params")] JsonElement? Params);

public record RpcError(int Code, string Message, object? Data = null);

public record RpcResponse<T>(
    [property: JsonPropertyName("jsonrpc")] string Jsonrpc,
    [property: JsonPropertyName("id")] string? Id,
    [property: JsonPropertyName("result")] T? Result,
    [property: JsonPropertyName("error")] RpcError? Error);

public record ToolDef(string Name, string Description, object? InputSchema);
public record ToolsListResult(ToolDef[] Tools);
public record CallToolParams(string Name, JsonElement? Arguments);

public static class JsonOpts
{
    public static readonly JsonSerializerOptions Default = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };
}
