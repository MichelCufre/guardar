using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using WIS.Domain.Automatismo;
using WIS.Domain.Integracion.Dtos;
using WIS.Domain.Integracion.Factories;
using WIS.Domain.Integracion.HTTPMethod;

namespace WIS.Domain.Integracion
{
    public abstract class ClientIntegrationService
    {
        protected readonly ILogger _logger;
        protected readonly int _timeout;
        protected readonly string _secret;
        protected readonly IHttpContextAccessor _httpContextAccessor;

        protected HTTPMethodServiceFactory _factoryHTTPMethod = new HTTPMethodServiceFactory();
        protected IntegracionServicio _integration;
        protected Action _handleIntegration;
        protected AutenticacionFactory _autenticacionFactory;

        public ClientIntegrationService(ILogger logger,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor)
        {
            this._logger = logger;
            this._timeout = int.Parse(configuration.GetSection("AppSettings:InternalTimeout")?.Value ?? "30");
            this._secret = configuration.GetSection("AuthSettings:IntegrationSecret")?.Value;
            this._autenticacionFactory = new AutenticacionFactory();
            this._httpContextAccessor = httpContextAccessor;

            if (string.IsNullOrEmpty(this._secret))
            {
                this._secret = configuration.GetSection("IntegrationSettings:Secret")?.Value;
            }
        }

        protected void SetHandleIntegration(Action handle)
        {
            this._handleIntegration = handle;
        }

        protected void BuildIntegration()
        {
            if (this._integration == null)
            {
                this._handleIntegration.Invoke();
            }
        }

        protected void SetIntegration(IntegracionServicio config)
        {
            this._integration = new IntegracionServicio
            {
                Habilitado = config.Habilitado,
                TipoAutenticacion = config.TipoAutenticacion,
                Scope = config.Scope,
                Secret = config.Secret,
                TipoComunicacion = config.TipoComunicacion,
                UrlAuthServer = config.UrlAuthServer,
                UrlIntegracion = config.UrlIntegracion,
                User = config.User,
                SecretFormat = config.SecretFormat,
                SecretSalt = config.SecretSalt,
            };

            this._integration.Authorization = this._autenticacionFactory.Create(this._integration);
        }

        public bool IsEnabled()
        {
            this.BuildIntegration();

            return _integration?.Habilitado ?? false;
        }

        public (TOut, HttpResponseMessage) Post<TOut>(object content, string method)
        {
            return this.SendParse<TOut>(ServiceHttpProtocol.POST, content, method, timeout: _timeout, secret: _secret);
        }
        public (TOut, HttpResponseMessage) Delete<TOut>(Dictionary<string, string> queryParams, string method)
        {
            if (string.IsNullOrEmpty(method))
                throw new ArgumentNullException("method");

            var urlParams = "/delete?" + string.Join("&", queryParams.Select(s => $"{Uri.EscapeDataString(s.Key)}={Uri.EscapeDataString(s.Value)}"));

            this.BuildIntegration();
            var t = Task.Run(() =>
            {
                var service = this.GetHTTPMethodService(ServiceHttpProtocol.DELETE, this._integration.UrlIntegracion + method + urlParams, _timeout, _secret);

                return service.ExecuteAsync<TOut>(null);
            });

            t.Wait();

            return t.Result;
        }
        public (TOut, HttpResponseMessage) Get<TOut>(NameValueCollection content, string method)
        {
            return this.SendParse<TOut>(ServiceHttpProtocol.GET, null, method, content, timeout: _timeout);
        }

        protected (TOut, HttpResponseMessage) SendParse<TOut>(ServiceHttpProtocol protocol, object content, string method, NameValueCollection queryParams = null, int timeout = 30, string secret = "")
        {
            if (string.IsNullOrEmpty(method))
                throw new ArgumentNullException("method");

            this.BuildIntegration();

            var t = Task.Run(() =>
            {
                var service = this.GetHTTPMethodService(protocol, this._integration.UrlIntegracion + method, timeout, secret);

                service.SetQueryParams(queryParams);

                return service.ExecuteAsync<TOut>(content);
            });

            t.Wait();

            return t.Result;
        }

        protected T WaitAsync<T>(Task<T> task)
        {
            var t = Task.Run(() => task);

            t.Wait();

            return t.Result;
        }

        protected HTTPMethodService GetHTTPMethodService(ServiceHttpProtocol protocolo, string url, int timeout, string secret)
        {
            var method = this._factoryHTTPMethod.Create(protocolo, this._logger, _httpContextAccessor, timeout, secret);

            method.SetValuesRequest(url, this._integration);

            return method;
        }

        protected AutomatismoInterpreterRequest Map(AutomatismoInterfaz interfaz, object objContent)
        {
            return new AutomatismoInterpreterRequest
            {
                IntegracionServicio = new IntegracionServicioConfigRequest
                {
                    Habilitado = interfaz.IntegracionServicio.Habilitado,
                    Scope = interfaz.IntegracionServicio.Scope,
                    Secret = interfaz.IntegracionServicio.Secret,
                    TipoAutenticacion = interfaz.IntegracionServicio.TipoAutenticacion,
                    TipoComunicacion = interfaz.IntegracionServicio.TipoComunicacion,
                    UrlAuthServer = interfaz.IntegracionServicio.UrlAuthServer,
                    UrlIntegracion = interfaz.IntegracionServicio.UrlIntegracion,
                    User = interfaz.IntegracionServicio.User,
                },
                IntegracionServicioConexion = new IntegracionServicioConexionRequest
                {
                    Interfaz = interfaz.Interfaz,
                    InterfazExterna = interfaz.InterfazExterna,
                    ProtocoloComunicacion = interfaz.ProtocoloComunicacion,
                    Url = interfaz.Method,
                    Contenido = JsonConvert.SerializeObject(objContent)
                }
            };
        }

    }
}
