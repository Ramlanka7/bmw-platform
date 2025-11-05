namespace BmwSales.Mcp.Data;

public interface ISalesRepository
{
    Task<IEnumerable<YearTotalRow>> GetYearTotalsAsync(CancellationToken ct);
    Task<(IEnumerable<TopModelRow> Items, int Total)> GetTopModelsAsync(int? year, int page, int pageSize, CancellationToken ct);
    Task<IEnumerable<RegionalMixRow>> GetRegionalMixAsync(int? year, CancellationToken ct);
}

public record YearTotalRow(short Year, int Units);
public record TopModelRow(short Year, string Model, string? Series, int Units);
public record RegionalMixRow(short Year, string Region, int Units);
