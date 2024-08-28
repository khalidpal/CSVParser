using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using System.Text;

namespace CSVParser
{
    public class DataModel
    {
        required public int organisation_id { get; set; }

        [Optional]
        public string suborg_id { get; set; }

        required public string organisation_name { get; set; }

        required public string organisation_number { get; set; }

        required public string parent_or_child { get; set; }

        [Optional]
        public string license { get; set; }

        [Ignore]
        public string errorsStr { get; set; }

        [Ignore]
        public UploadFileErrorModel errorsObj { get; set; }
    }

    public class DataModelMap : ClassMap<DataModel>
    {
        public DataModelMap()
        {
            var errorsInRow = string.Empty;

            Map(m => m.organisation_id).Index(0).Validate(field => !field.Equals(null));
            Map(m => m.suborg_id);
            Map(m => m.organisation_name).Validate(field => !field.Equals(string.Empty));
            Map(m => m.organisation_number).Validate(field => !field.Equals(string.Empty));
            Map(m => m.parent_or_child).Validate(field => !field.Equals(string.Empty));
            Map(m => m.license);

            Map(m => m.errorsStr).Index(6).Convert(args =>
            {
                var theRow = args.Row;
                var errors = new StringBuilder();
                if (string.IsNullOrEmpty(theRow.GetField(nameof(DataModel.organisation_id))))
                {
                    errors.Append("organisation_id is null");
                }
                if (string.IsNullOrEmpty(theRow.GetField(nameof(DataModel.suborg_id))))
                {
                    errors.Append("suborg_id is null");
                }
                if (string.IsNullOrEmpty(theRow.GetField(nameof(DataModel.organisation_name))))
                {
                    errors.Append("organisation_name is null");
                }
                if (string.IsNullOrEmpty(theRow.GetField(nameof(DataModel.organisation_number))))
                {
                    errors.Append("organisation_number is null");
                }
                if (string.IsNullOrEmpty(theRow.GetField(nameof(DataModel.parent_or_child))))
                {
                    errors.Append("parent_or_child is null");
                }
                if (string.IsNullOrEmpty(theRow.GetField(nameof(DataModel.license))))
                {
                    errors.Append("license is null");
                }

                errorsInRow = errors.ToString();
                return errorsInRow;
            });

            Map(m => m.errorsObj).Convert(args =>
            {
                    var rowErrors = new UploadFileErrorModel
                    {
                        file_line_no = args.Row.Context.Reader.Parser.Row,
                        line_content = args.Row.Context.Reader.Parser.RawRecord,
                        line_error_message = errorsInRow
                    };


                return rowErrors;
            });

        }
    }
}
