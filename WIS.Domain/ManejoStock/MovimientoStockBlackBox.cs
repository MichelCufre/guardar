using System;
using WIS.Domain.ManejoStock.Enums;

namespace WIS.Domain.ManejoStock
{
    public class MovimientoStockBlackBox
    {
        public string NumeroMovimientoBB { get; set; }
        public string UbicacionOrigen { get; set; }
        public string UbicacionDestino { get; set; }
        public string CodigoProducto { get; set; }
        public string NumeroIdentificador { get; set; }
        public decimal? CodigoFaixa { get; set; }
        public int? CodigoEmpresa { get; set; }
        public decimal? CantidadMovimiento { get; set; }
        public decimal? CantidadRechazoAveria { get; set; }
        public decimal? CantidadRechazoSano { get; set; }
        public TipoMovimientosBlackBox TipoAccionMovimiento { get; set; }
        public DateTime? FechaMovimiento { get; set; }
        public int? Usuario { get; set; }
        public string Ingreso { get; set; }
        public long? NumeroInterfazEjecucion { get; set; }
    }
}