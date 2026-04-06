using Newtonsoft.Json;

namespace WIS.Domain.Impresiones.Dtos
{
    public class ImpresionRequest
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        [JsonProperty(PropertyName = "print_num_record")]
        public string Number { get; set; }
        [JsonProperty(PropertyName = "user")]
        public string User { get; set; }
        [JsonProperty(PropertyName = "printer_name")]
        public string PrinterAddress { get; set; }
        [JsonProperty(PropertyName = "printer_data")]
        public string PrintData { get; set; }
        [JsonProperty(PropertyName = "referencia")]
        public string Reference { get; set; }
        [JsonProperty(PropertyName = "printerPort")]
        public string Port { get; set; }
        [JsonProperty(PropertyName = "printerUser")]
        public string PrinterUser { get; set; }
        [JsonProperty(PropertyName = "printerPass")]
        public string PrinterPassword { get; set; }
        [JsonProperty(PropertyName = "client_user")]
        public string Client { get; set; }
    }
}
