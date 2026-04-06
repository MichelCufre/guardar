namespace WIS.Webhook.Execution
{
    public class WebhookRequest
    {
        public string Application { get; set; }
        public string PayloadUrl { get; set; }
        public string Secret { get; set; }
        public string Empresa { get; set; }
    }
}
