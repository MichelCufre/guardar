namespace WIS.Domain.General
{
    public class UbicacionPickingTipoOperacion
    {
        public UbicacionPickingTipoOperacion()
        {
        }

        public UbicacionPickingTipoOperacion(UbicacionPickingProducto ubicacion, string tipoOperacion)
        {
            Ubicacion = ubicacion;
            TipoOperacion = tipoOperacion;
        }

        public UbicacionPickingProducto Ubicacion { get; set; }
        public string TipoOperacion { get; set; }
    }
}
