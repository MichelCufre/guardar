namespace WIS.Domain.Automatismo.Dtos
{
    public class PtlColorRequest : PtlRequest
    {
        public int UserId { get; set; }
        public string Color { get; set; }
        public string Serializado { get; set; }
    }
}
