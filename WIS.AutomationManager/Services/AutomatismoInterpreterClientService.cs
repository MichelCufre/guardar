using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using WIS.Automation;
using System.Net.Http;
using WIS.AutomationManager.Interfaces;
using WIS.Domain.Automatismo;
using WIS.Domain.Automatismo.Constants;
using WIS.Domain.Automatismo.Dtos;
using WIS.Domain.Automatismo.Interfaces;
using WIS.Domain.DataModel;
using WIS.Domain.Integracion;
using WIS.Domain.Integracion.Constants;
using WIS.Domain.Services.Interfaces;

namespace WIS.AutomationManager.Services
{
    public class AutomatismoInterpreterClientService : ClientIntegrationService, IAutomatismoInterpreterClientService
    {
        protected readonly IParameterService _parameterService;

        public AutomatismoInterpreterClientService(
            IUnitOfWorkFactory uowFactory,
            IConfiguration configuration,
            IParameterService parameterService,
            ILogger<AutomatismoInterpreterClientService> logger,
            IHttpContextAccessor httpContextAccessor) : base(logger, configuration, httpContextAccessor)
        {
            _parameterService = parameterService;

            this.SetHandleIntegration(() =>
            {
                using var uow = uowFactory.GetUnitOfWork();
                this.SetIntegration(uow.IntegracionServicioRepository.GetIntegrationByCodigo(IntegracionesDb.AutomationInterpreter));
            });
        }

        public AutomatismoResponse SendProductos(IAutomatismo automatismo, ProductosAutomatismoRequest request)
        {
            var interfazEnUso = automatismo.GetInterfazEnUso();

            if (interfazEnUso.InterfazExterna == CodigoInterfazAutomatismoDb.CD_INTERFAZ_UBICACIONES_PICKING)
                interfazEnUso = automatismo.GetInterfaz(CodigoInterfazAutomatismoDb.CD_INTERFAZ_PRODUCTOS);

            var interpretRequest = this.Map(interfazEnUso, request);
            var (response, httpResponse) = this.Post<AutomatismoResponse>(interpretRequest, "/Automatismo/SendProductos");

            return this.ProccessResult(response, httpResponse);
        }

        public AutomatismoResponse SendCodigosBarras(IAutomatismo automatismo, CodigosBarrasAutomatismoRequest request)
        {
            var interfazEnUso = automatismo.GetInterfazEnUso();

            if (interfazEnUso.InterfazExterna == CodigoInterfazAutomatismoDb.CD_INTERFAZ_UBICACIONES_PICKING)
                interfazEnUso = automatismo.GetInterfaz(CodigoInterfazAutomatismoDb.CD_INTERFAZ_CODIGO_BARRAS);

            var interpretRequest = this.Map(interfazEnUso, request);
            var (response, httpResponse) = this.Post<AutomatismoResponse>(interpretRequest, "/Automatismo/SendCodigosBarras");

            return this.ProccessResult(response, httpResponse);
        }

        public AutomatismoResponse SendEntrada(IAutomatismo automatismo, EntradaStockAutomatismoRequest request)
        {
            var interfazEnUso = automatismo.GetInterfazEnUso();
            var interpretRequest = this.Map(interfazEnUso, request);
            var (response, httpResponse) = this.Post<AutomatismoResponse>(interpretRequest, "/Automatismo/SendEntrada");

            return this.ProccessResult(response, httpResponse);
        }

        public AutomatismoResponse SendSalida(IAutomatismo automatismo, SalidaStockAutomatismoRequest request)
        {
            var interfazEnUso = automatismo.GetInterfazEnUso();
            var interpretRequest = this.Map(interfazEnUso, request);
            var (response, httpResponse) = this.Post<AutomatismoResponse>(interpretRequest, "/Automatismo/SendSalida");

            return this.ProccessResult(response, httpResponse);
        }

        private AutomatismoResponse ProccessResult(AutomatismoResponse response, HttpResponseMessage httpResponse)
        {
            if (response == null && !httpResponse.IsSuccessStatusCode)
            {
                string content = this.WaitAsync<string>(httpResponse.Content.ReadAsStringAsync());
                throw new Exception(content);
            }

            return response;
        }
    }
}
