using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Logging;
using System.Globalization;

namespace CSVParser
{
    public class CsvHandler : ICsvHandler
    {
        #region *** Setup ***

        private ILogger logger;

        public CsvHandler(ILogger<CsvHandler> _logger)
        {
            this.logger = _logger;
        }

        public IEnumerable<T> GetListOfRecords<T, M>(string fileContent) where M : ClassMap
        {
            var _csvHelper = new Factory();

            var config = new CsvConfiguration(System.Globalization.CultureInfo.InvariantCulture);

            config.Delimiter = ",";
            config.MissingFieldFound = null;
            config.TrimOptions = TrimOptions.Trim;
            config.HeaderValidated = null;
            config.BadDataFound = null;

            using (var stringReader = new StringReader(fileContent))
            {
                using (var csvReader = _csvHelper.CreateReader(stringReader, config))
                {
                    csvReader.Context.RegisterClassMap<M>();
                    var records = csvReader.GetRecords<T>().ToList();
                    return records;
                }
            }
        }

        #endregion

    }
}
