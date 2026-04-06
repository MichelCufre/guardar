using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.Tracking.Models
{
    public class PuntoEntregaCarga
    {
        public int Camion { get; set; }
        public string Cliente { get; set; }
        public int Empresa { get; set; }
        public string Pedido { get; set; }
        public string Predio { get; set; }
        public string PuntoEntrega { get; set; }
        public int? NuOrdenEntrega { get; set; }
        public bool PedidoSincronizado { get; set; }
        public string TipoExpedicion { get; set; }
        public bool TpExpManejaTracking { get; set; }
        public short? NuPrioridadCarga { get; set; }
    }
}
