using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.Services.Interfaces;

namespace WIS.Domain.Services
{
    public class WebhookCallerService : IWebhookCallerService
    {
        public virtual async Task<string> Invoke(string payloadUrl, byte[] hash, string content, double timeout)
        {
            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage(HttpMethod.Post, payloadUrl))
            {
                client.Timeout = TimeSpan.FromMinutes(timeout);

                request.Content = new StringContent(content, Encoding.UTF8, "application/json");
                request.Headers.Add("X-Hub-Signature", Convert.ToBase64String(hash));

                request.Headers.ConnectionClose = true;

                using (var response = await client.SendAsync(request))
                {
                    var detail = $"StatusCode = {response.StatusCode}. ";
                    detail += await response.Content.ReadAsStringAsync();

                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        throw new Exception(detail);
                    }
                    else
                    {
                        return detail;
                    }
                }
            }
        }
    }
}
