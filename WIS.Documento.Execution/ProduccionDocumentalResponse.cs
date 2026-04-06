namespace WIS.Documento.Execution
{
    public class ProduccionDocumentalResponse
    {
        public bool Success { get; set; }
        public string ErrorMsg { get; set; }
        public string[] StrArguments { get; set; }
        public string NroDocumentoIngreso { get; set; }
        public string TipoDocumentoIngreso { get; set; }
        public string NroDocumentoEgreso { get; set; }
        public string TipoDocumentoEgreso { get; set; }

        public ProduccionDocumentalResponse()
        {
            Success = true;
        }
    }
}
