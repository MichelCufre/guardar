namespace WIS.Domain.Automatismo.Dtos
{
    public class PtlProductRequest : PtlRequest
    {
        public string Product { get; set; }
        public int Company { get; set; }
        public string Color { get; set; }
        public string Serializado { get; set; }
    }
}
