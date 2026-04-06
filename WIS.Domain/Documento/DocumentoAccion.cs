namespace WIS.Domain.Documento
{
    public class DocumentoAccion
    {
        public string Codigo { get; set; }
        public DocumentoEstado Origen { get; set; }
        public DocumentoEstado Destino { get; set; }
        public string TipoDocumento { get; set; }
    }
}
