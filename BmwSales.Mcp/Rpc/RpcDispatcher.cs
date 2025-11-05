using System.Text.Json;
using System.Text.Json.Nodes;
using BmwSales.Mcp.Abstractions;
using Microsoft.Extensions.Configuration;

namespace BmwSales.Mcp.Rpc;

public sealed class RpcDispatcher : IRpcDispatcher
{
    private readonly IToolRegistry _tools;
    private readonly IConfiguration _cfg;

    public RpcDispatcher(IToolRegistry tools, IConfiguration cfg)
    {
        _tools = tools;
        _cfg = cfg;
    }

    public Task<string> HandleAsync(string rawJson, CancellationToken ct)
    {
        RpcRequest? req;
        try
        {
            req = JsonSerializer.Deserialize<RpcRequest>(rawJson, JsonOpts.Default);
            if (req is null || req.Jsonrpc != "2.0")
                return Task.FromResult(SerializeError(null, -32600, "Invalid Request"));
        }
        catch (Exception ex)
        {
            return Task.FromResult(SerializeError(null, -32700, "Parse error", new { ex.Message }));
        }

        return req.Method switch
        {
            "initialize" => Task.FromResult(HandleInitialize(req)),
            "tools/list" => Task.FromResult(HandleToolsList(req)),
            "tools/call" => HandleToolsCallAsync(req, ct),
            _ => Task.FromResult(SerializeError(req.Id, -32601, $"Unknown method {req.Method}"))
        };
    }

    private string HandleInitialize(RpcRequest req)
    {
        var result = new
        {
            protocolVersion = _cfg["Mcp:ProtocolVersion"] ?? "2024-11-01",
            serverInfo = new
            {
                name = _cfg["Mcp:ServerName"] ?? "BmwSales.Mcp",
                version = _cfg["Mcp:ServerVersion"] ?? "1.0.0"
            },
            capabilities = new { tools = new { } }
        };
        return SerializeOk(req.Id, result);
    }

    private string HandleToolsList(RpcRequest req)
    {
        var defs = _tools.GetAll()
            .Select(t => new ToolDef(t.Name, t.Description, t.InputSchema))
            .ToArray();
        return SerializeOk(req.Id, new ToolsListResult(defs));
    }

    private async Task<string> HandleToolsCallAsync(RpcRequest req, CancellationToken ct)
    {
        try
        {
            // Validate and deserialize params safely
            if (req.Params is null || !req.Params.HasValue)
                return SerializeError(req.Id, -32602, "Invalid params");

            CallToolParams? call;
            try
            {
                call = JsonSerializer.Deserialize<CallToolParams>(req.Params.Value.GetRawText(), JsonOpts.Default);
            }
            catch (JsonException ex)
            {
                return SerializeError(req.Id, -32602, "Invalid params", new { ex.Message });
            }

            if (call is null) return SerializeError(req.Id, -32602, "Invalid params");

            var tool = _tools.GetByName(call.Name);
            if (tool is null) return SerializeError(req.Id, -32601, $"Unknown tool {call.Name}");

            JsonNode? args = null;
            if (call.Arguments is JsonElement a && a.ValueKind != JsonValueKind.Null)
            {
                try
                {
                    // Convert JsonElement -> JsonNode for ergonomic access
                    args = JsonNode.Parse(a.GetRawText());
                }
                catch (JsonException ex)
                {
                    return SerializeError(req.Id, -32602, "Invalid arguments", new { ex.Message });
                }
            }

            var payload = await tool.CallAsync(args, ct);

            // Ensure the payload is converted to a JSON representation before embedding in the response.
            // This avoids runtime serialization errors when payload contains types that the runtime cannot
            // directly serialize as part of the anonymous envelope.
            JsonElement payloadElement;
            try
            {
                // Use SerializeToElement to produce a JSON element representation of the payload.
                payloadElement = JsonSerializer.SerializeToElement(payload, payload?.GetType() ?? typeof(object), JsonOpts.Default);
            }
            catch (Exception ex)
            {
                return SerializeError(req.Id, -32000, "Tool returned non-serializable payload", new { ex.Message });
            }

            var envelope = new { content = payloadElement };
            return SerializeOk(req.Id, envelope);
        }
        catch (Exception ex)
        {
            return SerializeError(req.Id, -32000, "Tool error", new { ex.Message });
        }
    }

    private static string SerializeOk<T>(string? id, T result)
        => JsonSerializer.Serialize(new RpcResponse<T>("2.0", id, result, null), JsonOpts.Default);

    private static string SerializeError(string? id, int code, string message, object? data = null)
        => JsonSerializer.Serialize(new RpcResponse<object>("2.0", id, null, new RpcError(code, message, data)), JsonOpts.Default);
}