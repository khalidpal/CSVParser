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

        [Optional]
        public string? errors { get; set; }
    }

    public class DataModelMap : ClassMap<DataModel>
    {
        public DataModelMap()
        {
            Map(m => m.organisation_id).Validate(field => !field.Equals(null));
            Map(m => m.suborg_id);
            Map(m => m.organisation_name).Validate(field => !field.Equals(string.Empty));
            Map(m => m.organisation_number).Validate(field => !field.Equals(string.Empty));
            Map(m => m.parent_or_child).Validate(field => !field.Equals(string.Empty));
            Map(m => m.license);

            Map(m => m.errors).Convert(args =>
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

                return errors.ToString();
            });
        }
    }
}
