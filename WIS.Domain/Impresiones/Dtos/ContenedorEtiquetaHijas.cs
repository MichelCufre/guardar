namespace WIS.Domain.Impresiones.Dtos
{
    public class ContenedorEtiquetaHijas
    {
        public string Anexo1 { get; set; }
        public string NumeroPedido { get; set; }
        public int CantidadBultos { get; set; }
        public string CodigoCliente { get; set; }
        public string DescripcionCliente { get; set; }
        public string Anexo4 { get; set; }
        public int CodigoTransportadora { get; set; }
        public int NumeroContenedor { get; set; }
        public string CodigoZona { get; set; }
        public string CodigoBarrasEtiqueta { get; set; }
        public string TipoContenedor { get; set; }
        public string DescripcionUbicacion { get; set; }
        public string Memo { get; set; }
        public int NumeroEtiquetaHija { get; set; }
        public bool ImprimePrimerBulto { get; set; }
        public string IdExterno{ get; set; }
        public string CodigoBarras { get; set; }
        public string DescripcionContenedor { get; set; }
    }
}
