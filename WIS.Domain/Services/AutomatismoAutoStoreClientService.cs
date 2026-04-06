using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using WIS.Domain.Automatismo;
using WIS.Domain.Automatismo.Constants;
using WIS.Domain.Automatismo.Dtos;
using WIS.Domain.DataModel;
using WIS.Domain.Integracion;
using WIS.Domain.Integracion.Constants;
using WIS.Domain.Services.Interfaces;
using WIS.Security;

namespace WIS.Domain.Services
{
    public class AutomatismoAutoStoreClientService : ClientIntegrationService, IAutomatismoAutoStoreClientService
    {
        protected readonly IIdentityService _identity;

        public AutomatismoAutoStoreClientService(IUnitOfWorkFactory uowFactory,
            IIdentityService identity,
            ILogger<AutomatismoAutoStoreClientService> logger,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor) : base(logger, configuration, httpContextAccessor)
        {
            this._identity = identity;

            this.SetHandleIntegration(() =>
            {
                using var uow = uowFactory.GetUnitOfWork();

                this.SetIntegration(uow.IntegracionServicioRepository.GetIntegrationByCodigo(IntegracionesDb.AutomationManager));
            });
        }


        public virtual void SendCodigosBarras(CodigosBarrasAutomatismoRequest request)
        {
            try
            {
                var (response, httpResponse) = this.Post<object>(request, this.GetMethod(CodigoInterfazAutomatismoDb.CD_INTERFAZ_CODIGO_BARRAS));
                if (!httpResponse.IsSuccessStatusCode)
                {
                    string content = this.WaitAsync<string>(httpResponse.Content.ReadAsStringAsync());
                    _logger.LogError(content, "AutomatismoAutoStoreClientService - SendUbicacionesPicking");
                    throw new AutomatismoException("General_Error_Error_SincronizarAutomatismo");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AutomatismoAutoStoreClientService - SendUbicacionesPicking");
                throw new AutomatismoException("General_Error_Error_SincronizarAutomatismo");
            }
        }

        public virtual void SendProductos(ProductosAutomatismoRequest request)
        {
            try
            {

                var (response, httpResponse) = this.Post<object>(request, this.GetMethod(CodigoInterfazAutomatismoDb.CD_INTERFAZ_PRODUCTOS));
                if (!httpResponse.IsSuccessStatusCode)
                {
                    string content = this.WaitAsync<string>(httpResponse.Content.ReadAsStringAsync());
                    _logger.LogError(content, "AutomatismoAutoStoreClientService - SendUbicacionesPicking");
                    throw new AutomatismoException("General_Error_Error_SincronizarAutomatismo");

                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AutomatismoAutoStoreClientService - SendUbicacionesPicking");
                throw new AutomatismoException("General_Error_Error_SincronizarAutomatismo");
            }
        }

        public virtual void SendSalida(SalidaStockAutomatismoRequest request)
        {
            var (response, httpResponse) = this.Post<object>(request, this.GetMethod(CodigoInterfazAutomatismoDb.CD_INTERFAZ_SALIDA));
        }

        public virtual void SendUbicacionesPicking(UbicacionesPickingAutomatismoRequest request)
        {
            try
            {
                var (response, httpResponse) = this.Post<object>(request, this.GetMethod(CodigoInterfazAutomatismoDb.CD_INTERFAZ_UBICACIONES_PICKING));
                if (!httpResponse.IsSuccessStatusCode)
                {
                    string content = this.WaitAsync<string>(httpResponse.Content.ReadAsStringAsync());
                    _logger.LogError(content, "AutomatismoAutoStoreClientService - SendUbicacionesPicking");
                    throw new AutomatismoException("General_Error_Error_SincronizarAutomatismo");

                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AutomatismoAutoStoreClientService - SendUbicacionesPicking");
                throw new AutomatismoException("General_Error_Error_SincronizarAutomatismo");
            }
        }

        public virtual void SendReprocesar(int? cdInterfazExterna, string request)
        {
            var method = this.GetMethod(cdInterfazExterna);
            var (response, httpResponse) = this.Post<object>(request, method);
        }

        public virtual string GetMethod(int? cdInterfazExterna)
        {
            switch (cdInterfazExterna)
            {
                case CodigoInterfazAutomatismoDb.CD_INTERFAZ_PRODUCTOS: return "/Producto/Update";
                case CodigoInterfazAutomatismoDb.CD_INTERFAZ_CODIGO_BARRAS: return "/CodigoBarra/CreateUpdateOrDelete";
                case CodigoInterfazAutomatismoDb.CD_INTERFAZ_ENTRADAS: return "/Entrada/EntradaStock";
                case CodigoInterfazAutomatismoDb.CD_INTERFAZ_SALIDA: return "/Salida/SalidaStock";
                case CodigoInterfazAutomatismoDb.CD_INTERFAZ_UBICACIONES_PICKING: return "/UbicacionPicking/CreateUpdateOrDelete";
                case CodigoInterfazAutomatismoDb.CD_INTERFAZ_CONF_ENTRADAS: return "/ConfirmacionEntrada/Send";
                case CodigoInterfazAutomatismoDb.CD_INTERFAZ_CONF_SALIDAS: return "/ConfirmacionSalida/Send";
                case CodigoInterfazAutomatismoDb.CD_INTERFAZ_NOTIF_AJUSTES: return "/ConfirmacionAjusteStock/Send";
                case CodigoInterfazAutomatismoDb.CD_INTERFAZ_CONF_MOVIMIENTO: return "/ConfirmacionMovimiento/Send";
                default: throw new NotImplementedException("No implementa GetMethod");
            }
        }
    }
}
