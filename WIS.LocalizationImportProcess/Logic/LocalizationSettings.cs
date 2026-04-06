namespace WIS.LocalizationImportProcess.Logic
{
    public class LocalizationSettings
    {
        public string LocalePath { get; set; }
        public string ServiceUri { get; set; }
        public string DefaultLanguage { get; set; }
        public string MutexId { get; set; }
        public int? MutexTimeout { get; set; }
    }
}
