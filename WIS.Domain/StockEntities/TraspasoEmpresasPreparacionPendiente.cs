namespace WIS.Domain.StockEntities
{
    public class TraspasoEmpresasPreparacionPendiente
    {
        public int Preparacion { get;  set; }
        public string Descripcion { get;  set; }
        public int? Empresa { get;  set; }
        public int? CantidadLoteAuto { get;  set; }
        public decimal? CantidadDetallePickingPorAtributo { get;  set; }
        public decimal? CantidadDetallleLpn { get;  set; }
        public decimal? CantidadDetallePickingPorLpn { get;  set; }
        public int? CantidadPickingMayorSuelto { get;  set; }
        public int? CantidadPreparada { get;  set; }
        public decimal? CantidadSaldoSinTrabajar { get;  set; }
        public int? CantidadPedidoNoTraspaso { get;  set; }
        public decimal? CantidadPendiente { get;  set; }
        public int? CantidadPickingReabastecimientoUbicacion { get;  set; }
    }
}
