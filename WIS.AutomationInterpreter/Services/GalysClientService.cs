using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Text.Json;
using WIS.Automation;
using WIS.AutomationInterpreter.Interfaces;
using WIS.AutomationInterpreter.Models.Mappers.Interfaces;
using WIS.Domain.Automatismo.Dtos;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Integracion;
using WIS.Domain.Integracion.Dtos;

namespace WIS.AutomationInterpreter.Services
{
    public class GalysClientService : ClientIntegrationService, IAutoStoreClientService
    {
        protected IGalysMapper _mapper;

        public GalysClientService(ILogger logger,
            IConfiguration configuration,
            IGalysMapper mapper,
            IHttpContextAccessor httpContextAccessor) : base(logger, configuration, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public virtual AutomatismoResponse SendProductos(AutomatismoInterpreterRequest request)
        {
            var result = new AutomatismoResponse();
            var contenido = JsonSerializer.Deserialize<ProductosAutomatismoRequest>(request.IntegracionServicioConexion.Contenido);

            this.SetIntegration(Map(request.IntegracionServicio));

            foreach (var producto in contenido.Productos)
            {
                result = SendProducto(request.IntegracionServicioConexion, producto);

                if (result.IsError)
                    break;
            }

            return result;
        }

        public virtual AutomatismoResponse SendProducto(IntegracionServicioConexionRequest conexion, ProductoAutomatismoRequest request)
        {
            var result = new AutomatismoResponse();
            var contenido = JsonSerializer.Serialize(_mapper.Map(request));
            var (response, httpResponse) = this.SendParse<Automation.Galys.GalysResponse>(conexion.ProtocoloComunicacion, contenido, conexion.Url);

            ProcessResponse(result, response, httpResponse);

            return result;
        }

        public virtual AutomatismoResponse SendCodigosBarras(AutomatismoInterpreterRequest request)
        {
            var result = new AutomatismoResponse();
            var contenido = JsonSerializer.Deserialize<CodigosBarrasAutomatismoRequest>(request.IntegracionServicioConexion.Contenido);

            this.SetIntegration(Map(request.IntegracionServicio));

            foreach (var codigoBarra in contenido.CodigosDeBarras)
            {
                result = SendCodigoBarra(request.IntegracionServicioConexion, codigoBarra);

                if (result.IsError)
                    break;
            }

            return result;
        }

        public virtual AutomatismoResponse SendCodigoBarra(IntegracionServicioConexionRequest conexion, CodigoBarraAutomatismoRequest request)
        {
            var result = new AutomatismoResponse();

            var codigoBarrasGalysRequest = _mapper.Map(request);

            if (request.TipoOperacion == TipoOperacionDb.Baja)
            {
                Dictionary<string, string> urlParams = new Dictionary<string, string>
                    {
                        { "codAlmacen", codigoBarrasGalysRequest.codAlmacen },
                        { "codArticulo", codigoBarrasGalysRequest.codArticulo },
                        { "codBarras", codigoBarrasGalysRequest.codBarras },
                    };

                var (response, httpResponse) = this.Delete<Automation.Galys.GalysResponse>(urlParams, conexion.Url);
                ProcessResponse(result, response, httpResponse);
            }
            else
            {
                var contenido = JsonSerializer.Serialize(codigoBarrasGalysRequest);
                var (response, httpResponse) = this.SendParse<Automation.Galys.GalysResponse>(conexion.ProtocoloComunicacion, contenido, conexion.Url);
                ProcessResponse(result, response, httpResponse);
            }
            return result;
        }

        public virtual AutomatismoResponse SendEntrada(AutomatismoInterpreterRequest request)
        {
            var result = new AutomatismoResponse();
            var contenido = JsonSerializer.Deserialize<EntradaStockAutomatismoRequest>(request.IntegracionServicioConexion.Contenido);
            var detalles = new Dictionary<string, List<EntradaStockLineaAutomatismoRequest>>();

            foreach (var det in contenido.Detalles)
            {
                var key = $"{det.TipoAgente}.{det.CodigoAgente}";

                if (!detalles.ContainsKey(key))
                    detalles[key] = new List<EntradaStockLineaAutomatismoRequest>();

                detalles[key].Add(det);
            }

            this.SetIntegration(Map(request.IntegracionServicio));

            foreach (var key in detalles.Keys)
            {
                result = SendEntrada(request.IntegracionServicioConexion, contenido, detalles[key]);

                if (result.IsError)
                    break;
            }

            return result;
        }

        public virtual AutomatismoResponse SendEntrada(IntegracionServicioConexionRequest conexion, EntradaStockAutomatismoRequest cabezal, List<EntradaStockLineaAutomatismoRequest> detalles)
        {
            var result = new AutomatismoResponse();
            var contenido = JsonSerializer.Serialize(_mapper.Map(cabezal, detalles));
            var (response, httpResponse) = this.SendParse<Automation.Galys.GalysResponse>(conexion.ProtocoloComunicacion, contenido, conexion.Url);

            ProcessResponse(result, response, httpResponse);

            return result;
        }

        public virtual AutomatismoResponse SendSalida(AutomatismoInterpreterRequest request)
        {
            var result = new AutomatismoResponse();
            var contenido = JsonSerializer.Deserialize<SalidaStockAutomatismoRequest>(request.IntegracionServicioConexion.Contenido);
            var detalles = new Dictionary<string, List<SalidaStockLineaAutomatismoRequest>>();

            this.SetIntegration(Map(request.IntegracionServicio));

            result = SendSalida(request.IntegracionServicioConexion, contenido);

            return result;
        }

        public virtual AutomatismoResponse SendSalida(IntegracionServicioConexionRequest conexion, SalidaStockAutomatismoRequest cabezal)
        {
            var result = new AutomatismoResponse();
            var contenido = JsonSerializer.Serialize(_mapper.Map(cabezal));
            var (response, httpResponse) = this.SendParse<Automation.Galys.GalysResponse>(conexion.ProtocoloComunicacion, contenido, conexion.Url);

            ProcessResponse(result, response, httpResponse);

            return result;
        }

        public virtual void ProcessResponse(AutomatismoResponse result, Automation.Galys.GalysResponse response, System.Net.Http.HttpResponseMessage httpResponse)
        {
            if (!httpResponse.IsSuccessStatusCode)
            {
                if (response != null)
                {
                    result.SetError(response.descError);
                    result.Operacion = response.resultado;
                }
                else if (httpResponse.Content != null)
                {
                    var contenido = httpResponse.Content.ReadAsStringAsync().Result;
                    response = JsonSerializer.Deserialize<Automation.Galys.GalysResponse>(contenido);

                    if (response != null)
                    {
                        result.SetError(response.descError);
                        result.Operacion = response.resultado;
                    }
                    else
                        result.SetError("Error no controlado");

                }
                else
                {
                    result.SetError("Error no controlado");
                }
            }
            else
            {
                if (response != null)
                {
                    result.Operacion = response.resultado;

                    if (response.resultado == 0)
                        result.Mensaje = response.descError;
                    else
                        result.SetError(response.descError);
                }
            }
        }

        public virtual IntegracionServicio Map(IntegracionServicioConfigRequest request)
        {
            return new IntegracionServicio
            {
                Habilitado = request.Habilitado,
                Scope = request.Scope,
                Secret = request.Secret,
                TipoAutenticacion = request.TipoAutenticacion,
                TipoComunicacion = request.TipoComunicacion,
                UrlAuthServer = request.UrlAuthServer,
                UrlIntegracion = request.UrlIntegracion,
                User = request.User,
            };
        }
    }
}
