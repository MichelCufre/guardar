using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using WIS.Automation.Galys;
using WIS.AutomationInterpreter.Models;
using WIS.Domain.Automatismo.Dtos;
using WIS.Domain.General;
using WIS.Domain.Integracion;

namespace WIS.AutomationInterpreter.Services
{
    public class AutomatismoClientService : ClientIntegrationService
    {
        public AutomatismoClientService(IOptions<IntegrationSettings> settings,
            IConfiguration configuration,
            ILogger<AutomatismoClientService> logger,
            IHttpContextAccessor httpContextAccessor) : base(logger, configuration, httpContextAccessor)
        {
            base.SetIntegration(new IntegracionServicio
            {
                Habilitado = settings.Value.Habilitado,
                Scope = settings.Value.Scope,
                Secret = settings.Value.Secret,
                TipoAutenticacion = settings.Value.TipoAutenticacion,
                TipoComunicacion = settings.Value.TipoComunicacion,
                UrlAuthServer = settings.Value.UrlAuthServer,
                UrlIntegracion = settings.Value.UrlIntegracion,
                User = settings.Value.User,
            });
        }

        protected virtual PtlCommandResponse ProccessErrorPtl(HttpResponseMessage httpResponse)
        {
            string content = this.WaitAsync<string>(httpResponse.Content.ReadAsStringAsync());

            return JsonConvert.DeserializeObject<PtlCommandResponse>(content);
        }

        public virtual PtlCommandResponse ConfirmCommand(PtlCommandConfirmRequest request)
        {
            var (response, httpResponse) = this.Post<PtlCommandResponse>(request, "/PtlConfirmation/ConfirmCommand");

            if (!httpResponse.IsSuccessStatusCode)
            {
                return this.ProccessErrorPtl(httpResponse);
            }

            return response;
        }

        protected virtual GalysResponse ProccessErrorAutostore(ValidationsResult response, HttpResponseMessage httpResponse)
        {
            throw new Exception(string.Join('.', response.GetErrors()));
        }

        protected virtual GalysResponse ProccessSuccessAutostore(ValidationsResult response, HttpResponseMessage httpResponse)
        {
            return new GalysResponse()
            {
                resultado = 0,
                descError = "Confirmación tratada correctamente",
            };
        }

        public virtual GalysResponse NotificarAjuste(NotificacionAjustesStockRequest request)
        {
            var (response, httpResponse) = this.Post<ValidationsResult>(request, "/NotificacionAjusteStock/Send");

            if (!httpResponse.IsSuccessStatusCode)
            {
                var task = httpResponse?.Content?.ReadAsStringAsync();

                task.Wait();

                var content = task.Result;
                var problemDetails = JsonConvert.DeserializeObject<ProblemDetails>(content);
                var result = JsonConvert.DeserializeObject<List<ValidationsError>>(problemDetails.Detail);

                return this.ProccessErrorAutostore(new ValidationsResult()
                {
                    Errors = result
                }, httpResponse);
            }

            return ProccessSuccessAutostore(response, httpResponse);
        }

        public virtual GalysResponse ConfirmarEntrada(ConfirmacionEntradaStockRequest request)
        {
            var (response, httpResponse) = this.Post<ValidationsResult>(request, "/ConfirmacionEntrada/Send");

            if (!httpResponse.IsSuccessStatusCode)
            {
                var task = httpResponse?.Content?.ReadAsStringAsync();

                task.Wait();

                var content = task.Result;
                var problemDetails = JsonConvert.DeserializeObject<ProblemDetails>(content);
                var result = JsonConvert.DeserializeObject<List<ValidationsError>>(problemDetails.Detail);

                return this.ProccessErrorAutostore(new ValidationsResult()
                {
                    Errors = result
                }, httpResponse);
            }

            return ProccessSuccessAutostore(response, httpResponse);
        }

        public virtual GalysResponse ConfirmarSalida(ConfirmacionSalidaStockRequest request)
        {
            var (response, httpResponse) = this.Post<ValidationsResult>(request, "/ConfirmacionSalida/Send");

            if (!httpResponse.IsSuccessStatusCode)
            {
                var task = httpResponse?.Content?.ReadAsStringAsync();

                task.Wait();

                var content = task.Result;
                var problemDetails = JsonConvert.DeserializeObject<ProblemDetails>(content);
                var result = JsonConvert.DeserializeObject<List<ValidationsError>>(problemDetails.Detail);

                return this.ProccessErrorAutostore(new ValidationsResult()
                {
                    Errors = result
                }, httpResponse);
            }

            return ProccessSuccessAutostore(response, httpResponse);
        }

        public virtual GalysResponse ConfirmarMovimiento(ConfirmacionMovimientoStockRequest request)
        {

            var (response, httpResponse) = this.Post<ValidationsResult>(request, "/ConfirmacionMovimiento/Send");

            if (!httpResponse.IsSuccessStatusCode)
            {
                var task = httpResponse?.Content?.ReadAsStringAsync();

                task.Wait();

                var content = task.Result;
                var problemDetails = JsonConvert.DeserializeObject<ProblemDetails>(content);
                var result = JsonConvert.DeserializeObject<List<ValidationsError>>(problemDetails.Detail);

                return this.ProccessErrorAutostore(new ValidationsResult()
                {
                    Errors = result
                }, httpResponse);
            }

            return ProccessSuccessAutostore(response, httpResponse);
        }
    }
}
