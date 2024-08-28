using CsvHelper.Configuration;
using Microsoft.Extensions.Hosting;

namespace CSVParser
{
    public interface IParserApp
    {
        public Task RunAsync();
    }
}
