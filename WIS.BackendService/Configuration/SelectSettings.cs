using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WIS.BackendService.Configuration
{
    public class SelectSettings
    {
        public const string Position = "SelectSettings";

        public string MaxSelectAsyncResults { get; set; }
    }
}
