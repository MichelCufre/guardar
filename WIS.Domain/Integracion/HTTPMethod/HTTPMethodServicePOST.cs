using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using WIS.Domain.Integracion.Enums;
using WIS.Domain.Security;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace WIS.Domain.Integracion.HTTPMethod
{
	public class HTTPMethodServicePOST : HTTPMethodService
	{
		protected readonly string _secret;

		public HTTPMethodServicePOST(IHttpContextAccessor httpContextAccessor, ILogger logger, int timeout, string secret)
			: base(httpContextAccessor, logger, timeout)
		{
			_secret = secret;
		}

		public override async Task<(TOut, HttpResponseMessage)> ExecuteAsync<TOut>(object requestData)
		{
			try
			{
				var baseAddress = new Uri(this._baseAddress);
				var serializeOptions = new JsonSerializerOptions
				{
					PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
					WriteIndented = true
				};

				var strContent = (requestData.GetType() == typeof(string)) ? (string)requestData : JsonSerializer.Serialize(requestData, serializeOptions);

				using (var httpClient = new HttpClient())
				{
					if (this._integracionServicio.Authorization != null)
					{
						httpClient.DefaultRequestHeaders.Authorization = this._integracionServicio.Authorization.GetAuthorizationHeaderValue(_httpContextAccessor);
					}

					var requestUri = new Uri(this._baseAddress);
					var content = new StringContent(strContent, Encoding.UTF8, "application/json");

					if ((this._integracionServicio.Authorization == null || httpClient.DefaultRequestHeaders.Authorization == null)
						&& !string.IsNullOrEmpty(this._integracionServicio.Secret))
					{
						if (this._integracionServicio.TipoAutenticacion == TipoAutenticacionDb.SECRET)
						{
							var secret = SecurityLogic.GetSecret(this._integracionServicio);
							var hash = ComputeHash(secret, strContent);
							content.Headers.Add("X-Hub-Signature", Convert.ToBase64String(hash));
						}
					}
					else if (!string.IsNullOrEmpty(_secret))
					{
						var hash = ComputeHash(_secret, strContent);
						content.Headers.Add("X-Hub-Signature", Convert.ToBase64String(hash));
					}

					var builder = new UriBuilder(requestUri);
					var query = HttpUtility.ParseQueryString(builder.Query);

					if (this._queryParams != null)
					{
						foreach (var key in this._queryParams.AllKeys)
						{
							query[key] = this._queryParams[key];
						}
					}

					builder.Query = query.ToString();
					requestUri = builder.Uri;

					httpClient.Timeout = TimeSpan.FromMinutes(_timeout);

					var response = await httpClient.PostAsync(requestUri, content);
					var result = default(TOut);

					if (response.IsSuccessStatusCode)
					{
						var responseContent = await response?.Content?.ReadAsStringAsync();

						if (responseContent != null)
						{
							result = JsonConvert.DeserializeObject<TOut>(responseContent);
						}
					}

					return (result, response);
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "** ERROR ** Post PostAsync: " + ex.Message);
				throw ex;
			}
		}
	}

}
