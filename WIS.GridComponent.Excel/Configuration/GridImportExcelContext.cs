using System;
using System.Collections.Generic;
using System.Text;
using WIS.Components.Common;
using WIS.GridComponent.Build.Configuration;
using WIS.Translation;

namespace WIS.GridComponent.Excel.Configuration
{
    public class GridImportExcelContext : ComponentContext
    {
        public string FileName { get; set; }
        public string GridId { get; set; }
        public string Payload { get; set; }
        public ITranslator Translator { get; set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "Query")]
        public GridFetchContext FetchContext { get; set; }
    }
}
