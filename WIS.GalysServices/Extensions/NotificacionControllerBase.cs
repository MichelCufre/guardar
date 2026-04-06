using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WIS.Automation;
using WIS.Automation.Galys;

namespace WIS.GalysServices.Controllers.Salida
{
	[ApiController]
	[Route("[controller]")]
	public class NotificacionControllerBase : ControllerBase
	{
		protected readonly IConfiguration _config;
		public NotificacionControllerBase(IConfiguration config)
		{
			_config = config;
		}

		protected async Task<GalysResponse> SendNotification(string url, object request)
		{
			using (var client = new HttpClient())
			{
				var internalTimeout = _config.GetSection("AppSettings:InternalTimeout")?.Value ?? "30";
				var urlIntegracion = _config.GetSection("IntegrationSettings:UrlIntegracion")?.Value;

				var requestUri = urlIntegracion + url;

				var serializeOptions = new JsonSerializerOptions
				{
					PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
					WriteIndented = true
				};

				var strContent = (request.GetType() == typeof(string)) ? request.ToString() : JsonSerializer.Serialize(request, serializeOptions);
				var content = new StringContent(strContent, Encoding.UTF8, "application/json");

				client.Timeout = TimeSpan.FromMinutes(int.Parse(internalTimeout));

				var response = await client.PutAsync(requestUri, content);
				var responseContent = await response?.Content?.ReadAsStringAsync();

				if (response.IsSuccessStatusCode)
				{
					var automatismoResponse = JsonSerializer.Deserialize<AutomatismoResponse>(responseContent, serializeOptions);

					if (automatismoResponse.IsError)
					{
						return new GalysResponse()
						{
                            descError = automatismoResponse.Mensaje,
							resultado = 1
						};
					}
					else
					{
						return new GalysResponse()
						{
                            descError = "",
							resultado = 0
						};
					}
				}
				else
				{
					return new GalysResponse()
					{
                        descError = responseContent,
						resultado = 1
					};
				}
			}
		}
	}
}
