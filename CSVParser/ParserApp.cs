using CsvHelper.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CSVParser
{
    public class ParserApp : IParserApp
    {
        private readonly IHostLifetime _lifetime;
        private readonly IHostApplicationLifetime _appLifetime;

        public ICsvHandler _csvHandler;
        public ILogger<ParserApp> _logger;

        public string csvFilePath { get; set; }
        public CsvConfiguration csvConfiguration { get; set; }

        public ParserApp(ICsvHandler csvHandler)
        {
            _csvHandler = csvHandler;
            using var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder
                    .AddFilter("Microsoft", LogLevel.Warning)
                    .AddFilter("System", LogLevel.Warning)
                    .AddFilter("LoggingConsoleApp.Program", LogLevel.Debug)
                    .AddConsole();
            });
            _logger = loggerFactory.CreateLogger<ParserApp>();

        }

        public async Task RunAsync()
        {

            var filePath = "Empty.CSV";
            var config = new CsvConfiguration(System.Globalization.CultureInfo.InvariantCulture);

            config.Delimiter = ",";
            config.MissingFieldFound = null;
            config.TrimOptions = TrimOptions.Trim;
            config.HeaderValidated = null;
            config.BadDataFound = null;


            try
            {
                var methodResult = _csvHandler.GetListOfRecordsFromPath<DataModel, DataModelMap>(filePath, config).ToList();

                if (methodResult != null)
                {
                    if (methodResult.Count() > 0)
                    {
                        Console.WriteLine("organisationId\tsubOrganisationId\torganisationName\torganisationNumber\tparentOrChild\tlicense\tErrors");
                        foreach (var item in methodResult)
                        {
                            Console.WriteLine($"{item.organisation_id.ToString()}\t{item.suborg_id}\t{item.organisation_name}\t{item.organisation_number}\t{item.parent_or_child}\t{item.license}\t{item.errorsStr}");
                        }
                        _logger.LogInformation($"{methodResult.Count()} records");
                    }
                    else
                    {
                        _logger.LogInformation("No records");
                        Console.WriteLine("No records");
                        _logger.LogInformation("No records");
                    }
                }
                else
                {
                    _logger.LogInformation("Problems !");
                    Console.WriteLine("Problems !");
                }

            }
            catch (Exception e)
            {
                _logger.LogCritical($"Problems ! {e.Message}");
                Console.WriteLine($"Problems ! {e.Message}");
            }


            Console.WriteLine();
            Console.WriteLine("RunAsync done");
        }
    }
}
