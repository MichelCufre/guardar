namespace WIS.Domain.Documento.Serializables.Entrada
{
    public class ConsultarCantidadDocumentoRequest
    {
        public int usuario { get; set; }
        public string aplicacion { get; set; }                
        public string cdProd { get; set; }
        public int cdEmp { get; set; }
    }
}
