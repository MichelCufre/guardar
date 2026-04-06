using IdentityModel.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WIS.Configuration;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Impresiones;
using WIS.Domain.Impresiones.Dtos;
using WIS.Domain.Services.Interfaces;

namespace WIS.Domain.Services
{
    public class PrintingService : IPrintingService
    {
        protected readonly IOptions<PrintingSettings> _printingSettings;
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IHttpClientFactory _httpClientFactory;
        protected readonly ILogger<PrintingService> _logger;
        protected readonly IOptions<AuthSettings> _authSettings;
        protected readonly IHttpContextAccessor _httpContextAccessor;

        public PrintingService(IUnitOfWorkFactory uowFactory,
            IHttpClientFactory httpClientFactory,
            ILogger<PrintingService> logger,
            IOptions<PrintingSettings> printingSettings,
            IOptions<AuthSettings> authSettings,
            IHttpContextAccessor httpContextAccessor)
        {
            this._uowFactory = uowFactory;
            this._httpClientFactory = httpClientFactory;
            this._logger = logger;
            this._printingSettings = printingSettings;
            this._authSettings = authSettings;
            this._httpContextAccessor = httpContextAccessor;
        }

        public virtual string GetEstadoInicial()
        {
            if (_printingSettings.Value?.IsEnabled ?? false)
                return EstadoImpresionDb.EnvioInmediato;

            return EstadoImpresionDb.Enviado;
        }

        public virtual void SendToPrint(int nuImpresion)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                SendToPrint(uow, nuImpresion);
            }
        }

        public virtual void SendToPrint(IUnitOfWork uow, int nuImpresion)
        {
            if ((_printingSettings.Value?.IsEnabled ?? false) && nuImpresion > 0)
            {
                var impresion = uow.ImpresionRepository.GetImpresionWithDetail(nuImpresion);
                var task = SendToPrintAsync(uow, impresion, CancellationToken.None);
                task.Wait();
            }
        }

        public virtual async Task SendToPrintAsync(IUnitOfWork uow, int nuImpresion)
        {
            if ((_printingSettings.Value?.IsEnabled ?? false) && nuImpresion > 0)
            {
                var impresion = uow.ImpresionRepository.GetImpresionWithDetail(nuImpresion);
                await SendToPrintAsync(uow, impresion, CancellationToken.None);
            }
        }

        public virtual async Task SendToPrintAsync(IUnitOfWork uow, Impresion impresion, CancellationToken cancellationToken)
        {
            ImpresoraServidor impresora = await uow.ImpresoraRepository.GetImpresora(impresion.CodigoImpresora);

            if (impresora == null)
            {
                var error = $"Impresora {impresion.CodigoImpresora} desconocida";
                this._logger.LogError($"Error al procesar la impresión {impresion.Id}. {error}");
                impresion.SetEstadoError(error);
                await uow.ImpresionRepository.UpdateImpresion(impresion);
            }
            else
            {
                foreach (var detalle in impresion.Detalles.OrderBy(s => s.Registro))
                {
                    try
                    {
                        await this.SendRequest(impresion, detalle, impresora, uow, cancellationToken);
                        detalle.SetEstadoRecibido();
                    }
                    catch (Exception ex)
                    {
                        this._logger.LogError(ex, $"Error al enviar la solicitud de impresión {impresion.Id}, registro {detalle.Registro}.");
                        detalle.SetEstadoError(ex.Message);
                    }

                    await uow.ImpresionRepository.UpdateDetalleImpresion(impresion, detalle);
                }

                if (impresion.IsTodoRecibido())
                {
                    impresion.SetEstadoRecibido();
                    await uow.ImpresionRepository.UpdateImpresion(impresion);
                }
                else if (impresion.AnyError())
                {
                    impresion.SetEstadoError("Error al enviar la solicitud de impresión. Consulte el estado de los detalles asociados");
                    await uow.ImpresionRepository.UpdateImpresion(impresion);
                }
            }
        }

        protected virtual async Task SendRequest(Impresion impresion, DetalleImpresion detalle, ImpresoraServidor impresora, IUnitOfWork uow, CancellationToken cancellationToken)
        {
            var contenido = detalle.Contenido;
            var prefix = "reporte:///";
            var isReport = false;

            if (contenido.StartsWith(prefix))
            {
                isReport = true;

                var nuReporte = long.Parse(contenido.Substring(prefix.Length));
                var reporte = uow.ReporteRepository.GetReporte(nuReporte);

                contenido = "json:///" + JsonConvert.SerializeObject(new
                {
                    FileName = reporte.NombreArchivo,
                    Content = reporte.Contenido
                });
            }

            var impresionRequest = new ImpresionRequest
            {
                Id = impresion.Id.ToString(),
                Number = Convert.ToString(detalle.Registro),
                User = impresion.Usuario?.ToString(),
                PrinterAddress = @$"\\{impresora.Address}\{impresora.Id}",
                Client = impresora.ClientId,
                PrintData = contenido
            };


            if (isReport)
                impresionRequest.PrinterAddress = impresora.Id;

            var token = await this.GetToken(cancellationToken);

            using (var client = this._httpClientFactory.CreateClient())
            using (var request = new HttpRequestMessage(HttpMethod.Post, this._printingSettings.Value.Endpoint))
            {
                request.Content = new StringContent(JsonConvert.SerializeObject(impresionRequest), Encoding.UTF8, "application/json");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                request.Headers.ConnectionClose = true;

                var response = await client.SendAsync(request, cancellationToken);
                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    throw new InvalidOperationException($"Error al enviar la solicitud de impresión: {response.ReasonPhrase}. Detalle: {content}");
            }
        }

        protected virtual async Task<string> GetToken(CancellationToken cancellationToken)
        {
            var estado = $"Obteniendo token de acceso...";

            this._logger.LogDebug(estado);

            var access_token = _httpContextAccessor.HttpContext?.Request?.Headers["access_token"][0];

            if (!string.IsNullOrEmpty(access_token))
            {
                estado = "Autenticado";
                this._logger.LogDebug(estado);
                return access_token;
            }
            else
            {
                using (var client = new HttpClient())
                {
                    var response = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
                    {
                        Address = this._authSettings.Value.TokenUrl,
                        ClientId = this._authSettings.Value.ClientId,
                        ClientSecret = this._authSettings.Value.ClientSecret,
                        Scope = this._authSettings.Value.AccessScope,
                    }, cancellationToken);

                    if (response.IsError)
                    {
                        estado = $"Ocurrió un error al obtener el token de acceso: {response.Error}";

                        this._logger.LogError(estado);

                        throw new InvalidOperationException(estado);
                    }
                    else
                    {
                        estado = "Autenticado";
                        this._logger.LogDebug(estado);
                    }

                    return response.AccessToken;
                }
            }
        }
    }
}
