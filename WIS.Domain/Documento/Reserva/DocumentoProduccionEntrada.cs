namespace WIS.Domain.Documento.Reserva
{
    public class DocumentoProduccionEntrada
    {
        public int NumeroPreparacion { get; set; }
        public string NumeroPedido { get; set; }
        public int NumeroContenedor { get; set; }
        public int Empresa { get; set; }
        public string Producto { get; set; }
        public decimal Faixa { get; set; }
        public string Identificador { get; set; }
        public decimal? CantidadReservada { get; set; }

        public virtual string GetKey(string idAnulacion)
        {
            return string.Format("{0}#{1}#{2}#{3}#{4}#{5}#{6}#{7}", NumeroPreparacion, NumeroPedido, NumeroContenedor, Empresa, Producto, Faixa, Identificador, idAnulacion);
        }
    }
}
