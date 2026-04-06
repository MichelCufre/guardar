using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.Recepcion
{
    public class FacturaDetalle
    {
        public FacturaDetalle()
        {

        }
        public int Id { get; set; }
        public int IdFactura { get; set; }
        public int IdEmpresa { get; set; }
        public string Producto { get; set; }
        public decimal Faixa { get; set; }
        public string Identificador { get; set; }
        public decimal? CantidadFacturada { get; set; }
        public decimal? CantidadValidada { get; set; }
        public decimal? CantidadRecibida { get; set; }
        public decimal? ImporteUnitario { get; set; }
        public DateTime? FechaVencimiento { get; set; }
        public long? NumeroTransaccion { get; set; }
        public long? NumeroTransaccionDelete { get; set; }
        public DateTime? FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public string Anexo1 { get; set; }
        public string Anexo2 { get; set; }
        public string Anexo3 { get; set; }
        public string Anexo4 { get; set; }
    }
}
