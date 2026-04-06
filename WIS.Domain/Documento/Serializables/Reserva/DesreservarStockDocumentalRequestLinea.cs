namespace WIS.Domain.Documento.Serializables.Reserva
{
    public class DesreservarStockDocumentalRequestLinea
    {
        public int Empresa { get; set; }
        public string Producto { get; set; }
        public decimal? Faixa { get; set; }
        public string EspecificaIdentificador { get; set; }
        public string NumeroIdentificador { get; set; }
        public decimal CantidadAnular { get; set; }
        public string Semiacabado { get; set; }
        public string Consumible { get; set; }
    }
}
