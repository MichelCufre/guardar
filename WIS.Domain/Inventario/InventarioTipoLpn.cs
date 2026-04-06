using CTipoInventario = WIS.Domain.DataModel.Mappers.Constants.TipoInventario;

namespace WIS.Domain.Inventario
{
    public class InventarioTipoLpn : Inventario, IInventario
    {
        public InventarioTipoLpn()
        {
            this.TipoInventario = CTipoInventario.Lpn;
            this.SoloRegistroFoto = true;
        }
    }
}
