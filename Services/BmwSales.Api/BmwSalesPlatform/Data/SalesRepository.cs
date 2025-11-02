// Data/SalesRepository.cs
using BmwSalesPlatform.Contracts;
using BmwSalesPlatform.Data.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;

namespace BmwSales.Api.Data;
public class SalesRepository : ISalesRepository
{
    private readonly ISqlConnectionFactory _factory;
    public SalesRepository(ISqlConnectionFactory factory) => _factory = factory;

    public async Task<IEnumerable<YearTotalDto>> GetYearTotalsAsync(CancellationToken ct)
    {
        using IDbConnection conn = _factory.Create();
        var sqlConn = (SqlConnection)conn;
        await sqlConn.OpenAsync(ct);
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "usp_Sales_GetYearTotals";
        cmd.CommandType = CommandType.StoredProcedure;

        using var rdr = await ((System.Data.Common.DbCommand)cmd).ExecuteReaderAsync(ct);
        var list = new List<YearTotalDto>();
        while (await rdr.ReadAsync(ct))
            list.Add(new YearTotalDto(
                Year: rdr.GetInt16(rdr.GetOrdinal("Year")),
                Units: rdr.GetInt32(rdr.GetOrdinal("Units"))
            ));
        return list;
    }

    public async Task<(IEnumerable<TopModelDto> Items, int Total)> GetTopModelsAsync(short? year, int page, int pageSize, CancellationToken ct)
    {
        using var conn = _factory.Create();
        var sqlConn = (SqlConnection)conn;
        await sqlConn.OpenAsync(ct);
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "usp_Sales_GetTopModels";
        cmd.CommandType = CommandType.StoredProcedure;

        var pYear = cmd.CreateParameter(); pYear.ParameterName = "@Year"; pYear.Value = (object?)year ?? DBNull.Value; cmd.Parameters.Add(pYear);
        var pPage = cmd.CreateParameter(); pPage.ParameterName = "@Page"; pPage.Value = page; cmd.Parameters.Add(pPage);
        var pSize = cmd.CreateParameter(); pSize.ParameterName = "@PageSize"; pSize.Value = pageSize; cmd.Parameters.Add(pSize);

        using var rdr = await ((System.Data.Common.DbCommand)cmd).ExecuteReaderAsync(ct);

        // First result: Total
        int total = 0;
        if (await rdr.ReadAsync(ct))
            total = rdr.GetInt32(rdr.GetOrdinal("Total"));

        // Next result: Page items
        await rdr.NextResultAsync(ct);
        var items = new List<TopModelDto>();
        while (await rdr.ReadAsync(ct))
            items.Add(new TopModelDto(
                Year: rdr.GetInt16(rdr.GetOrdinal("Year")),
                Model: rdr.GetString(rdr.GetOrdinal("Model")),
                Series: rdr.IsDBNull(rdr.GetOrdinal("Series")) ? null : rdr.GetString(rdr.GetOrdinal("Series")),
                Units: rdr.GetInt32(rdr.GetOrdinal("Units"))
            ));

        return (items, total);
    }

    public async Task<IEnumerable<RegionalMixDto>> GetRegionalMixAsync(short? year, CancellationToken ct)
    {
        using var conn = _factory.Create();
        var sqlConn = (SqlConnection)conn;
        await sqlConn.OpenAsync(ct);
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "usp_Sales_GetRegionalMix";
        cmd.CommandType = CommandType.StoredProcedure;

        var pYear = cmd.CreateParameter(); pYear.ParameterName = "@Year"; pYear.Value = (object?)year ?? DBNull.Value; cmd.Parameters.Add(pYear);

        using var rdr = await ((System.Data.Common.DbCommand)cmd).ExecuteReaderAsync(ct);
        var list = new List<RegionalMixDto>();
        while (await rdr.ReadAsync(ct))
            list.Add(new RegionalMixDto(
                Year: rdr.GetInt16(rdr.GetOrdinal("Year")),
                Region: rdr.GetString(rdr.GetOrdinal("Region")),
                Units: rdr.GetInt32(rdr.GetOrdinal("Units"))
            ));
        return list;
    }
}
