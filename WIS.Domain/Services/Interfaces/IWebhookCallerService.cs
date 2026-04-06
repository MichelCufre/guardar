using System.Threading.Tasks;

namespace WIS.Domain.Services.Interfaces
{
    public interface IWebhookCallerService
    {
        Task<string> Invoke(string payloadUrl, byte[] hash, string content, double timeout);
    }
}
