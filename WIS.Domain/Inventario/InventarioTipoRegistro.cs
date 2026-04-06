using WIS.Persistence.Database;
using CTipoInventario = WIS.Domain.DataModel.Mappers.Constants.TipoInventario;

namespace WIS.Domain.Inventario
{
    public class InventarioTipoRegistro : Inventario, IInventario
    {
        public InventarioTipoRegistro()
        {
            this.TipoInventario = CTipoInventario.Registro;
            this.SoloRegistroFoto = true;
        }
    }
}
