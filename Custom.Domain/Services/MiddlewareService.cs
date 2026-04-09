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
        protected readonly ErpDataExtractor _erpExtractor;

        public MiddlewareService(
            IOptions<AuthSettings> authSettings,
            IHttpContextAccessor httpContextAccessor,
            BatchWmsApiService wmsApiService,
            IUnitOfWorkFactory uowFactory,
            ErpDataExtractor erpExtractor,
            ILogger<MiddlewareService> logger)
        {
            _authSettings  = authSettings;
            _httpContextAccessor = httpContextAccessor;
            _wmsApiService = wmsApiService;
            _uowFactory    = uowFactory;
            _erpExtractor  = erpExtractor;
            _logger        = logger;
        }

        public virtual void Run()
        {
            _logger.LogInformation("Iniciando proceso MiddlewareService");

            if (!_wmsApiService.IsEnabled())
            {
                _logger.LogWarning("WMS API deshabilitada. Proceso abortado.");
                return;
            }

            // FASE 1 — Consultar la API del cliente y encolar los datos
            ExtraerYEncolar();

            // FASE 2 — Obtener token WMS y procesar la cola hacia WMS
            var token = GetToken(CancellationToken.None).Result;
            _wmsApiService.SetAccessToken(token);

            ProcesarCola();

            _logger.LogInformation("MiddlewareService finalizado");
        }

        // ─── Fase 1: extrae datos del ERP del cliente y los encola ───────────────

        protected virtual void ExtraerYEncolar()
        {
            using (var uow = (UnitOfWorkCustom)_uowFactory.GetUnitOfWork())
            {
                EncolarSiHayDatos(uow, MiddlewareColaTipo.Producto, _erpExtractor.ObtenerXmlProductos);
                EncolarSiHayDatos(uow, MiddlewareColaTipo.Agente, _erpExtractor.ObtenerXmlAgentes);
                EncolarSiHayDatos(uow, MiddlewareColaTipo.CodigoBarras, _erpExtractor.ObtenerXmlCodigosBarras);
            }
        }

        private void EncolarSiHayDatos(UnitOfWorkCustom uow, string tipo, Func<string> obtenerPayload)
        {
            try
            {
                var payload = obtenerPayload();
                uow.MiddlewareColaRepository.Encolar(tipo, payload);
                _logger.LogInformation("Encolado tipo={Tipo}", tipo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al extraer/encolar tipo={Tipo}", tipo);
            }
        }

        // ─── Fase 2: procesa la cola y envia a WMS ───────────────────────────────

        protected virtual void ProcesarCola()
        {
            using (var uow = (UnitOfWorkCustom)_uowFactory.GetUnitOfWork())
            {
                var pendientes = uow.MiddlewareColaRepository.GetPendientes();

                foreach (var item in pendientes)
                {
                    try
                    {
                        _logger.LogInformation("Procesando cola Id={Id} Tipo={Tipo}", item.NU_COLA, item.TP_COLA);

                        var endpoint = ResolverEndpoint(item.TP_COLA);
                        var wmsPayload = TraducirPayload(item.TP_COLA, item.DS_PAYLOAD);
                        var result = _wmsApiService.CallService(endpoint, wmsPayload);

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
        }

        private static string ResolverEndpoint(string tipo)
        {
            return tipo switch
            {
                MiddlewareColaTipo.Producto => "Producto/CreateOrUpdate",
                MiddlewareColaTipo.Agente => "Agente/CreateOrUpdate",
                MiddlewareColaTipo.CodigoBarras => "CodigoBarras/CreateUpdateOrDelete",
                _ => throw new InvalidOperationException($"Tipo de cola desconocido: {tipo}")
            };
        }

        private string TraducirPayload(string tipo, string xmlPayload)
        {
            return tipo switch
            {
                MiddlewareColaTipo.Producto => _erpExtractor.ToWmsProductos(xmlPayload),
                MiddlewareColaTipo.Agente => _erpExtractor.ToWmsAgentes(xmlPayload),
                MiddlewareColaTipo.CodigoBarras => _erpExtractor.ToWmsCodigosBarras(xmlPayload),
                _ => throw new InvalidOperationException($"Tipo de cola sin traductor: {tipo}")
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
                    var error = $"Ocurrio un error al obtener el token de acceso: {response.Error}";
                    _logger.LogError(error);
                    throw new InvalidOperationException(error);
                }

                _logger.LogDebug("Autenticado via client_credentials");
                return response.AccessToken;
            }
        }
    }
}
