namespace WIS.Domain.Documento.Serializables.Entrada
{
    public class ConsultarCantidadDocumentoResponse
    {
        public ConsultarCantidadDocumentoResponse()
        {
            this.success = true;
        }

        public int cantidadDocumentalDisponible { get; set; }
        public bool success { get; set; }
        public string errorMsg { get; set; }

        public decimal totalDisponibleDocumental { get; set; }
    }
}
