using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Http;
using WIS.Domain.Automatismo;
using WIS.Domain.Automatismo.Dtos;
using WIS.Domain.DataModel;
using WIS.Domain.General;
using WIS.Domain.Integracion;
using WIS.Domain.Integracion.Constants;
using WIS.Domain.Services.Interfaces;

namespace WIS.Domain.Services
{
    public class AutomatismoPtlClientService : ClientIntegrationService, IAutomatismoPtlClientService
    {
        protected readonly int _internalTimeout;

        public AutomatismoPtlClientService(IUnitOfWorkFactory uowFactory,
            ILogger<AutomatismoPtlClientService> logger,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor) : base(logger, configuration, httpContextAccessor)
        {
            this.SetHandleIntegration(() =>
            {
                using var uow = uowFactory.GetUnitOfWork();
                this.SetIntegration(uow.IntegracionServicioRepository.GetIntegrationByCodigo(IntegracionesDb.AutomationManager));
            });
        }

        public virtual ValidationsResult ProccessResult(HttpResponseMessage httpResponse)
        {
            var result = new ValidationsResult();

			if (!httpResponse.IsSuccessStatusCode)
			{
				string content = this.WaitAsync<string>(httpResponse.Content.ReadAsStringAsync());

				ProblemDetails problem = JsonConvert.DeserializeObject<ProblemDetails>(content);

				if (!string.IsNullOrEmpty(problem.Title))
					result.AddError(problem.Title);

				if (!string.IsNullOrEmpty(problem.Detail))
					result.Errors = JsonConvert.DeserializeObject<List<ValidationsError>>(problem.Detail);
			}

			return result;
        }

        public (ValidationsResult, PtlColorResponse) GetPtlColor(int userId, string zona)
        {
            var (response, httpResponse) = this.Post<PtlColorResponse>(new
            {
                UserId = userId,
                Ptl = zona
            }, "/Ptl/GetColor");

            return (this.ProccessResult(httpResponse), response);

        }

        public (ValidationsResult, string) PrenderLuces(int userId, string codigoPtl, string codigoColor, int empresa, string detail, string referencia, string agrupacion)
        {
            var (response, httpResponse) = this.Post<string>(new
            {
                UserId = userId,
                Ptl = codigoPtl,
                Color = codigoColor,
                Company = empresa,
                Referencia = referencia,
                Agrupacion = agrupacion,
                Detail = detail
            }, "/Ptl/TrunOnLigth");

            return (this.ProccessResult(httpResponse), response);

        }

        public (ValidationsResult, List<PtlCommandConfirmRequest>) GetLightsOnByPtl(string cdZonaUbicacion)
        {
            var content = new NameValueCollection();
            content.Add("ptl", cdZonaUbicacion);

            var (response, httpResponse) = this.Get<List<PtlCommandConfirmRequest>>(content, "/Ptl/GetLightsOnByPtl");

            return (this.ProccessResult(httpResponse), response);
        }

        public (ValidationsResult, bool) ValidatePtlReferencia(string cdZonaUbicacion, string referencia)
        {
            var (response, httpResponse) = this.Post<bool>(new
            {
                Ptl = cdZonaUbicacion,
                Referencia = referencia
            }, "/Ptl/ValidatePtlReferencia");

            return (this.ProccessResult(httpResponse), response);
        }

        public (ValidationsResult, bool) ValidarOperacion(string cdZonaUbicacion, string color, int empresa, string producto)
        {
            var (response, httpResponse) = this.Post<bool>(new
            {
                Ptl = cdZonaUbicacion,
                Product = producto,
                Company = empresa,
                Color = color,
            }, "/Ptl/ValidarOperacion");

            return (this.ProccessResult(httpResponse), response);
        }

        public (ValidationsResult, string) GetPtlByTipoAutomatismo(string tipoPTL)
        {
            var content = new NameValueCollection();
            content.Add("tipoPTL", tipoPTL);

            var (response, httpResponse) = this.Get<string>(content, "/Ptl/GetPtlByTipoAutomatismo");

            return (this.ProccessResult(httpResponse), response);
        }

        public (ValidationsResult, List<PtlCommandConfirmRequest>) GetColoresActivosRecords(string ptl)
        {
            var content = new NameValueCollection();
            content.Add("ptl", ptl);

            var (response, httpResponse) = this.Get<List<PtlCommandConfirmRequest>>(content, "/Ptl/GetLightsOnByPtl");

            return (this.ProccessResult(httpResponse), response);
        }

        public (ValidationsResult, List<PtlColorResponse>) GetColoresActivosByPtl(string ptl)
        {
            var (response, httpResponse) = this.Post<List<PtlColorResponse>>(new
            {
                ptl = ptl
            }, "/Ptl/GetColors");

            return (this.ProccessResult(httpResponse), response);
        }

        public (ValidationsResult, bool) ClearColor(string zona, string color, int userId)
        {
            var (response, httpResponse) = this.Post<bool>(new
            {
                Ptl = zona,
                UserId = userId,
                Color = color
            }, "/Ptl/ClearColor");

            return (this.ProccessResult(httpResponse), response);
        }

        public (ValidationsResult, bool) FinishOperation(string zona, string color, int userId)
        {
            var (response, httpResponse) = this.Post<bool>(new
            {
                Ptl = zona,
                UserId = userId,
                Color = color
            }, "/Ptl/FinishOperation");

            return (this.ProccessResult(httpResponse), response);
        }

        public (ValidationsResult, bool) UpdateLuzByPtlColor(PtlColorActivoRequest colorActivo)
        {
            var (response, httpResponse) = this.Post<bool>(colorActivo, "/Ptl/UpdateLuzByPtlColor");

            return (this.ProccessResult(httpResponse), response);
        }
    }
}
