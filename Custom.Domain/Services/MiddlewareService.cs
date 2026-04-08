using Custom.Domain.Services.Interfaces;
using Custom.Domain.DataModel;
using Custom.Domain.DataModel.Repositories;
using Custom.Domain.Services.Interfaces;
using IdentityModel.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using WIS.Configuration;
using WIS.Domain.DataModel;

namespace Custom.Domain.Services
{
    public class MiddlewareService : IMiddlewareService
    {
        protected readonly IOptions<AuthSettings> _authSettings;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly ILogger<MiddlewareService> _logger;
        protected readonly BatchWmsApiService _wmsApiService;
        protected readonly IUnitOfWorkFactory _uowFactory;

        public MiddlewareService(
            IOptions<AuthSettings> authSettings,
            IHttpContextAccessor httpContextAccessor,
            BatchWmsApiService wmsApiService,
            IUnitOfWorkFactory uowFactory,
            ILogger<MiddlewareService> logger)
        {
            _authSettings = authSettings;
            _httpContextAccessor = httpContextAccessor;
            _wmsApiService = wmsApiService;
            _uowFactory = uowFactory;
            _logger = logger;
        }

        public virtual void Run()
        {
            _logger.LogInformation("Iniciando proceso MiddlewareService");

            if (!_wmsApiService.IsEnabled())
            {
                _logger.LogWarning("WMS API deshabilitada. Proceso abortado.");
                return;
            }

            var token = GetToken(CancellationToken.None).Result;
            _wmsApiService.SetAccessToken(token);

            using (var uow = (UnitOfWorkCustom)_uowFactory.GetUnitOfWork())
            {
                var pendientes = uow.MiddlewareColaRepository.GetPendientes();

                foreach (var item in pendientes)
                {
                    try
                    {
                        _logger.LogInformation("Procesando cola Id={Id} Tipo={Tipo}", item.NU_COLA, item.TP_COLA);

                        var endpoint = ResolverEndpoint(item.TP_COLA);
                        var result   = _wmsApiService.CallService(endpoint, item.DS_PAYLOAD);

                        uow.MiddlewareColaRepository.MarcarProcesado(item.NU_COLA);

                        _logger.LogInformation("Cola Id={Id} procesada. Resultado={Result}", item.NU_COLA, result);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error al procesar cola Id={Id} Tipo={Tipo}", item.NU_COLA, item.TP_COLA);
                        uow.MiddlewareColaRepository.MarcarError(item.NU_COLA, ex.Message);
                    }
                }
            }

            _logger.LogInformation("MiddlewareService finalizado");
        }

        private static string ResolverEndpoint(string tipo)
        {
            return tipo switch
            {
                MiddlewareColaTipo.Producto => "Producto/CreateOrUpdate",
                MiddlewareColaTipo.Agente => "Agente/CreateOrUpdate",
                MiddlewareColaTipo.CodigoBarras => "CodigoBarras/CreateOrUpdate",
                MiddlewareColaTipo.Pedido => "Pedido/CreateOrUpdate",
                _ => throw new InvalidOperationException($"Tipo de cola desconocido: {tipo}")
            };
        }

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
    }
}
