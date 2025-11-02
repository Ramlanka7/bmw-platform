using BmwSalesPlatform.Contracts;

namespace BmwSalesPlatform.Data.Interfaces
{
    public interface ISalesRepository
    {
        Task<IEnumerable<YearTotalDto>> GetYearTotalsAsync(CancellationToken cancellationToken);
        Task<(IEnumerable<TopModelDto> Items, int Total)> GetTopModelsAsync(short? year, int page, int pageSize, CancellationToken cancellationToken);
        Task<IEnumerable<RegionalMixDto>> GetRegionalMixAsync(short? year, CancellationToken cancellationToken);
    }
}
