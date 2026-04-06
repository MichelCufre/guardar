using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.Ptl;
using WIS.Components.Common;
using WIS.Domain.Automatismo.Logic;
using WIS.Domain.DataModel;
using WIS.Domain.General;
using WIS.Domain.General.Auxiliares;
using WIS.Domain.Services.Interfaces;
using WIS.Extension;
using WIS.FormComponent;
using WIS.FormComponent.Validation;
using WIS.Security;
using WIS.Session;
using WIS.TrafficOfficer;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.FormModules.Ptl
{
    public class PTL010NotificarPTLModalValidationModule : FormValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly IIdentityService _identity;
        protected readonly ISessionAccessor _session;
        protected readonly ITrafficOfficerService _concurrencyControl;
        protected readonly IBarcodeService _barcodeService;

        protected readonly PtlLogic _logicPtl;

        public PTL010NotificarPTLModalValidationModule(IUnitOfWork uow, IIdentityService identity, ISessionAccessor session, ITrafficOfficerService concurrencyControl, IBarcodeService barcodeService)
        {
            this.Schema = new FormValidationSchema
            {
                ["contenedor"] = this.ValidateContenedor
            };

            _uow = uow;
            _identity = identity;
            _session = session;
            _concurrencyControl = concurrencyControl;
            _logicPtl = new PtlLogic(barcodeService, identity);
            _barcodeService = barcodeService;
        }

        public virtual FormValidationGroup ValidateContenedor(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var cliente = parameters.FirstOrDefault(x => x.Id == "cliente").Value;
            var empresa = parameters.FirstOrDefault(x => x.Id == "empresa").Value.ToNumber<int>();
            var preparacion = parameters.FirstOrDefault(x => x.Id == "preparacion").Value.ToNumber<int>();

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new StringMaxLengthValidationRule(field.Value, 16),
                    new PTL010NotificarPTLModalContenedorValidationRule(_uow, _identity, _session, _concurrencyControl, field.Value, preparacion, _barcodeService)
                },
                OnSuccess = this.ValidatePredio_OnSucess,
            };
        }

        public virtual void ValidatePredio_OnSucess(FormField field, Form form, List<ComponentParameter> parameters)
        {
            
            _barcodeService.ValidarEtiquetaContenedor(field.Value, _identity.UserId, out AuxContenedor datosContenedor, out int cantidadEmpresa);

            Contenedor contenedor = _uow.ContenedorRepository.GetContenedor(datosContenedor.NuPreparacion, datosContenedor.NuContenedor);
            if (contenedor != null)
            {
                var agrupacionContenedor = _logicPtl.GetAgrupacionContenedor(_uow, contenedor.NumeroPreparacion, contenedor.Numero);

                if (agrupacionContenedor != null)
                {
                    if (parameters.Any(x => x.Id == "AGRUPACION_CONTENEDOR_LEIDO"))
                        parameters.FirstOrDefault(x => x.Id == "AGRUPACION_CONTENEDOR_LEIDO").Value = JsonConvert.SerializeObject(agrupacionContenedor);
                    else
                        parameters.Add(new ComponentParameter
                        {
                            Id = "AGRUPACION_CONTENEDOR_LEIDO",
                            Value = JsonConvert.SerializeObject(agrupacionContenedor)
                        });
                }
            }
        }
    }
}
