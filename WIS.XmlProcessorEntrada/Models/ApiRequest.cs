namespace WIS.XmlProcessorEntrada.Models
{
    public class ApiRequest
    {
        public int InterfazExterna {  get; set; }
        public int Empresa { get; set; }
        public string IdRequest { get; set; }
        public object Payload { get; set; }
    }
}
