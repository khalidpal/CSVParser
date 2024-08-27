using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using static System.Net.Mime.MediaTypeNames;

namespace CSVParser
{
    public class ParserClass : IParserClass
    {
        #region *** Setup ***

        private ILogger logger;

        public ParserClass(ILogger<ParserClass> logger)
        {
            this.logger = logger;
        }

        #endregion

        #region *** Old Method ***

        // With CsvHelper
        public (ResponseClass, List<DataModel>) ParseAlt(string filePath)
        {
            var response = new ResponseClass() { isDone = false, Messages = "None" };
            var rows = new List<DataModel>();

            try
            {
                rows = Parse(filePath);
                response = new ResponseClass() { isDone = true, Messages = "All Done!" };
                logger.LogInformation("All done!");
            }
            catch (Exception e)
            {
                response = new ResponseClass() { isDone = false, Messages = e.Message };
                logger.LogError($"Exception! {e.Message}");
            }
            return (response, rows);
        }


        public List<DataModel> Parse(string filePath)
        {
            var rows = new List<DataModel>();

            if (!File.Exists(filePath))
            {
                logger.LogCritical("The specified file was not found.!");
                throw new FileNotFoundException("The specified file was not found.", filePath);
            }

            var config = new CsvConfiguration(System.Globalization.CultureInfo.InvariantCulture);

            config.Delimiter = ",";
            config.MissingFieldFound = null;
            config.TrimOptions = TrimOptions.Trim;
            config.HeaderValidated = null;
            config.BadDataFound = null;

            IList<string> readRow = new List<string>();
                
            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, config))
            {
                csv.Context.RegisterClassMap<DataModelMap>();

                rows = csv.GetRecords<DataModel>().ToList();
            }
            logger.LogInformation("CSV file converted to list of DataClass!");
            return rows;
        }

        #endregion

        #region *** New Method ***

        public (ResponseClass returnedResponse, List<DataModel> ReturnedData) ParseAlt(StreamReader csvStream, CsvConfiguration csvConfig)
        {
            var response = new ResponseClass() { isDone = false, Messages = "None" };
            var rows = new List<DataModel>();

            try
            {
                rows = Parse(csvStream, csvConfig);
                response = new ResponseClass() { isDone = true, Messages = "All Done!" };
                logger.LogInformation("All done!");
            }
            catch (Exception e)
            {
                response = new ResponseClass() { isDone = false, Messages = e.Message };
                logger.LogError($"Exception! {e.Message}");
            }
            return (response, rows);
        }

        public List<DataModel> Parse(StreamReader csvStream, CsvConfiguration csvConfig)
        {
            var rows = new List<DataModel>();
            //IList<string> readRow = new List<string>();

            try
            {
                using (var csv = new CsvReader(csvStream, csvConfig))
                {
                    csv.Context.RegisterClassMap<DataModelMap>();

                    rows = csv.GetRecords<DataModel>().ToList();
                }
                logger.LogInformation("CSV file converted to list of DataClass!");
            }
            catch (Exception e)
            {
                logger.LogError($"Exception! {e.Message}");
                //throw;
            }


            return rows;
        }

        #endregion

    }
}
