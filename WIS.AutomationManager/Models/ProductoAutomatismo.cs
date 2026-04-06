namespace WIS.AutomationManager.Models
{
    public class ProductoAutomatismo
    {
        public string Predio { get; set; }
        public string Codigo { get; set; }
        public int Empresa { get; set; }
        public string Descripcion { get; set; }
        public bool ManejaLote { get; set; }
        public bool ManejaVencimiento { get; set; }
        public decimal? PesoBruto { get; set; }
        public decimal? Ancho { get; set; }
        public decimal? Altura { get; set; }
        public decimal? Profundidad { get; set; }
        public string Udc { get; set; } //Tipo de subdivisión de la caja AutoStore donde se almacena. ej. CAJA1/8
        public int? UnidadesPorUdc { get; set; } //Unidades que caben en su correspondinte subdivisión
    }
}
