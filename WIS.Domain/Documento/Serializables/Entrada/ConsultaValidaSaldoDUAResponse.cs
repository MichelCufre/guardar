namespace WIS.Domain.Documento.Serializables.Entrada
{
    public class ConsultaValidaSaldoDUAResponse
    {
        public ConsultaValidaSaldoDUAResponse()
        {
            this.success = true;
        }

        public bool success { get; set; }
        public decimal saldoDUADisponible { get; set; }
        public string errorMsg { get; set; }
    }
}
