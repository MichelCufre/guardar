using System;
using WIS.Domain.General;

namespace WIS.Domain.Documento
{
    public class DocumentoAnulacionPreparacionReserva
    {
        public int? NumeroPreparacion { get; set; }
        public int? Empresa { get; set; }
        public string Producto { get; set; }
        public decimal? Faixa { get; set; }
        public string EspecificaIdentificador { get; set; }
        public string Identificador { get; set; }
        public decimal? CantidadAnular { get; set; }
        public string IdentificadorAnulacion { get; set; }
        public string Estado { get; set; }
        public DateTime? FechaAlta { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public string Semiacabado { get; set; }
        public string Consumible { get; set; }

        public virtual void AnulacionEjecutada()
        {
            this.Estado = EstadoAnularReservaPreparacion.EJECUTADA;
            this.FechaModificacion = DateTime.Now;
        }

        public virtual void AnulacionFallida()
        {
            this.Estado = EstadoAnularReservaPreparacion.ERROR;
            this.FechaModificacion = DateTime.Now;
        }
    }
}
