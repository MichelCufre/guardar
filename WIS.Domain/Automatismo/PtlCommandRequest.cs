namespace WIS.Domain.Automatismo
{
    public class PtlCommandRequest
    {
        public string Id { get; set; }
        public string CommandType { get; set; }
        public long CommandId { get; set; }
        public string Address { get; set; }
        public string Text { get; set; }
        public string TextFn { get; set; }
        public string Color { get; set; }
        public string MultiPicking { get; set; }
    }
}
