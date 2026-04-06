namespace WIS.Domain.General
{
    public class CodigoBarrasTipoOperacion
    {
        public CodigoBarrasTipoOperacion()
        {
        }

        public CodigoBarrasTipoOperacion(ProductoCodigoBarra codigoBarra, string tipoOperacion)
        {
            CodigoBarra = codigoBarra;
            TipoOperacion = tipoOperacion;
        }

        public ProductoCodigoBarra CodigoBarra { get; set; }
        public string TipoOperacion { get; set; }
    }
}
