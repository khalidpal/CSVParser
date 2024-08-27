using CsvHelper.Configuration;

namespace CSVParser
{
    public interface IParserClass
    {
        public List<DataModel> Parse(string filePath);
        public (ResponseClass, List<DataModel>) ParseAlt(string filePath);

        public List<DataModel> Parse(StreamReader csvStream, CsvConfiguration csvConfig);
        public (ResponseClass returnedResponse, List<DataModel> ReturnedData) ParseAlt(StreamReader csvStream, CsvConfiguration csvConfig);
    }
}
