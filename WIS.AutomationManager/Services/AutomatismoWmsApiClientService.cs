using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using WIS.AutomationManager.Interfaces;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General;
using WIS.Domain.General.API.Dtos.Entrada;
using WIS.Domain.General.Enums;
using WIS.Domain.Integracion;
using WIS.Domain.Integracion.Constants;

namespace WIS.AutomationManager.Services
{
    public class AutomatismoWmsApiClientService : ClientIntegrationService, IAutomatismoWmsApiClientService
    {
        public AutomatismoWmsApiClientService(IUnitOfWorkFactory uowFactory,
            ILogger<AutomatismoWmsApiClientService> logger,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor) : base(logger, configuration, httpContextAccessor)
        {
            this.SetHandleIntegration(() =>
            {
                using var uow = uowFactory.GetUnitOfWork();
                this.SetIntegration(uow.IntegracionServicioRepository.GetIntegrationByCodigo(IntegracionesDb.WMSAPI));
            });
        }

        private ValidationsResult ProccessResult(EntradaResponse response, HttpResponseMessage httpResponse)
        {
            var result = new ValidationsResult();

            if (!httpResponse.IsSuccessStatusCode)
            {
                var content = this.WaitAsync<string>(httpResponse.Content.ReadAsStringAsync());

                if (httpResponse.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    result.AddError(string.IsNullOrEmpty(content) ? "WMSAPI - Unauthorized" : content);
                }
                else
                {
                    var problem = JsonConvert.DeserializeObject<ProblemDetails>(content);

                    result.AddError(problem.Title);

                    var errors = JsonConvert.DeserializeObject<List<ValidationsError>>(problem.Detail);

                    if (errors != null)
                    {
                        var item = 0;
                        errors.ForEach(err =>
                        {
                            result.AddError(err.Messages, item);
                            item++;
                        });
                    }
                }
            }

            if (response != null)
            {
                result.SuccessMessage = response.NumeroInterfaz.ToString();
            }

            return result;
        }

        public ValidationsResult SepararProductoPreparacion(int userId, int preparacion, int contenedorOrigen, int contenedorDestino, string ubicacion, int empresa, string producto, decimal cantidadIngresada, int? tipoAgrupacion, string comparteAgrupacion, string tipoEtiqueta)
        {

            var (response, httpResponse) = this.Post<EntradaResponse>(new
            {
                DsReferencia = "PTL",
                UserId = userId,
                Preparacion = preparacion,
                ContenedorOrigen = contenedorOrigen,
                ContenedorDestino = contenedorDestino,
                EnderecoDestino = ubicacion,
                SituacionDestino = EstadoContenedor.Contabilizado,
                Empresa = empresa,
                Producto = producto,
                Cantidad = cantidadIngresada,
                TipoAgrupacion = tipoAgrupacion,
                ComparteAgrupacion = comparteAgrupacion,
                TipoEtiquetaDestino = tipoEtiqueta,
            }, "/Preparacion/SepararPicking");

            return this.ProccessResult(response, httpResponse);
        }

        public ValidationsResult SepararCrossDocking(int userId, int preparacion, int agenda, string codigoAgente, string tipoAgente, int contenedorDestino, string ubicacion, int empresa, string producto, string lote, decimal cantidadIngresada, string tipoEtiqueta)
        {
            var (response, httpResponse) = this.Post<EntradaResponse>(new
            {
                DsReferencia = "PTL",
                UserId = userId,
                Preparacion = preparacion,
                Agenda = agenda,
                CodigoAgente = codigoAgente,
                TipoAgente = tipoAgente,
                IdExternoContenedor = contenedorDestino.ToString(),
                SituacionDestino = EstadoContenedor.Contabilizado,
                Ubicacion = ubicacion,
                Empresa = empresa,
                Producto = producto,
                Identificador = lote,
                Cantidad = cantidadIngresada,
                TipoContenedor = tipoEtiqueta,
            }, "/CrossDocking/CrossDockingUnaFase");

            return this.ProccessResult(response, httpResponse);

        }

        public ValidationsResult CambiarContenedor(int contenedorOrigen, int userId, string cdEndereco, int contenedorDestino, int tipoOperacion, int? tipoAgrupacion, string comparteAgrupacion, string tipoEtiqueta)
        {
            var (response, httpResponse) = this.Post<EntradaResponse>(new
            {
                DsReferencia = "PTL",
                UserId = userId,
                ContenedorOrigen = contenedorOrigen,
                ContenedorDestino = contenedorDestino,
                UbicacionDestino = cdEndereco,
                TipoOperacion = tipoOperacion,
                SituacionOrigen = EstadoContenedor.Contabilizado,
                TipoAgrupacion = tipoAgrupacion,
                ComparteAgrupacion = comparteAgrupacion,
                TipoEtiquetaDestino = tipoEtiqueta,
            }, "/Preparacion/CambiarContenedor");

            return this.ProccessResult(response, httpResponse);
        }

        public ValidationsResult Picking(List<DetallePickingRequest> detalles, int empresa)
        {
            var (response, httpResponse) = this.Post<EntradaResponse>(new
            {
                Detalles = detalles,
                Estado = EstadoDetallePreparacion.ESTADO_PENDIENTE_AUTO,
                Empresa = empresa,
                DsReferencia = "Picking",
                Archivo = "Picking"
            }, "/Preparacion/Picking");

            return this.ProccessResult(response, httpResponse);
        }

        public ValidationsResult NotificarAjuste(AjustesDeStockRequest ajuste)
        {
            var (response, httpResponse) = this.Post<EntradaResponse>(ajuste, "/Stock/Ajustar");
            return this.ProccessResult(response, httpResponse);
        }

        public ValidationsResult ConfirmarEntrada(TransferenciaStockRequest entrada)
        {
            var (response, httpResponse) = this.Post<EntradaResponse>(entrada, "/Stock/Transferir");
            return this.ProccessResult(response, httpResponse);
        }

        public ValidationsResult ConfirmarSalida(PickingRequest salida)
        {
            var (response, httpResponse) = this.Post<EntradaResponse>(salida, "/Preparacion/Picking");
            return this.ProccessResult(response, httpResponse);
        }

        public ValidationsResult ConfirmarAnulacionesPendientes(AnularPickingPedidoPendienteRequest anulaciones)
        {
            var (response, httpResponse) = this.Post<EntradaResponse>(anulaciones, "/Preparacion/AnularPickingPedidoPendienteAutomatismo");
            return this.ProccessResult(response, httpResponse);
        }
        public ValidationsResult ConfirmarMovimiento(TransferenciaStockRequest entrada)
        {
            var (response, httpResponse) = this.Post<EntradaResponse>(entrada, "/Stock/MovimientoStockAutomatismo");
            return this.ProccessResult(response, httpResponse);
        }
    }
}
