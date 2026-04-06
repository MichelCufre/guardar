using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Specialized;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.Integracion.HTTPMethod
{
	public abstract class HTTPMethodService
	{
		protected string _apiKey { get; set; }
		protected string _baseAddress { get; set; }
		protected string _template { get; set; }
		protected NameValueCollection _pathParams { get; set; }
		protected NameValueCollection _queryParams { get; set; }
		protected IntegracionServicio _integracionServicio { get; set; }
		protected object _fromBody { get; set; }
		protected ILogger _logger;
		protected int _timeout;
		protected IHttpContextAccessor _httpContextAccessor;

		public HTTPMethodService(IHttpContextAccessor httpContextAccessor, ILogger logger, int timeout)
		{
			_logger = logger;
			_timeout = timeout;
			_httpContextAccessor = httpContextAccessor;
		}

		public void SetValuesRequest(string baseAddress, IntegracionServicio integracionServicio)
		{
			this._integracionServicio = integracionServicio;
			this._baseAddress = baseAddress;
		}

		public abstract Task<(TOut, HttpResponseMessage)> ExecuteAsync<TOut>(object requestData);

		public void SetQueryParams(NameValueCollection queryParams)
		{
			_queryParams = queryParams;
		}

		public static byte[] ComputeHash(string secret, string content)
		{
			var bytes = Encoding.UTF8.GetBytes(secret);
			using (var hmac = new HMACSHA512(bytes))
			{
				bytes = Encoding.UTF8.GetBytes(content);
				return hmac.ComputeHash(bytes);
			}
		}
	}
}
