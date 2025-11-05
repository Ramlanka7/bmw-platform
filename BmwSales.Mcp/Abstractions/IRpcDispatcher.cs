namespace BmwSales.Mcp.Abstractions
{
    public interface IRpcDispatcher
    {
        Task<string> HandleAsync(string rawJson, CancellationToken ct);
    }
}
