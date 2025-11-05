using Microsoft.Data.SqlClient;

namespace BmwSales.Mcp.Abstractions;

public interface ISqlConnectionFactory
{
    SqlConnection Create();
}
