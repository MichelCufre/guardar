using WIS.Domain.DataModel.Mappers.Constants;

namespace WIS.Domain.Inventario
{
    public class InventarioOperacion
    {
        public decimal NumeroInventario { get; set; }
        public decimal NumeroInventarioUbicacion { get; set; }
        public decimal NumeroInventarioUbicacionDetalle { get; set; }
        public string TipoInventario { get; set; }
        public string Ubicacion { get; set; }
        public string Estado { get; set; }
        public string Predio { get; set; }
        public string RestablecerLpnFinalizado { get; set; }
        public string ModificarStockEnDiferencia { get; set; }
        public string GenerarPrimerConteo { get; set; }

        public virtual bool EnProceso()
        {
            return this.Estado == EstadoInventario.EnProceso;
        }
    }
}
