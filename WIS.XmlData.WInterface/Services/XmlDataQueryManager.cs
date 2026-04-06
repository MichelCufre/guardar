using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Web;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General.API.Dtos.Salida;
using WIS.Domain.Validation;
using WIS.Persistence.Database;
using WIS.XmlData.WInterface.Helpers;
using WIS.XmlData.WInterface.Models;
using WIS.XmlData.WInterface.Services.Interfaces;

namespace WIS.XmlData.WInterface.Services
{
    public class XmlDataQueryManager : IXmlDataQueryManager
    {
        protected ApiMapper _mapper;
        protected readonly IConfiguration _configuration;

        protected List<int> CodigosInterfazConsulta { get; set; }

        public XmlDataQueryManager(IConfiguration configuration)
        {
            _configuration = configuration;

            Initialize();
        }

        public async Task<string> GetXmlData(WISDB context, string token, string loginName, int interfazExterna, int empresa, long nuEjecucion)
        {
            var userId = GetUserId(context, loginName);
            var interfaces_habilitadas = GetInterfacesHabilitadas(context, userId);

            if (InterfazHabilitada(interfaces_habilitadas, empresa, interfazExterna))
            {
                var data = context.T_INTERFAZ_EJECUCION_DATA.FirstOrDefault(x => x.NU_INTERFAZ_EJECUCION == nuEjecucion).DATA;
                var str_xml = ByteString.GetString(data);
                var wmsApiEndpoint = _configuration.GetValue<string>("WmsApiSettings:Endpoint");

                switch (interfazExterna)
                {
                    case CInterfazExterna.ConsultaDeStock:
                        return await GetConsultaStock(wmsApiEndpoint, token, empresa, str_xml);
                    default:
                        throw new NotImplementedException();
                }
            }
            else
            {
                throw new XDInterfazException(XDCDError.EMPRESA_INTERFAZ);
            }
        }

        protected async Task<string> GetConsultaStock(string baseUri, string token, int empresa, string xml)
        {
            var requests = _mapper.MapConsultaStockRequests(empresa, xml);
            var uriString = baseUri + "/ConsultaDeStock/GetData";
            var stock = new List<StockResponse>();

            foreach (var request in requests)
            {
                try
                {
                    while (true)
                    {
                        var response = await InvokeAsync<ConsultaStockResponse>(uriString, HttpMethod.Post, token, request);
                        stock.AddRange(response.Stock);
                        request.Pagina++;
                    }
                }
                catch (ApiResponseException ex)
                {
                    var problemDetails = ex.ProblemDetails;

                    if (problemDetails.Detail?.Contains(ValidationMessage.WMSAPI_msg_Error_StockNoEncontrado) ?? false)
                    {
                        continue;
                    }
                    else 
                    {
                        throw ex;
                    }
                }
            }

            return _mapper.MapConsultaStockResponse(new ConsultaStockResponse { Stock = stock });
        }

        protected virtual void Initialize()
        {
            _mapper = new ApiMapper();

            CodigosInterfazConsulta = new List<int>()
            {
                CInterfazExterna.ConsultaDeStock,
            };
        }

        protected virtual int GetUserId(WISDB context, string loginName)
        {
            return context.USERS.FirstOrDefault(u => u.LOGINNAME.ToLower().Trim() == loginName.ToLower().Trim()).USERID;
        }

        protected virtual Dictionary<int, HashSet<int>> GetInterfacesHabilitadas(WISDB context, int userId)
        {
            var result = new Dictionary<int, HashSet<int>>();

            var parametros = CodigosInterfazConsulta
                .Select(c => $"IC_{c}_HABILITADA");

            var entidades = context.T_EMPRESA_FUNCIONARIO
                .AsNoTracking()
                .Where(ef => ef.USERID == userId)
                .Select(ef => $"{Helpers.ParamManager.PARAM_EMPR}_{ef.CD_EMPRESA}")
                .ToList();

            var configuraciones = context.T_LPARAMETRO_CONFIGURACION
                .AsNoTracking()
                .Where(c => parametros.Contains(c.CD_PARAMETRO)
				    && c.DO_ENTIDAD_PARAMETRIZABLE == Helpers.ParamManager.PARAM_EMPR
                    && c.VL_PARAMETRO == "S")
                .ToList();

            configuraciones = configuraciones
                .Where(c => entidades.Contains(c.ND_ENTIDAD))
                .ToList();

            foreach (var c in configuraciones)
            {
                var empresa = int.Parse(c.ND_ENTIDAD.Replace($"{Helpers.ParamManager.PARAM_EMPR}_", ""));
                var interfaz = int.Parse(c.CD_PARAMETRO.Replace("IC_", "").Replace("_HABILITADA", ""));

                if (!result.ContainsKey(empresa))
                {
                    result[empresa] = new HashSet<int>();
                }

                if (!result[empresa].Contains(interfaz))
                {
                    result[empresa].Add(interfaz);
                }
            }

            return result;
        }

        protected virtual bool InterfazHabilitada(Dictionary<int, HashSet<int>> interfaceHabilitadas, int empresa, int interfazExterna)
        {
            return interfaceHabilitadas.ContainsKey(empresa) && interfaceHabilitadas[empresa].Contains(interfazExterna);
        }

        protected virtual async Task<T> InvokeAsync<T>(string uriString, HttpMethod method, string token, object request)
        {
            var uriBuilder = new UriBuilder(uriString);
            var jsonContent = string.Empty;
            var jsonSerializeOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };

            if (method == HttpMethod.Get)
            {
                if (request != null)
                {
                    var properties = from p in request.GetType().GetProperties()
                                     where p.GetValue(request, null) != null
                                     select p.Name + "=" + HttpUtility.UrlEncode(p.GetValue(request, null)?.ToString());

                    uriBuilder.Query = string.Join("&", properties);
                }
            }
            else
            {
                jsonContent = JsonSerializer.Serialize(request);
            }

            var requestUri = uriBuilder.Uri;

            using (var client = new HttpClient())
            using (var requestMessage = new HttpRequestMessage(method, requestUri))
            {
                client.Timeout = TimeSpan.FromMinutes(30);

                if (!string.IsNullOrEmpty(jsonContent))
                {
                    requestMessage.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                }

                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                requestMessage.Headers.ConnectionClose = true;

                using (var responseMessage = await client.SendAsync(requestMessage))
                {
                    var detail = await responseMessage.Content.ReadAsStringAsync();

                    if (responseMessage.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        return JsonSerializer.Deserialize<T>(detail, jsonSerializeOptions);
                    }
                    else if (responseMessage.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        throw new UnauthorizedAccessException();
                    }
                    else
                    {
                        throw new ApiResponseException(JsonSerializer.Deserialize<ProblemDetails>(detail, jsonSerializeOptions));
                    }
                }
            }
        }
    }
}
