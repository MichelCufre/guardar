using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.Tracking.Models
{
    public class AnularPedidoRequest
    {
        public string Numero { get; set; }
        public int CodigoEmpresa { get; set; }
        public string TipoAgente { get; set; }
        public string CodigoAgente { get; set; }
    }
}
