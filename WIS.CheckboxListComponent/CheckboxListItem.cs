using Newtonsoft.Json;
using System;

namespace WIS.CheckboxListComponent
{
    public class CheckboxListItem
    {
        [JsonProperty(PropertyName ="id")]
        public string Id { get; set; }
        [JsonProperty(PropertyName = "label")]
        public string Label { get; set; }
        [JsonProperty(PropertyName = "selected")]
        public bool Selected { get; set; }
    }
}
