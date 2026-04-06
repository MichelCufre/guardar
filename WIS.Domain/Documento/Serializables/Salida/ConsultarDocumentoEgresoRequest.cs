namespace WIS.Domain.Documento.Serializables.Salida
{
    public class ConsultarDocumentoEgresoRequest
    {
        public int CodigoCamion { get; set; }
        public string aplicacion { get; set; }
        public int usuario { get; set; }
    }
}
