using BmwSalesPlatform.Contracts;
using BmwSalesPlatform.Data.Interfaces;
using BmwSalesPlatform.Services.Interfaces;

namespace BmwSales.Api.Services;
public class SalesService : ISalesService
{
    private readonly ISalesRepository _repo;
    public SalesService(ISalesRepository repo) => _repo = repo;

    public Task<IEnumerable<YearTotalDto>> GetYearTotalsAsync(CancellationToken ct)
        => _repo.GetYearTotalsAsync(ct);

    public async Task<PagedResult<TopModelDto>> GetTopModelsAsync(short? year, int page, int pageSize, CancellationToken ct)
    {
        if (page <= 0) page = 1;
        if (pageSize <= 0) pageSize = 10; else pageSize = Math.Min(pageSize, 100);

        var (items, total) = await _repo.GetTopModelsAsync(year, page, pageSize, ct);
        return new PagedResult<TopModelDto>(items, total, page, pageSize);
    }

    public Task<IEnumerable<RegionalMixDto>> GetRegionalMixAsync(short? year, CancellationToken ct)
        => _repo.GetRegionalMixAsync(year, ct);
}
