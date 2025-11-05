namespace BmwSales.Mcp.Abstractions;

public interface IRpcTransport : IAsyncDisposable
{
    IAsyncEnumerable<string> ReadLinesAsync(CancellationToken ct);
    Task WriteLineAsync(string line, CancellationToken ct);
}
