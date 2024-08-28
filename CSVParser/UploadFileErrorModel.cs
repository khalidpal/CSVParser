namespace CSVParser
{
    public class UploadFileErrorModel
    {
        public int file_line_no { get; set; }

        public string line_content { get; set; }

        public string line_error_message { get; set; }
    }
}
