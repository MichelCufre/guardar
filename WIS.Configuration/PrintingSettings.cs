namespace WIS.Configuration
{
    public class PrintingSettings
    {
        public const string Position = "PrintingSettings";
        public string Endpoint { get; set; }
        public bool IsEnabled { get; set; }
        public string MutexId { get; set; }
        public int? MutexTimeout { get; set; }
    }
}
