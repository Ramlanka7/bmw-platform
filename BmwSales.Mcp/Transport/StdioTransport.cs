using System.Runtime.CompilerServices;
using BmwSales.Mcp.Abstractions;

namespace BmwSales.Mcp.Transport;

public sealed class StdioTransport : IRpcTransport
{
    public async IAsyncEnumerable<string> ReadLinesAsync([EnumeratorCancellation] CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            var line = await Console.In.ReadLineAsync();
            if (line is null) yield break;
            if (string.IsNullOrWhiteSpace(line)) continue;
            yield return line;
        }
    }

    public Task WriteLineAsync(string line, CancellationToken ct)
        => Console.Out.WriteLineAsync(line);

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;
}
