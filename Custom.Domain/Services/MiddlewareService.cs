using Custom.Domain.Services.Interfaces;
using IdentityModel.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using WIS.Configuration;
using WIS.Domain.General.API.Dtos.Entrada;

namespace Custom.Domain.Services
{
    public class MiddlewareService : IMiddlewareService
    {
        protected readonly IOptions<AuthSettings> _authSettings;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly ILogger<MiddlewareService> _logger;
        protected readonly BatchWmsApiService _wmsApiService;

        public MiddlewareService(
            IOptions<AuthSettings> authSettings,
            IHttpContextAccessor httpContextAccessor,
            BatchWmsApiService wmsApiService,
            ILogger<MiddlewareService> logger)
        {
            _authSettings = authSettings;
            _httpContextAccessor = httpContextAccessor;
            _wmsApiService = wmsApiService;
            _logger = logger;
        }

        public virtual void Run()
        {
            _logger.LogInformation("Iniciando proceso MiddlewareService");

            var token = GetToken(CancellationToken.None).Result;

            _wmsApiService.SetAccessToken(token);

            CrearProducto(new ProductosRequest
            {
                Empresa = 0,           // TODO: completar
                DsReferencia = "Middleware - Productos",
                Archivo = "",          // TODO: completar
                IdRequest = "",        // TODO: completar
                Productos = new List<ProductoRequest>
                {
                    // TODO: completar con datos reales
                }
            });

            CrearCodigoBarras(new CodigosBarrasRequest
            {
                Empresa = 0,           // TODO: completar
                DsReferencia = "Middleware - Codigos de Barra",
                Archivo = "",          // TODO: completar
                IdRequest = "",        // TODO: completar
                CodigosDeBarras = new List<CodigoBarraRequest>
                {
                    // TODO: completar con datos reales
                }
            });

            CrearAgentes(new AgentesRequest
            {
                Empresa = 0,           // TODO: completar
                DsReferencia = "Middleware - Agentes",
                Archivo = "",          // TODO: completar
                IdRequest = "",        // TODO: completar
                Agentes = new List<AgenteRequest>
                {
                    // TODO: completar con datos reales
                }
            });

            CrearPedidos(new PedidosRequest
            {
                Empresa = 0,           // TODO: completar
                DsReferencia = "Middleware - Pedidos",
                Archivo = "",          // TODO: completar
                IdRequest = "",        // TODO: completar
                Pedidos = new List<PedidoRequest>
                {
                    // TODO: completar con datos reales
                }
            });

            _logger.LogInformation("MiddlewareService finalizado");
        }

        /// <summary>
        /// Obtiene el access token. Si hay HttpContext lo toma del header access_token,
        /// caso contrario usa client_credentials (flujo batch), igual que PrintingService.
        /// </summary>
        protected virtual async Task<string> GetToken(CancellationToken cancellationToken)
        {
            _logger.LogDebug("Obteniendo token de acceso...");

            var access_token = _httpContextAccessor.HttpContext?.Request?.Headers["access_token"][0];

            if (!string.IsNullOrEmpty(access_token))
            {
                _logger.LogDebug("Autenticado desde HttpContext");
                return access_token;
            }

            using (var client = new HttpClient())
            {
                var response = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
                {
                    Address = _authSettings.Value.TokenUrl,
                    ClientId = _authSettings.Value.ClientId,
                    ClientSecret = _authSettings.Value.ClientSecret,
                    Scope = _authSettings.Value.AccessScope,
                }, cancellationToken);

                if (response.IsError)
                {
                    var error = $"Ocurrió un error al obtener el token de acceso: {response.Error}";
                    _logger.LogError(error);
                    throw new InvalidOperationException(error);
                }

                _logger.LogDebug("Autenticado via client_credentials");
                return response.AccessToken;
            }
        }

        protected virtual void CrearProducto(ProductosRequest request)
        {
            _logger.LogInformation("Procesando productos...");

            if (!_wmsApiService.IsEnabled())
                throw new Exception("INT050_Sec0_Error_ApiDeshabilitada");

            string result = _wmsApiService.CallService("Producto/CreateOrUpdate", JsonConvert.SerializeObject(request));

            _logger.LogInformation($"Productos procesados. {result}");
        }

        protected virtual void CrearCodigoBarras(CodigosBarrasRequest request)
        {
            _logger.LogInformation("Procesando codigos de barra...");

            if (!_wmsApiService.IsEnabled())
                throw new Exception("INT050_Sec0_Error_ApiDeshabilitada");

            string result = _wmsApiService.CallService("CodigoBarras/CreateOrUpdate", JsonConvert.SerializeObject(request));

            _logger.LogInformation($"Codigos de barra procesados. {result}");
        }

        protected virtual void CrearAgentes(AgentesRequest request)
        {
            _logger.LogInformation("Procesando agentes...");

            if (!_wmsApiService.IsEnabled())
                throw new Exception("INT050_Sec0_Error_ApiDeshabilitada");

            string result = _wmsApiService.CallService("Agente/CreateOrUpdate", JsonConvert.SerializeObject(request));

            _logger.LogInformation($"Agentes procesados. {result}");
        }

        protected virtual void CrearPedidos(PedidosRequest request)
        {
            _logger.LogInformation("Procesando pedidos...");

            if (!_wmsApiService.IsEnabled())
                throw new Exception("INT050_Sec0_Error_ApiDeshabilitada");

            string result = _wmsApiService.CallService("Pedido/CreateOrUpdate", JsonConvert.SerializeObject(request));

            _logger.LogInformation($"Pedidos procesados. {result}");
        }
    }
}
