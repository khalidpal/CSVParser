using CsvHelper.Configuration;
using Microsoft.Extensions.Logging;

namespace CSVParser
{
    internal sealed class ParsingHelper
    {
        public ICsvHandler csvHandler;
        public ILogger logger;

        public string csvFilePath { get; set; }
        public CsvConfiguration csvConfiguration { get; set; }


        //public ParsingHelper() { }

        public ParsingHelper(ICsvHandler _csvHandler, ILogger _logger)
        { 
            this.logger = _logger;
            this.csvHandler = _csvHandler;
        }

        public void InitHelper(string _csvFilePath, CsvConfiguration _csvConfiguration)
        { 
            this.csvConfiguration = _csvConfiguration;
            this.csvConfiguration = _csvConfiguration;
        }

        public IEnumerable<DataModel> GetRecordsFromCSV()
        { 
            var records = new List<DataModel>();

            if (!File.Exists(csvFilePath))
            {
                logger.LogCritical("The specified file was not found.!");
                return records;
            }
            else
            {
                return csvHandler.GetListOfRecordsFromPath<DataModel, DataModelMap>(csvFilePath, csvConfiguration);
            }
        }

    }
}
