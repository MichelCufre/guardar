namespace WIS.Domain.General.API.Dtos.Entrada
{
    public class PickingProductoResponse
    {
        public int Id { get; set; }
        public int Empresa { get; set; }
        public string CodigoProducto { get; set; }
        public string Ubicacion { get; set; }
        public decimal Faixa { get; set; }
        public decimal Padron { get; set; }
        public string UbicacionSeparacion { get; set; }
        public int? StockMinimo { get; set; }
        public int? StockMaximo { get; set; }
        public int? CantidadDesborde { get; set; }
        public int? CantidadPadronDesborde { get; set; }
        public string TipoPicking { get; set; }
        public string FechaAlta { get; set; }
        public string FechaModificacion { get; set; }
        public string Predio { get; set; }
        public long? NuTransaccion { get; set; }
        public long? NuTransaccionDelete { get; set; }

        public string CodigoUnidadCajaAutomatismo { get; set; }
        public int? CantidadUnidadCajaAutomatismo { get; set; }        
        public string FlagConfirmarCodBarrasAutomatismo { get; set; }
        public int Prioridad { get; set; }
    }
}
