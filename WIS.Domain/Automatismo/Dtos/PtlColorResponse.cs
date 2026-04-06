namespace WIS.Domain.Automatismo.Dtos
{
    public class PtlColorResponse
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public string Css { get; set; }
        public bool IsEnabled { get; set; }
        public int? UserId { get; set; }
    }
}
