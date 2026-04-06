using System.Collections.Generic;

namespace WIS.Configuration
{
    public class ApplicationSettings
    {
        public const string Position = "AppSettings";
        public string MenuConfigPath { get; set; }
        public int? InternalTimeout { get; set; }
        public long? MaxRequestBodySize { get; set; }
        public List<ImageSetting> Images { get; set; }
        public string MutexId { get; set; }
        public int? MutexTimeout { get; set; }
        public string FaviconName { get; set; }
    }
}