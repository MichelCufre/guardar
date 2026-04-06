using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.Tracking.Models
{
    public class PlanificacionRequest
    {
        public string CodigoPuntoDeEntrega { get; set; }
        public string CodigoPuntoEntregaDeposito { get; set; }
        public string CodigoAgrupacionTarea { get; set; }
        public string Descripcion { get; set; }
        public string Telefono { get; set; }
        public string Prometida { get; set; }
        public string SistemaCreacion { get; set; }

        public List<EntregaRequest> Entregas { get; set; }
        public List<RecepcionRequest> Recepciones { get; set; }
        public List<DevolucionRequest> Devoluciones { get; set; }

        public PlanificacionRequest()
        {
            this.Entregas = new List<EntregaRequest>();
            this.Recepciones = new List<RecepcionRequest>();
            this.Devoluciones = new List<DevolucionRequest>();
        }
    }

    public class EntregaRequest
    {
        public int CodigoEmpresa { get; set; }
        public string CodigoCliente { get; set; }   //CodigoAgente
        public string TipoCliente { get; set; }     //TipoAgente
        public string Numero { get; set; }
        public string Descripcion { get; set; }
        public string Tipo { get; set; }
        public string CodigoBarras { get; set; }
        public decimal? Cantidad { get; set; }
        public decimal? Volumen { get; set; }
        public decimal? Peso { get; set; }
        public decimal? Alto { get; set; }
        public decimal? Largo { get; set; }
        public decimal? Profundidad { get; set; }
        public string NumeroTracking { get; set; }
        public string TipoContenedor { get; set; }
    }

    public class RecepcionRequest
    {
        public int CodigoEmpresa { get; set; }
        public string CodigoCliente { get; set; }   //CodigoAgente
        public string TipoCliente { get; set; }     //TipoAgente
        public string Numero { get; set; }
        public string Descripcion { get; set; }
        public string Tipo { get; set; }
        public string CodigoBarras { get; set; }
        public decimal? Cantidad { get; set; }
        public decimal? Volumen { get; set; }
        public decimal? Peso { get; set; }
        public decimal? Alto { get; set; }
        public decimal? Largo { get; set; }
        public decimal? Profundidad { get; set; }
        public string NumeroTracking { get; set; }
        public string CodigoPuntoEntregaDestino { get; set; }
    }

    public class DevolucionRequest
    {
        public int CodigoEmpresa { get; set; }
        public string CodigoCliente { get; set; }   //CodigoAgente
        public string TipoCliente { get; set; }     //TipoAgente
        public string Numero { get; set; }
        public string Descripcion { get; set; }
        public string Tipo { get; set; }
        public string CodigoBarras { get; set; }
        public decimal? Cantidad { get; set; }
        public decimal? Volumen { get; set; }
        public decimal? Peso { get; set; }
        public decimal? Alto { get; set; }
        public decimal? Largo { get; set; }
        public decimal? Profundidad { get; set; }
        public string NumeroTracking { get; set; }
        public string TipoReferencia { get; set; }
        public string CodigoReferencia { get; set; }
        public List<DetalleDevolucionRequest> DetallesDevolucion { get; set; }

        public DevolucionRequest()
        {
            DetallesDevolucion = new List<DetalleDevolucionRequest>();
        }
    }

    public class DetalleDevolucionRequest
    {
        public string CodigoExterno { get; set; }
        public string TipoLinea { get; set; }
        public string CodigoBarras { get; set; }
        public string Descripcion { get; set; }
        public string Lote { get; set; }
        public decimal Cantidad { get; set; }
        public DateTime? Fecha { get; set; }
        public string Dato1 { get; set; }
        public string Dato2 { get; set; }
        public string Dato3 { get; set; }
        public string Dato4 { get; set; }
    }

}
