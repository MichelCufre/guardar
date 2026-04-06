namespace WIS.Documento.Execution
{
    public class CambioLoteRequest
    {
        public string Producto { get; set; }
        public int Empresa { get; set; }
        public string LoteOrigen { get; set; }
        public string LoteDestino { get; set; }
        public decimal Cantidad { get; set; }
    }
}
