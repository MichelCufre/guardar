using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Configuration
{
    public class DatabaseSettings
    {
        public const string Position = "DatabaseSettings";
        public string Schema { get; set; }
        public string ConnectionString { get; set; }
        public string AdoDataSource { get; set; }
        public string Provider { get; set; }
        public int? MaxParameterCountPerQuery { get; set; }
    }
}
