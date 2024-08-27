using CsvHelper.Configuration;

namespace CSVParser
{
    public interface ICsvHandler
    {
        public IEnumerable<T> GetListOfRecords<T, M>(string fileContent) where M : ClassMap;
    }
}
