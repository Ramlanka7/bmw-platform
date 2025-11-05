using BmwSales.Mcp.Abstractions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BmwSales.Mcp.Hosting;

public sealed class McpWorker : BackgroundService
{
    private readonly ILogger<McpWorker> _logger;
    private readonly IRpcTransport _transport;
    private readonly IRpcDispatcher _dispatcher;

    public McpWorker(ILogger<McpWorker> logger, IRpcTransport transport, IRpcDispatcher dispatcher)
    {
        _logger = logger;
        _transport = transport;
        _dispatcher = dispatcher;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("MCP server started (stdio).");

        await foreach (var line in _transport.ReadLinesAsync(stoppingToken))
        {
            try
            {
                var response = await _dispatcher.HandleAsync(line, stoppingToken);
                await _transport.WriteLineAsync(response, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process message.");
            }
        }

        _logger.LogInformation("MCP server stopped.");
    }
}
