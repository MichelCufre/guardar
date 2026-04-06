using CTipoInventario = WIS.Domain.DataModel.Mappers.Constants.TipoInventario;

namespace WIS.Domain.Inventario
{
    public class InventarioTipoLpnDetalle : Inventario, IInventario
    {
        public InventarioTipoLpnDetalle()
        {
            this.TipoInventario = CTipoInventario.DetalleLpn;
            this.SoloRegistroFoto = true;
        }
    }
}
