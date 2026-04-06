namespace WIS.Domain.Documento.Integracion.Egreso
{
    public class InformacionReserva
    {
        public string NumeroDocumentoIngreso { get; set; }
        public string TipoDocumentoIngreso { get; set; }
        public int Preparacion { get; set; }
        public int Empresa { get; set; }
        public string Producto { get; set; }
        public decimal Faixa { get; set; }
        public string IdentificadorPicking { get; set; }
        public string Identificador { get; set; }
        public decimal? CantidadAfectada { get; set; }
    }
}
