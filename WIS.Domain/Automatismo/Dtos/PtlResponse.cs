namespace WIS.Domain.Automatismo.Dtos
{
    public class PtlResponse
    {
        public int Id { get; set; }
        public string Ptl { get; set; }
        public string Description { get; set; }
        public string CodeType { get; set; }
        public string DescriptionType { get; set; }
        public bool IsEnabled { get; set; }
    }
}
