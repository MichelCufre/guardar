namespace WIS.Domain.Documento
{
    public class SaldoDetalleDocumento
    {
        public string NumeroDocumento { get; set; }
        public string TipoDocumento { get; set; }
        public string TipoDocumentoIngresoDUA { get; set; }
        public int Empresa { get; set; }
        public string Producto { get; set; }
        public string Identificador { get; set; }
        public decimal Faixa { get; set; }
        public decimal CantidadDisponible { get; set; }
    }
}
