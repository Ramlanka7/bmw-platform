using BmwSales.Mcp.Abstractions;
using Microsoft.Data.SqlClient;

namespace BmwSales.Mcp.Data;

public sealed class SqlConnectionFactory : ISqlConnectionFactory
{
    private readonly string _cs;
    public SqlConnectionFactory(string cs) => _cs = cs;
    public SqlConnection Create() => new(_cs);
}
