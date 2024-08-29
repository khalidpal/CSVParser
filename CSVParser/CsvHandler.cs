using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Text;

namespace CSVParser
{
    public class CsvHandler : ICsvHandler
    {
        #region *** Setup ***

        public ILogger<ParserApp> _logger;

        public CsvHandler() 
        {
            using var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder
                    .AddFilter("Microsoft", LogLevel.Warning)
                    .AddFilter("System", LogLevel.Warning)
                    .AddFilter("LoggingConsoleApp.Program", LogLevel.Debug)
                    .AddConsole();
            });
            _logger = loggerFactory.CreateLogger<ParserApp>();
        }


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

            var tmpReader = new StreamReader(filePath, Encoding.Latin1);
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


        //public async Task WriteAsync<T>(string path, T record)
        //{
        //    bool containsNewLines = ContainsNewLines(path);
        //    Encoding fileEncoding = DetectFileEncoding(path, Encoding.Latin1);

        //    using (var stream = File.Open(path, FileMode.Append))
        //    using (var writer = new StreamWriter(stream, fileEncoding))
        //    using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        //    {
        //        if (!containsNewLines)
        //        {
        //            await csv.NextRecordAsync();
        //        }
        //        csv.WriteRecord(record);
        //        await csv.NextRecordAsync();
        //    }
        //}

        public bool CreateErrorsCSV(List<DataModel> errorList, CsvConfiguration csvConfiguration)
        {
            var theResult = false;

            var csvConfig = new CsvConfiguration(System.Globalization.CultureInfo.InvariantCulture)
            {
                Encoding = Encoding.Latin1,  //(ISO-8859-1), //Encoding.UTF8,
                HasHeaderRecord = true,
                IgnoreBlankLines = true
            };

            try
            {
                using (TextWriter writer = new StreamWriter(@"errors.csv"))
                {
                    var csv = new CsvWriter(writer, csvConfig);
                    foreach (var value in errorList)
                    {
                        var writeLine = value.errorsObj.file_line_no + "," + value.errorsObj.line_content + "," + value.errorsObj.line_error_message;
                        _logger.LogInformation(writeLine);
                        csv.WriteField(value.errorsObj.file_line_no.ToString());
                        csv.WriteField(value.errorsObj.line_error_message.ToString());
                        csv.WriteField(value.errorsObj.line_content.ToString());
                        csv.NextRecord();
                    }
                }

                theResult = true;

            }
            catch (Exception)
            {

                //throw;
            }

            return theResult;

        }


        public Encoding DetectFileEncoding(string fileName, Encoding defaultEncoding)
        {
            var Utf8EncodingVerifier = Encoding.GetEncoding("utf-8",
                    new EncoderExceptionFallback(), new DecoderExceptionFallback());
            using (var reader = new StreamReader(fileName, Utf8EncodingVerifier,
                    detectEncodingFromByteOrderMarks: true, bufferSize: 1024))
            {
                try
                {
                    while (!reader.EndOfStream)
                    {
                        _ = reader.ReadLine();
                    }
                    return reader.CurrentEncoding;
                }
                catch (Exception)
                {
                    // Failed to decode the file using the BOM/UTF8. 
                    // return default ANSI encoding
                    return defaultEncoding;
                }
            }
        }


        private bool ContainsNewLines(string filePath)
        {
            using (var reader = new StreamReader(filePath))
            {
                string content = reader.ReadToEnd();
                return content.EndsWith(Environment.NewLine);
            }
        }

        #endregion

    }
}
