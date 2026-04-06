using System;
using WIS.Domain.DataModel.Mappers.Constants;

namespace WIS.Domain.Inventario.Factories
{
    public class InventarioFactory
    {
        public static IInventario Create(string tipo)
        {
            switch (tipo)
            {
                case TipoInventario.Registro:
                    return new InventarioTipoRegistro();
                case TipoInventario.Ubicacion:
                    return new InventarioTipoUbicacion();
                case TipoInventario.Lpn:
                    return new InventarioTipoLpn();
                case TipoInventario.DetalleLpn:
                    return new InventarioTipoLpnDetalle();
                default:
                    throw new InvalidOperationException("INV410_Sec0_Error_TipoInventarioNoExiste");
            }
        }
    }
}
