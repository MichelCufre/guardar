using System.Collections.Generic;

namespace WIS.WebhookClient.Models
{
    public class AjustesRequest
    {
        public List<AjusteRequest> Ajustes { get; set; }

        public AjustesRequest()
        {
            Ajustes = new List<AjusteRequest>();
        }
    }
}
