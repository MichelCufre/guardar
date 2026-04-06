using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.Facturacion
{
    public class CotizacionListas
    {
        public int CodigoListaPrecio { get; set; }
        public string CodigoFacturacion { get; set; }
        public string NumeroComponente { get; set; }
        public int? Funcionario { get; set; }
        public decimal? CantidadImporte { get; set; }
        public decimal? CantidadImporteMinimo { get; set; }
        public DateTime? FechaAlta { get; set; }
        public DateTime? FechaModificacion { get; set; }
    }
}
