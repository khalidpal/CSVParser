using Castle.Core.Logging;
using CsvHelper.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CSVParser
{
    public interface ICsvHandler
    {
        //ServiceLifetime IReportServiceLifetime.Lifetime => ServiceLifetime.Transient;

        public IEnumerable<T> GetListOfRecordsFromContents<T, M>(string fileContent) where M : ClassMap;

        public IList<T> GetListOfRecordsFromPath<T, M>(string filePath, CsvConfiguration csvConfiguration) where M : ClassMap;

    }
}
