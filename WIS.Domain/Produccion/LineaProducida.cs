using System;

namespace WIS.Domain.Produccion
{
    public class LineaProducida
    {
        public string Producto { get; set; }
        public int? Empresa { get; set; }
        public string Identificador { get; set; }
        public decimal Faixa { get; set; }
        public int Pasada { get; set; }
        public int Iteracion { get; set; }
        public decimal Cantidad { get; set; }
        public DateTime? Vencimiento { get; set; }
        public DateTime? FechaAlta { get; set; }
        public string Semiacabado { get; set; }
        public long? NumeroTransaccion { get; set; }

        public virtual void ConfirmarLinea(long nroTransaccion)
        {
            this.NumeroTransaccion = nroTransaccion;
        }

        public virtual void DescartarLinea()
        {
            this.NumeroTransaccion = -4;
        }

        public virtual bool IsLineaConfirmada()
        {
            return this.NumeroTransaccion != null;
        }
    }
}
