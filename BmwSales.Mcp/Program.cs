using BmwSales.Mcp.Abstractions;
using BmwSales.Mcp.Data;
using BmwSales.Mcp.Hosting;
using BmwSales.Mcp.Rpc;
using BmwSales.Mcp.Tools;
using BmwSales.Mcp.Transport;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(cfg =>
    {
        cfg.AddEnvironmentVariables();
    })
    .ConfigureLogging(lb => lb.AddConsole())
    .ConfigureServices((ctx, services) =>
    {
        //var asdf = Environment.GetEnvironmentVariable("BMWSALES_CS");
        //var cs = Environment.GetEnvironmentVariable("BMWSALES_CS")
        //         ?? ctx.Configuration.GetConnectionString("BmwSalesDw")
        //         ?? throw new InvalidOperationException("Connection string not found.");

        var cs = ctx.Configuration.GetConnectionString("BmwSalesDw");

        services.AddSingleton<ISqlConnectionFactory>(new SqlConnectionFactory(cs));

        // data
        services.AddScoped<ISalesRepository, SalesRepository>();

        // tools
        services.AddSingleton<ITool, GetYearTotalsTool>();
        services.AddSingleton<ITool, GetTopModelsTool>();
        services.AddSingleton<ITool, GetRegionalMixTool>();
        services.AddSingleton<IToolRegistry, ToolRegistry>();

        // rpc + transport
        services.AddSingleton<IRpcTransport, StdioTransport>();
        services.AddSingleton<IRpcDispatcher, RpcDispatcher>();

        // worker
        services.AddHostedService<McpWorker>();
    })
    .Build();

await host.RunAsync();
