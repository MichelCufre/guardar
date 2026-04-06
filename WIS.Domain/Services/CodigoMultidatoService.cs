using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WIS.Common.API.Dtos;
using WIS.Domain.CodigoMultidato;
using WIS.Domain.CodigoMultidato.Interfaces;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Services.Interfaces;
using WIS.Security;

namespace WIS.Domain.Services
{
    public class CodigoMultidatoService : ICodigoMultidatoService
    {
        protected readonly IConfiguration _config;
        protected readonly IIdentityService _identity;
        protected readonly ICodigoMultidatoHandlerFactory _codigoMultidataHandlerFactory;

        public CodigoMultidatoService(
            IConfiguration config,
            IIdentityService identity,
            ICodigoMultidatoHandlerFactory codigoMultidataHandlerFactory)
        {
            _config = config;
            _identity = identity;
            _codigoMultidataHandlerFactory = codigoMultidataHandlerFactory;
        }

        public virtual async Task<CodigoMultidatoAIs> GetAIs(IUnitOfWork uow, string aplicacion, string vlCodigoMultidato, Dictionary<string, string> extraData = null, int? empresa = null, string cdCodigoMultidato = null, CodigoMultidatoTipoLectura tipoLectura = CodigoMultidatoTipoLectura.Producto)
        {
            var aisResultado = new Dictionary<string, object>();
            var habilitado = uow.ParametroRepository.GetParameter(ParamManager.CODIGO_MULTIDATO_HABILITADO) == "S";

            if (!habilitado)
                return null;

            var handler = _codigoMultidataHandlerFactory.GetHandler(uow, cdCodigoMultidato, vlCodigoMultidato, out Dictionary<string, string> aisCodigoMultidato);

            if (handler == null)
                return null;

            cdCodigoMultidato = handler.GetCodigo();

            if (aisCodigoMultidato == null)
                aisCodigoMultidato = handler.GetAIs(uow, vlCodigoMultidato);

            if (aisCodigoMultidato != null)
            {
                empresa = GetEmpresa(uow, empresa, handler, aisCodigoMultidato, tipoLectura);

                if (empresa == null)
                    return null;

                var habilitadoEmpresa = uow.EmpresaRepository.IsCodigoMultidatoHabilitado(empresa.Value, cdCodigoMultidato);

                if (!habilitadoEmpresa)
                    return null;

                var urlApi = uow.ParametroRepository.GetParameter(ParamManager.CODIGO_MULTIDATO_URL_API, new Dictionary<string, string>
                {
                    [ParamManager.PARAM_EMPR] = $"{ParamManager.PARAM_EMPR}_{empresa}",
                })?.Trim();

                if (!string.IsNullOrEmpty(urlApi))
                {
                    aisCodigoMultidato = await ConvertAIs(urlApi, aplicacion, cdCodigoMultidato, vlCodigoMultidato, aisCodigoMultidato, extraData);
                }

                var mapeos = GetMapeos(uow, aplicacion, cdCodigoMultidato, empresa.Value);
                var tiposAIs = GetTiposAIs(uow, cdCodigoMultidato, out Dictionary<string, decimal> conversiones);
                var fnc1 = uow.ParametroRepository.GetParameter(ParamManager.CODIGO_MULTIDATO_FNC1);

                foreach (var campo in mapeos.Keys)
                {
                    var aisMapeo = mapeos[campo];

                    foreach (var ai in aisMapeo)
                    {
                        var valor = handler.GetAIValue(aisCodigoMultidato, tiposAIs, conversiones, fnc1, ai);
                        if (valor != null)
                        {
                            aisResultado[campo] = valor;
                            break;
                        }
                    }
                }
            }

            return new CodigoMultidatoAIs
            { 
                CodigoMultidato = cdCodigoMultidato,
                Empresa = empresa.Value,
                Aplicacion = aplicacion,
                AIs = aisResultado,
            };
        }

        protected virtual Dictionary<string, string> GetTiposAIs(IUnitOfWork uow, string cdCodigoMultidato, out Dictionary<string, decimal> conversiones)
        {
            var resultado = new Dictionary<string, string>();

            conversiones = new Dictionary<string, decimal>();

            var detalles = uow.CodigoMultidatoRepository.GetDetallesCodigoMultidatoEmpresa(cdCodigoMultidato);

            foreach (var detalle in detalles)
            {
                resultado[detalle.CodigoAI] = detalle.TipoAI;

                if (detalle.Conversion.HasValue)
                    conversiones[detalle.CodigoAI] = detalle.Conversion.Value;
            }

            return resultado;
        }

