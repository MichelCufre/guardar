namespace WIS.Domain.Documento.Serializables.Reserva
{
    public class AnularReservaPreparacionRequestLinea
    {
        public int Preparacion { get; set; }
        public int Empresa { get; set; }
        public decimal? Faixa { get; set; }
        public string Producto { get; set; }
        public string EspecificaIdentificador { get; set; }
        public string NumeroIdentificador { get; set; }
        public decimal CantidadAnular { get; set; }
        public string IndetificadorAnulacion { get; set; }
    }
}
