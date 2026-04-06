using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.Tracking.Models
{
    public class ViajeRealRequest
    {
        public int Numero { get; set; }
        public string Descripcion { get; set; }
        public string CodigoExternoVehiculo { get; set; }
        public string Deposito { get; set; }
        public int? NumeroEjecucionRuteo { get; set; }
        public string FechaEstimadaInicio { get; set; }
        public List<ViajeReferenciaPedidoRealRequest> ReferenciaPedidos { get; set; }

        public ViajeRealRequest()
        {
            this.ReferenciaPedidos = new List<ViajeReferenciaPedidoRealRequest>();
        }
    }

    public class ViajeReferenciaPedidoRealRequest
    {
        public string CodigoAgrupacion { get; set; }
        public string Numero { get; set; }
        public int CodigoEmpresa { get; set; }
        public string TipoAgente { get; set; }
        public string CodigoAgente { get; set; }
        public bool TodoPlanificado { get; set; }

        public ViajeReferenciaPedidoRealRequest()
        {
            TodoPlanificado = false;
        }
    }
}
