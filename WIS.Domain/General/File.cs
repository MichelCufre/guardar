namespace WIS.Domain.General
{
    public class File
    {
        public string FileId { get; set; }
        public string FileName { get; set; }
        public decimal Size { get; set; }
        public string Content { get; set; }
        public string TipoEntidad { get; set; }
        public string CodigoEntidad { get; set; }
        public int CodigoFuncionario { get; set; }
    }
}

