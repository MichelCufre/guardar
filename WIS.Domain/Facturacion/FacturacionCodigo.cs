using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.Facturacion
{
    public class FacturacionCodigo
    {
        public string CodigoFacturacion { get; set; }
        public string DescripcionFacturacion { get; set; }
        public string TipoCalculo { get; set; }
        public DateTime? FechaIngresado { get; set; }
        public DateTime? FechaModificacion { get; set; }
    }
}
