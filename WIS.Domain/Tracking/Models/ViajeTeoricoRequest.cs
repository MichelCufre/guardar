using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.Tracking.Models
{
    public class ViajeTeoricoRequest
    {
        public int Numero { get; set; }
        public string Descripcion { get; set; }
        public string CodigoExternoVehiculo { get; set; }
        public string Deposito { get; set; }
        public int? NumeroEjecucionRuteo { get; set; }
        public string FechaEstimadaInicio { get; set; }
        public List<ViajeDetalleTeoricoRequest> Detalles { get; set; }


        public ViajeTeoricoRequest()
        {
            this.Detalles = new List<ViajeDetalleTeoricoRequest>();
        }
    }

    public class ViajeDetalleTeoricoRequest
    {
        public string CodigoPuntoDeEntrega { get; set; }
        public short NumeroParada { get; set; }
        public string CodigoAgrupacionPrincipal { get; set; }
        public List<ViajeDetalleReferenciaPedidoTeoricoRequest> ReferenciaPedidos { get; set; }
        public List<ViajeDetalleObjetoTeoricoRequest> DetalleObjetos { get; set; }

        public ViajeDetalleTeoricoRequest()
        {
            this.ReferenciaPedidos = new List<ViajeDetalleReferenciaPedidoTeoricoRequest>();
            this.DetalleObjetos = new List<ViajeDetalleObjetoTeoricoRequest>();
        }

    }

    public class ViajeDetalleReferenciaPedidoTeoricoRequest
    {
        public string CodigoAgrupacion { get; set; }
        public string Numero { get; set; }
        public int CodigoEmpresa { get; set; }
        public string TipoAgente { get; set; }
        public string CodigoAgente { get; set; }

    }

    public class ViajeDetalleObjetoTeoricoRequest
    {
        public string Numero { get; set; }
        public string Descripcion { get; set; }
        public string Tipo { get; set; }
        public string CodigoBarras { get; set; }
        public decimal? Cantidad { get; set; }
        public decimal? Peso { get; set; }
        public decimal? Volumen { get; set; }
        public decimal? Alto { get; set; }
        public decimal? Largo { get; set; }

        public decimal? Profundidad { get; set; }
        public string NumeroTracking { get; set; }
        public string TipoContenedor { get; set; }

        public ViajeDetalleObjetoTeoricoRequest()
        {
            Cantidad = 1;
            Volumen = 1;
        }
    }


}
