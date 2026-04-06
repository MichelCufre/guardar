namespace WIS.Domain.Automatismo.Dtos
{
    public class PtlCommandConfirmRequest
    {
        public string Id { get; set; }
        public string CommandType { get; set; }
        public long CommandId { get; set; }
        public string Address { get; set; }
        public string DisplayText { get; set; }
        public string Color { get; set; }
        public string Cantidad { get; set; }
        public string Detail { get; set; }
        public int UserId { get; set; }
    }
}
