namespace WIS.File.Execution
{
    public class FileDownloadResponse
    {
        public string FileName { get; set; }
        public string ContetType { get; set; }
        public byte[] FileContents { get; set; }

        public FileDownloadResponse(string fileName, string contentType, byte[] fileContents)
        { 
            FileName = fileName;
            ContetType = contentType;
            FileContents = fileContents;
        }
    }
}
