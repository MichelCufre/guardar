using System.Threading.Tasks;
using WIS.Webhook.Execution;

namespace WIS.Application.Webhook
{
    public interface IWebhookService
    {
        Task<WebhookContent> Test(int user, WebhookRequest request);
    }
}
