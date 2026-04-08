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
using WIS.Domain.Services.Interfaces;

namespace Custom.Domain.Services
{
    public class MiddlewareService : IMiddlewareService
    {
        protected readonly IOptions<AuthSettings> _authSettings;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly ILogger<MiddlewareService> _logger;
        //protected readonly BatchWmsApiService _wmsApiService;
        protected readonly IWmsApiService _wmsApiService;


        public MiddlewareService(
            IOptions<AuthSettings> authSettings,
            IHttpContextAccessor httpContextAccessor,
            //BatchWmsApiService wmsApiService,
            IWmsApiService wmsApiService,
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

            //_wmsApiService.SetAccessToken(token);


            //CrearProducto(new ProductosRequest
            //{
            //    Empresa = 0,
            //    DsReferencia = "Middleware - Productos",
            //    Archivo = "",
            //    IdRequest = "",
            //    Productos = new List<ProductoRequest>
            //    {
            //    }
            //});


            //CrearAgentes(new AgentesRequest
            //{
            //    Empresa = 0,           
            //    DsReferencia = "Middleware - Agentes",
            //    Archivo = "",          
            //    IdRequest = "",        
            //    Agentes = new List<AgenteRequest>
            //    {
            //    }
            //});

            //CrearPedidos(new PedidosRequest
            //{
            //    Empresa = 0,           
            //    DsReferencia = "Middleware - Pedidos",
            //    Archivo = "",          
            //    IdRequest = "",        
            //    Pedidos = new List<PedidoRequest>
            //    {
            //    }
            //});

            CrearCodigoBarras(new CodigosBarrasRequest
            {
                Empresa = 1,
                DsReferencia = "Creación de códigos de barras",
                Archivo = "Archivo",
                IdRequest = "123",
                CodigosDeBarras =
                [
                    new CodigoBarraRequest
                    {
                        Codigo = "CB1",
                        Producto = "PR1",
                        TipoCodigo = 1,
                        PrioridadUso = 1,
                        CantidadEmbalaje = 1,
                        TipoOperacion = "A"
                    }
                ]
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
