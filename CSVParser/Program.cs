using CsvHelper.Configuration;
using CSVParser;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

#region *** Setup ***

StringBuilder LOGDATA = new StringBuilder();

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.Services.AddTransient<ICsvHandler, CsvHandler>();
using IHost host = builder.Build();


ExemplifyServiceLifetime(host.Services, "Lifetime 1");
ExemplifyServiceLifetime(host.Services, "Lifetime 2");


static void ExemplifyServiceLifetime(IServiceProvider hostProvider, string lifetime)
{
    using IServiceScope serviceScope = hostProvider.CreateScope();
    IServiceProvider provider = serviceScope.ServiceProvider;
}




void Log(params string[] args)
{
    var msg = string.Join(" ", args).TrimEnd();
    LOGDATA.AppendLine(msg);
}

using var loggerFactory = LoggerFactory.Create(builder =>
{
    builder
        .AddFilter("Microsoft", LogLevel.Warning)
        .AddFilter("System", LogLevel.Warning)
        .AddFilter("LoggingConsoleApp.Program", LogLevel.Debug)
        .AddConsole();
});
ILogger<CsvHandler> logger = loggerFactory.CreateLogger<CsvHandler>();


logger.LogInformation("CSV Parser!");
logger.LogInformation("Searching for CSV file must be named Test.CSV...");


Console.WriteLine("CSV Parser!");

Console.WriteLine("Searching for CSV file must be named Test.CSV...");

#endregion

#region *** New Method ***

var filePath = "Test.CSV";
ICsvHandler parserClass;// = new ParserClass(logger);

if (!File.Exists(filePath))
{
    logger.LogCritical("The specified file was not found.!");
}
else
{
    var config = new CsvConfiguration(System.Globalization.CultureInfo.InvariantCulture);

    config.Delimiter = ",";
    config.MissingFieldFound = null;
    config.TrimOptions = TrimOptions.Trim;
    config.HeaderValidated = null;
    config.BadDataFound = null;

    parserClass = new CsvHandler(logger);

    var reader = new StreamReader(filePath);
    var fileContent = reader.ReadToEnd();

    var listOfRecords = parserClass.GetListOfRecords<DataModel, DataModelMap>(fileContent).ToList();

    reader.Dispose();

    if (listOfRecords != null && listOfRecords.Any())
    {
        Console.WriteLine("organisationId\tsubOrganisationId\torganisationName\torganisationNumber\tparentOrChild\tlicense\tErrors");
        foreach (var item in listOfRecords)
        {
            Console.WriteLine($"{item.organisation_id.ToString()}\t{item.suborg_id}\t{item.organisation_name}\t{item.organisation_number}\t{item.parent_or_child}\t{item.license}\t{item.errors}");
        }
    }
    else
    {
        logger.LogInformation("Problems !");
        Console.WriteLine("Problems !");
    }

}

#endregion


Console.ReadLine();


#region *** Old Method ***

//var mockFilePath = "Test.CSV";
//IParserClass parserClass = new ParserClass(logger);

//var (response, theList) = parserClass.ParseAlt(mockFilePath);

//if (response.isDone)
//{
//    if (theList != null && theList.Count > 0)
//    {
//        Console.WriteLine("organisationId\tsubOrganisationId\torganisationName\torganisationNumber\tparentOrChild\tlicense\tErrors");
//        if (theList.Count > 0)
//        {
//            foreach (var item in theList)
//            {
//                Console.WriteLine($"{item.organisation_id.ToString()}\t{item.suborg_id}\t{item.organisation_name}\t{item.organisation_number}\t{item.parent_or_child}\t{item.license}\t{item.errors}");
//            }
//        }
//    }
//    else
//    {
//        Console.WriteLine("No records");
//        logger.LogInformation("No records");
//    }
//}
//else
//{
//    logger.LogInformation("Problems !" + response.Messages);
//    Console.WriteLine("Problems !" + response.Messages);
//}


#endregion


await host.RunAsync();
