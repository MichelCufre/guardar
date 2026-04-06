using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.Tracking.Models
{
    public class PuntoDeEntregaCliente
    {
        public string CodigoCliente { get; set; }
        public int Empresa { get; set; }
        public string CodigoAgente { get; set; }
        public string TipoAgente { get; set; }
        public string PuntoEntregaCliente { get; set; }
        public string DireccionCliente { get; set; }
        public string PuntoEntregaPedido { get; set; }
        public string DireccionPedido { get; set; }
    }
}
