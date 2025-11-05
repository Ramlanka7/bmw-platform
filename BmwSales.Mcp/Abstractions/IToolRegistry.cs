namespace BmwSales.Mcp.Abstractions;

public interface IToolRegistry
{
    IReadOnlyCollection<ITool> GetAll();
    ITool? GetByName(string name);
}
