using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using WIS.AutomationManager.Interfaces;
using WIS.Domain.Automatismo;
using WIS.Domain.Automatismo.Constants;
using WIS.Domain.Automatismo.Dtos;
using WIS.Domain.Automatismo.Interfaces;
using WIS.Domain.DataModel;
using WIS.Domain.General;
using WIS.Domain.Integracion;
using WIS.Domain.Integracion.Constants;

namespace WIS.AutomationManager.Services
{
    public class PtlInterpreterClientService : ClientIntegrationService, IPtlInterpreterClientService
    {
        public PtlInterpreterClientService(IUnitOfWorkFactory uowFactory,
            IConfiguration configuration,
            ILogger<PtlInterpreterClientService> logger,
            IHttpContextAccessor httpContextAccessor) : base(logger, configuration, httpContextAccessor)
        {
            this.SetHandleIntegration(() =>
            {
                using var uow = uowFactory.GetUnitOfWork();
                this.SetIntegration(uow.IntegracionServicioRepository.GetIntegrationByCodigo(IntegracionesDb.AutomationInterpreter));
            });
        }

        private ValidationsResult ProccessResultPtl(PtlCommandResponse response, HttpResponseMessage httpResponse)
        {
            var result = new ValidationsResult();

            if (!httpResponse.IsSuccessStatusCode)
            {
                string test = this.WaitAsync<string>(httpResponse.Content.ReadAsStringAsync());

                response = JsonConvert.DeserializeObject<PtlCommandResponse>(test);
            }

            if (response != null)
            {
                if (response.IsSuccess())
                {
                    result.SuccessMessage = response.Message;
                }
                else
                {
                    result.AddError(response.Error);
                }
            }

            return result;
        }

        public ValidationsResult TurnLigthOnOrOff(IPtl ptl, List<PtlPosicionEnUso> accion, bool isOn)
        {
            var interfazEnUso = ptl.GetInterfaz(CodigoInterfazAutomatismoDb.TrunOnLigth);
            var objContent = this.Map(ptl, accion, isOn);
            var interpretRequest = this.Map(interfazEnUso, objContent);
            var (response, httpResponse) = this.Post<PtlCommandResponse>(interpretRequest, "/Ptl/TurnLigthOnOrOff");
            var result = this.ProccessResultPtl(response, httpResponse);

            return result;
        }

        public ValidationsResult StartOfOperation(IPtl ptl)
        {
            var interfazEnUso = ptl.GetInterfazEnUso();
            var objContent = this.Map(ptl);
            var interpretRequest = this.Map(interfazEnUso, objContent);
            var (response, httpResponse) = this.Post<PtlCommandResponse>(interpretRequest, "/Ptl/StartOfOperation");
            var result = this.ProccessResultPtl(response, httpResponse);

            return result;
        }

        public ValidationsResult ResetOfOperation(IPtl ptl)
        {
            var interfazEnUso = ptl.GetInterfazEnUso();
            var objContent = this.Map(ptl);
            var interpretRequest = this.Map(interfazEnUso, objContent);
            var (response, httpResponse) = this.Post<PtlCommandResponse>(interpretRequest, "/Ptl/ResetOfOperation");
            var result = this.ProccessResultPtl(response, httpResponse);

            return result;
        }

        private PtlCommandStartRequest Map(IPtl ptl)
        {
            return new PtlCommandStartRequest
            {
                Id = ptl.Codigo,
            };
        }

        private List<PtlCommandRequest> Map(IPtl ptl, List<PtlPosicionEnUso> accion, bool isOn)
        {
            var commands = new List<PtlCommandRequest>();

            foreach (var item in accion)
            {
                commands.Add(new PtlCommandRequest
                {
                    Address = ptl.GetPosicion(item.Ubicacion).PosicionExterna,
                    Color = item.Color,
                    Id = ptl.Codigo,
                    CommandId = item.Id,
                    Text = item.Display,
                    TextFn = item.DisplayFn,
                    CommandType = (isOn) ? PtlTipoComandoDb.PrenderLuz : PtlTipoComandoDb.ApagarLuz
                });
            }

            return commands;
        }
    }
}