        protected virtual int? GetEmpresa(IUnitOfWork uow, int? empresa, ICodigoMultidatoHandler helper, Dictionary<string, string> aisCodigoMultidato, CodigoMultidatoTipoLectura tipoLectura)
        {
            if (empresa == null)
            {
                empresa = uow.CodigoMultidatoRepository.GetFirstEmpresaHabilitada(helper.GetCodigo(), _identity.UserId, out int cantidad);

                if (cantidad != 1)
                    empresa = null;
            }

            empresa = helper.GetEmpresa(uow, _identity.UserId, aisCodigoMultidato, tipoLectura, empresa);

            return empresa;
        }

        protected virtual async Task<Dictionary<string, string>> ConvertAIs(string url, string aplicacion, string cdCodigoMultidato, string vlCodigoMultidato, Dictionary<string, string> ais, Dictionary<string, string> extraData)
        {
            try
            {
                var resultado = new Dictionary<string, string>();

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var address = new Uri($"{url}/MultidataCodeConverter/Convert");
                    var request = new HttpRequestMessage(HttpMethod.Post, address);
                    MultidataCodeConversionRequest transferObject = GetAPIRequest(aplicacion, cdCodigoMultidato, vlCodigoMultidato, ais, extraData);

                    request.Method = HttpMethod.Post;
                    request.Content = new StringContent(JsonConvert.SerializeObject(transferObject), Encoding.UTF8, "application/json");

                    request.Headers.ConnectionClose = true;

                    var internalTimeout = _config.GetSection("AppSettings:InternalTimeout")?.Value ?? "30";
                    client.Timeout = TimeSpan.FromMinutes(int.Parse(internalTimeout));

                    var response = await client.SendAsync(request, new CancellationToken());

                    if (!response.IsSuccessStatusCode)
                        throw new Exception("Error, status: " + response.StatusCode + " - " + response.ReasonPhrase + "-" + await response.Content.ReadAsStringAsync());

                    var responseContent = JsonConvert.DeserializeObject<MultidataCodeConversionResponse>(await response.Content.ReadAsStringAsync());

                    if (!string.IsNullOrEmpty(responseContent.Error))
                        throw new InvalidCodigoMultidatoException(responseContent.Error);

                    foreach (var mapeo in responseContent.AIs)
                    {
                        resultado[mapeo.AI] = mapeo.Value;
                    }
                }

                return resultado;
            }
            catch (InvalidCodigoMultidatoException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            { 
                throw new ConvertAIsCodigoMultidatoException(ex);
            }
        }

        protected virtual MultidataCodeConversionRequest GetAPIRequest(string aplicacion, string cdCodigoMultidato, string vlCodigoMultidato, Dictionary<string, string> ais, Dictionary<string, string> extraData)
        {
            return new MultidataCodeConversionRequest
            {
                AIs = GetAIsAPIRequest(ais),
                Application = aplicacion,
                Context = GetContextAPIRequest(extraData),
                MultidataCode = vlCodigoMultidato,
                MultidataCodeType = cdCodigoMultidato,
            };
        }

        protected virtual List<MutidataCodeConversionContextKeyValuePairRequest> GetContextAPIRequest(Dictionary<string, string> extraData)
        {
            var request = new List<MutidataCodeConversionContextKeyValuePairRequest>();

            if (extraData != null)
            {
                foreach (var key in extraData.Keys)
                {
                    request.Add(new MutidataCodeConversionContextKeyValuePairRequest
                    {
                        Key = key,
                        Value = extraData[key]
                    });
                }
            }

            return request;
        }

        protected virtual List<MutidataCodeConversionAIRequest> GetAIsAPIRequest(Dictionary<string, string> ais)
        {
            var request = new List<MutidataCodeConversionAIRequest>();

            if (ais != null)
            {
                foreach (var ai in ais.Keys)
                {
                    request.Add(new MutidataCodeConversionAIRequest
                    {
                        AI = ai,
                        Value = ais[ai]
                    });
                }
            }

            return request;
        }

        protected virtual Dictionary<string, List<string>> GetMapeos(IUnitOfWork uow, string aplicacion, string cdCodigoMultidato, int empresa)
        {
            var resultado = new Dictionary<string, List<string>>();
            var mapeos = uow.CodigoMultidatoRepository.GetDetallesCodigoMultidatoEmpresa(empresa, cdCodigoMultidato, aplicacion);

            foreach (var mapeo in mapeos)
            {
                if (!resultado.ContainsKey(mapeo.CodigoCampo))
                {
                    resultado[mapeo.CodigoCampo] = new List<string>();
                }

                resultado[mapeo.CodigoCampo].Add(mapeo.CodigoAI);
            }

            return resultado;
        }
    }
}
