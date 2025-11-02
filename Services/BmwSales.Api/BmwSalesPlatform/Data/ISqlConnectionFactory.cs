// Data/ISqlConnectionFactory.cs
using Microsoft.Data.SqlClient;
using System.Data;

namespace BmwSales.Api.Data;
public interface ISqlConnectionFactory { IDbConnection Create(); }

public class SqlConnectionFactory : ISqlConnectionFactory
{
    private readonly string _cs;
    public SqlConnectionFactory(IConfiguration cfg) =>
        _cs = cfg.GetConnectionString("BmwSalesDw")!;
    public IDbConnection Create() => new SqlConnection(_cs);
}
