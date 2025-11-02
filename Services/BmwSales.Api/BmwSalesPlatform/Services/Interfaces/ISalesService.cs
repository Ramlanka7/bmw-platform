using BmwSalesPlatform.Contracts;

namespace BmwSalesPlatform.Services.Interfaces
{
    public interface ISalesService
    {
        Task<IEnumerable<YearTotalDto>> GetYearTotalsAsync(CancellationToken cancellationToken);
        Task<PagedResult<TopModelDto>> GetTopModelsAsync(short? year, int page, int pageSize, CancellationToken cancellationToken);
        Task<IEnumerable<RegionalMixDto>> GetRegionalMixAsync(short? year, CancellationToken cancellationToken);
    }
}
