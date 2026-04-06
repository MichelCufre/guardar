using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.Facturacion
{
    public class FacturacionResultado
    {
        public int NumeroEjecucion { get; set; }
        public int CodigoEmpresa { get; set; }
        public string CodigoFacturacion { get; set; }
        public string NumeroComponente { get; set; }
        public string CodigoUnidadMedida { get; set; }
        public string NumeroFactura { get; set; }
        public string NumeroCuentaContable { get; set; }
        public string DescripcionAdicional { get; set; }
        public string CodigoProceso { get; set; }
        public string NumeroTicketInterfazFacturacion { get; set; }
        public decimal? CantidadResultado { get; set; }
        public short? CodigoSituacion { get; set; }
        public int? CodigoFuncAprobacionRechazo { get; set; }
        public DateTime? FechaAprobacionRechazo { get; set; }
        public DateTime? FechaIngresado { get; set; }
        public DateTime? FechaActualizacion { get; set; }
        public decimal? PrecioUnitario { get; set; }
        public decimal? PrecioMinimo { get; set; }
        public string Moneda{ get; set; }
        public long? NumeroTransaccion { get; set; }
        public long? NumeroTransaccionDelete { get; set; }
    }
}
