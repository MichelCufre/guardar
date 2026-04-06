namespace WIS.Documento.Execution
{
    public class ModificarReservaDocumentalRequest
    {

        public int Preparacion { get; set; }
        public string Cliente { get; set; }
        public int Empresa { get; set; }
        public string Producto { get; set; }
        public decimal Faixa{ get; set; }
        public string Identificador { get; set; }
        public decimal Cantidad{ get; set; }
        public long NuTransaccion { get; set; }
        public string Aplicacion { get; set; }
        public int Usuario { get; set; }
    }
}
