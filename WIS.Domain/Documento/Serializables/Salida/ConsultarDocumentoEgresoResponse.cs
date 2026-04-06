namespace WIS.Domain.Documento.Serializables.Salida
{
    public class ConsultarDocumentoEgresoResponse
    {
        public ConsultarDocumentoEgresoResponse()
        {
            this.ExisteDocumento = false;
            this.Success = true;
        }

        public bool ExisteDocumento { get; set; }
        public string NumeroDocumento { get; set; }
        public string TipoDocumento { get; set; }
        public string EstadoDocumento { get; set; }
        public bool Success { get; set; }
        public string ErrorMsg { get; set; }
    }
}
