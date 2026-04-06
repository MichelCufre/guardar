using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.Facturacion
{
    public class FacturacionPalletDet
    {
        public int NumeroPalletDet { get; set; }
        public string NumeroPallet { get; set; }
        public string CodigoFacturacion { get; set; }
        public string NumeroComponente { get; set; }
        public string Estado { get; set; }
        public string AplicoMinimo { get; set; }
        public int? NumeroEjecucionFacturacion { get; set; }
        public int? CodigoEmpresa { get; set; }
        public int? NumeroBultosFacturados { get; set; }
        public decimal? CantidadResultado { get; set; }
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }
    }
}
