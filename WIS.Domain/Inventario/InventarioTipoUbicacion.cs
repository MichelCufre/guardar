using CTipoInventario = WIS.Domain.DataModel.Mappers.Constants.TipoInventario;

namespace WIS.Domain.Inventario
{
    public class InventarioTipoUbicacion : Inventario, IInventario
    {
        public InventarioTipoUbicacion()
        {
            this.TipoInventario = CTipoInventario.Ubicacion;
            this.SoloRegistroFoto = false;
        }
    }
}
