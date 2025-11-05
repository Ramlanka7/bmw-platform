using System.Data;
using BmwSales.Mcp.Abstractions;
using Microsoft.Data.SqlClient;

namespace BmwSales.Mcp.Data;

public sealed class SalesRepository : ISalesRepository
{
    private readonly ISqlConnectionFactory _factory;
    public SalesRepository(ISqlConnectionFactory factory) => _factory = factory;

    public async Task<IEnumerable<YearTotalRow>> GetYearTotalsAsync(CancellationToken ct)
    {
        await using var conn = _factory.Create();
        await conn.OpenAsync(ct);

        await using var cmd = conn.CreateCommand();
        cmd.CommandText = "dbo.usp_Sales_GetYearTotals";
        cmd.CommandType = CommandType.StoredProcedure;

        var list = new List<YearTotalRow>();
        await using var rdr = await cmd.ExecuteReaderAsync(ct);
        while (await rdr.ReadAsync(ct))
        {
            list.Add(new YearTotalRow(
                rdr.GetInt16(rdr.GetOrdinal("Year")),
                rdr.GetInt32(rdr.GetOrdinal("Units"))
            ));
        }
        return list;
    }

    public async Task<(IEnumerable<TopModelRow> Items, int Total)> GetTopModelsAsync(int? year, int page, int pageSize, CancellationToken ct)
    {
        await using var conn = _factory.Create();
        await conn.OpenAsync(ct);

        await using var cmd = conn.CreateCommand();
        cmd.CommandText = "dbo.usp_Sales_GetTopModels";
        cmd.CommandType = CommandType.StoredProcedure;

        var pYear = cmd.CreateParameter(); pYear.ParameterName = "@Year"; pYear.Value = (object?)year ?? DBNull.Value; cmd.Parameters.Add(pYear);
        var pPage = cmd.CreateParameter(); pPage.ParameterName = "@Page"; pPage.Value = page; cmd.Parameters.Add(pPage);
        var pSize = cmd.CreateParameter(); pSize.ParameterName = "@PageSize"; pSize.Value = pageSize; cmd.Parameters.Add(pSize);

        int total = 0;
        await using (var rdr = await cmd.ExecuteReaderAsync(ct))
        {
            if (await rdr.ReadAsync(ct))
                total = rdr.GetInt32(rdr.GetOrdinal("Total"));

            await rdr.NextResultAsync(ct);

            var items = new List<TopModelRow>();
            while (await rdr.ReadAsync(ct))
            {
                items.Add(new TopModelRow(
                    rdr.GetInt16(rdr.GetOrdinal("Year")),
                    rdr.GetString(rdr.GetOrdinal("Model")),
                    rdr.IsDBNull(rdr.GetOrdinal("Series")) ? null : rdr.GetString(rdr.GetOrdinal("Series")),
                    rdr.GetInt32(rdr.GetOrdinal("Units"))
                ));
            }
            return (items, total);
        }
    }

    public async Task<IEnumerable<RegionalMixRow>> GetRegionalMixAsync(int? year, CancellationToken ct)
    {
        await using var conn = _factory.Create();
        await conn.OpenAsync(ct);

        await using var cmd = conn.CreateCommand();
        cmd.CommandText = "dbo.usp_Sales_GetRegionalMix";
        cmd.CommandType = CommandType.StoredProcedure;

        var pYear = cmd.CreateParameter(); pYear.ParameterName = "@Year"; pYear.Value = (object?)year ?? DBNull.Value; cmd.Parameters.Add(pYear);

        var list = new List<RegionalMixRow>();
        await using var rdr = await cmd.ExecuteReaderAsync(ct);
        while (await rdr.ReadAsync(ct))
        {
            list.Add(new RegionalMixRow(
                rdr.GetInt16(rdr.GetOrdinal("Year")),
                rdr.GetString(rdr.GetOrdinal("Region")),
                rdr.GetInt32(rdr.GetOrdinal("Units"))
            ));
        }
        return list;
    }
}
