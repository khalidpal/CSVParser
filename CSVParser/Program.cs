using CsvHelper.Configuration;
using CSVParser;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Text;

#region *** Setup ***

StringBuilder LOGDATA = new StringBuilder();

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
ILogger<ParserClass> logger = loggerFactory.CreateLogger<ParserClass>();


logger.LogInformation("CSV Parser!");
logger.LogInformation("Searching for CSV file must be named Test.CSV...");


Console.WriteLine("CSV Parser!");

Console.WriteLine("Searching for CSV file must be named Test.CSV...");

#endregion

#region *** Old Method ***

//var filePath = "Test.CSV";
//IParserClass parserClass = new ParserClass(logger);

//var (response, theList) = parserClass.ParseAlt(filePath);

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

#region *** New Method ***

var filePath = "Test.CSV";
IParserClass parserClass = new ParserClass(logger);

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

    var reader = new StreamReader(filePath);

    var methodResult = parserClass.ParseAlt(reader, config);

    reader.Dispose();

    if (methodResult.returnedResponse.isDone)
    {
        if (methodResult.ReturnedData != null && methodResult.ReturnedData.Count > 0)
        {
            Console.WriteLine("organisationId\tsubOrganisationId\torganisationName\torganisationNumber\tparentOrChild\tlicense\tErrors");
            if (methodResult.ReturnedData.Count > 0)
            {
                foreach (var item in methodResult.ReturnedData)
                {
                    Console.WriteLine($"{item.organisation_id.ToString()}\t{item.suborg_id}\t{item.organisation_name}\t{item.organisation_number}\t{item.parent_or_child}\t{item.license}\t{item.errors}");
                }
            }
        }
        else
        {
            Console.WriteLine("No records");
            logger.LogInformation("No records");
        }
    }
    else
    {
        logger.LogInformation("Problems !" + methodResult.returnedResponse.Messages);
        Console.WriteLine("Problems !" + methodResult.returnedResponse.Messages);
    }

}

#endregion


Console.ReadLine();

