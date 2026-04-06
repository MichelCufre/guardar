using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.Facturacion
{
    public class FacturacionCodigoComponente
    {
        public string Id { get; set; }
        public string NumeroComponente { get; set; }
        public string Descripcion { get; set; }
        public DateTime? FechaIngresado { get; set; }
        public DateTime? FechaActualizado { get; set; }
        public string NumeroCuentaContable { get; set; }
        public long? NumeroTransaccion { get; set; }
    }
}


