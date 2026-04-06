namespace WIS.Domain.Automatismo
{
    public class PtlDetailPicking
    {
        public int Preparacion { get; set; }
        public string Cliente { get; set; }
        public int Contenedor { get; set; }
        public string TipoContenedor { get; set; }
        public string ComparteContenedorPicking { get; set; }
        public string SubClase { get; set; }
        public string UbicacionEquipo { get; set; }

        public virtual void Clonar(PtlDetailPicking data)
        {
            this.Preparacion = data.Preparacion;
            this.Cliente = data.Cliente;
            this.Contenedor = data.Contenedor;
            this.TipoContenedor = data.TipoContenedor;
            this.ComparteContenedorPicking = data.ComparteContenedorPicking;
            this.SubClase = data.SubClase;
            this.UbicacionEquipo = data.UbicacionEquipo;
        }
    }
}
