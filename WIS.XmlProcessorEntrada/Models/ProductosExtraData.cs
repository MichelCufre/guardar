namespace WIS.XmlProcessorEntrada.Models
{
    public class ProductosExtraData
    {
        public Dictionary<int, string> Familias { get; set; }
        public Dictionary<short, string> Ramos { get; set; }
        public Dictionary<string, string> Clases { get; set; }

        public ProductosExtraData()
        {
            Familias = new Dictionary<int, string>();
            Ramos = new Dictionary<short, string>();
            Clases = new Dictionary<string, string>();
        }
    }
}
