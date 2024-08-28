using CSVParser;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;


using var loggerFactory = LoggerFactory.Create(builder =>
{
    builder
        .AddFilter("Microsoft", LogLevel.Warning)
        .AddFilter("System", LogLevel.Warning)
        .AddFilter("LoggingConsoleApp.Program", LogLevel.Debug)
        .AddConsole();
});
ILogger<ParserApp> logger = loggerFactory.CreateLogger<ParserApp>();

logger.LogDebug($"Application Started {DateTime.Now}");


var hostBuilder = CreateDefaultApp(args);
var host = hostBuilder.Build();

Console.WriteLine("starting");

var app = host.Services.GetRequiredService<IParserApp>();

var hostTask = host.StartAsync();
await app.RunAsync();
await hostTask;


Console.WriteLine("stopped");


static IHostBuilder CreateDefaultApp(string[] args)
{
    var builder = Host.CreateDefaultBuilder();
    builder.ConfigureServices(conf =>
    {
        conf.AddSingleton<IParserApp, ParserApp>();
        conf.AddSingleton<ICsvHandler, CsvHandler>();
    });
    builder.ConfigureLogging(conf =>
    {
        conf.ClearProviders();
        conf.AddConsole();
    });
    builder.UseConsoleLifetime();

    return builder;
}
