namespace WIS.Domain.Automatismo.Dtos
{
    public class PtlActionRequest : PtlProductRequest
    {
        public string Position { get; set; }
        public string Display { get; set; }
        public string DisplayFn { get; set; }
        public int UserId { get; set; }
        public string Detail { get; set; }
        public string Serializado { get; set; }
    }
}
