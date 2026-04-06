namespace WIS.Domain.Automatismo
{
    public class UbicacionPickingZonaAutomatismo
    {
        public string Ubicacion { get; set; }
        public int? Empresa { get; set; }
        public string Producto { get; set; }
        public string Zona { get; set; }
        public bool ConfirmarCodigoBarrasAut { get; set; }
        public string UnidadCajaAut { get; set; }
        public int? CantidadUnidadCajaAut { get; set; }
    }
}
