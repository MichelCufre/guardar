using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace WIS.Domain.Integracion.Authentication
{
    public class AuthenticationOAuth2 : IAuthenticationMethod
    {
        private string _clientId { get; set; }
        private string _clientSecret { get; set; }
        private string _scope { get; set; }
        private string _urlToken { get; set; }

        public AuthenticationOAuth2(string clientId, string clientSecret, string scope, string urlToken) : base()
        {
            _clientId = clientId;
            _clientSecret = clientSecret;
            _scope = scope;
            _urlToken = urlToken;
        }

        public AuthenticationHeaderValue GetAuthorizationHeaderValue(IHttpContextAccessor httpContextAccessor)
        {
            string access_token = null;

			if (httpContextAccessor?.HttpContext?.Request?.Headers?.ContainsKey("access_token") ?? false)
			{
				access_token = httpContextAccessor.HttpContext.Request.Headers["access_token"][0];
            }
            else
            {
                var task = httpContextAccessor?.HttpContext?.GetTokenAsync("access_token");

                if (task != null)
                {
                    task.Wait();

                    access_token = task.Result;
                }

                if (string.IsNullOrEmpty(access_token))
                {
                    access_token = GetTokenExplained();
                }
            }

            return new AuthenticationHeaderValue("Bearer", access_token);
        }

        private string GetTokenExplained()
        {
            string creds = string.Format("{0}:{1}", this._clientId, WebUtility.UrlEncode(this._clientSecret));
            byte[] bytes = Encoding.UTF8.GetBytes(creds);

            var data = new Dictionary<string, string>
            {
                { "grant_type", "client_credentials" },
                { "scope", this._scope }
            };

            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
            clientHandler.UseDefaultCredentials = false;

            var client = new HttpClient(clientHandler);
            var message = new HttpRequestMessage(HttpMethod.Post, this._urlToken);
            message.Content = new FormUrlEncodedContent(data);
            message.Headers.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(bytes));

            message.Headers.ConnectionClose = true;

            var response = client.SendAsync(message).Result;

            string result = response.Content.ReadAsStringAsync().Result;

            var json = JObject.Parse(result);

            if (json.TryGetValue("access_token", out JToken token))
                return token.ToString();

            return "";
        }
    }
}
