namespace WIS.File.Execution
{
    public class FileUploadRequest
    {
        public string FileName { get; set; }
        public decimal Size { get; set; }
        public string Payload { get; set; }
        public string TipoEntidad { get; set; }
        public string CodigoEntidad { get; set; }
        public string Application { get; set; }
    }
}

