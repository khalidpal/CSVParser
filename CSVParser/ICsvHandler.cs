using CsvHelper.Configuration;

namespace CSVParser
{
    public interface ICsvHandler
    {
        IEnumerable<T> GetListOfRecordsFromContents<T, M>(string fileContent) where M : ClassMap;

        IList<T> GetListOfRecordsFromPath<T, M>(string filePath, CsvConfiguration csvConfiguration) where M : ClassMap;

        bool CreateErrorsCSV(List<DataModel> errorList, CsvConfiguration csvConfiguration);

        //Task WriteAsync<T>(string path, T record);

    }
}
