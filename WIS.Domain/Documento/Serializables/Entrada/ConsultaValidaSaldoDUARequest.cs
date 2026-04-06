namespace WIS.Domain.Documento.Serializables.Entrada
{
    public class ConsultaValidaSaldoDUARequest
    {
        public int usuario { get; set; }
        public string aplicacion { get; set; }
        public string produto { get; set; }
        public int empresa { get; set; }
        public decimal faixa { get; set; }
        public string identificador { get; set; }
    }
}
