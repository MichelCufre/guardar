namespace WIS.Domain.Documento.Serializables.Salida
{
    public class DocumentoEgresoIngreso
    {
        public DocumentoEgresoIngreso()
        {
            this.existeDocumento = false;
            this.success = true;
        }
        public string NU_DOC_NUEVO { get; set; }
        public string TP_DOCUMENTO { get; set; }
        public string NU_EGRESO { get; set; }
        public string TP_DOCUMENTO_EGR { get; set; }
        public bool existeDocumento { get; set; }
        public bool success { get; set; }
        public string errorMsg { get; set; }
        public int usuario { get; set; }
        public string aplicacion { get; set; }
        public string FechaControl { get; set; }
    }
}
