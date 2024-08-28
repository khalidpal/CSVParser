using Castle.Core.Logging;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Logging;
using System.Globalization;

namespace CSVParser
{
    internal sealed class CsvHandler : ICsvHandler
    {
        #region *** Setup ***

        public IEnumerable<T> GetListOfRecordsFromContents<T, M>(string fileContent) where M : ClassMap
        {
            var _csvHelper = new Factory();

            var config = new CsvConfiguration(System.Globalization.CultureInfo.InvariantCulture);

            config.Delimiter = ",";
            config.MissingFieldFound = null;
            config.TrimOptions = TrimOptions.Trim;
            config.HeaderValidated = null;
            config.BadDataFound = null;

            var tmpReader = new StringReader(fileContent);
            if (tmpReader.Peek() == -1)
            {
                throw new InvalidDataException("File does not have any data!");
            }

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


        public IList<T> GetListOfRecordsFromPath<T, M>(string filePath, CsvConfiguration csvConfiguration) where M : ClassMap
        {
            IEnumerable<T> rows = new List<T>();
            var returnedRows = new List<T>();

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("Required file not found!");
            }

            var tmpReader = new StreamReader(filePath);
            if (tmpReader.Peek() == -1)
            {
                tmpReader.Dispose();
                throw new InvalidDataException("File does not have any data!");
            }

            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, csvConfiguration))
            {
                csv.Context.RegisterClassMap<M>();

                rows = csv.GetRecords<T>();

                returnedRows = rows.ToList();
            }
            
            return returnedRows;
        }
        #endregion

    }
}
