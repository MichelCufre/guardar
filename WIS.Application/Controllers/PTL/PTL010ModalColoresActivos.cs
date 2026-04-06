using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Components.Common.Select;
using WIS.Domain.Automatismo;
using WIS.Domain.Automatismo.Constants;
using WIS.Domain.DataModel;
using WIS.Domain.General;
using WIS.Domain.Services.Interfaces;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.Security;

namespace WIS.Application.Controllers.PTL
{
    public class PTL010ModalColoresActivos : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly ISecurityService _security;
        protected readonly IFormValidationService _formValidationService;
        protected readonly IAutomatismoPtlClientService _automatismoPtlClientService;

        public PTL010ModalColoresActivos(
          ISecurityService security,
          IUnitOfWorkFactory uowFactory,
          IIdentityService identity,
          IFormValidationService formValidationService,
          IAutomatismoPtlClientService automatismoPtlClientService)
        {
            _security = security;
            _uowFactory = uowFactory;
            _identity = identity;
            _formValidationService = formValidationService;
            _automatismoPtlClientService = automatismoPtlClientService;
        }

        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            try
            {
                InicializarSelectAutomatismo(form, context);
            }
            catch (Exception ex)
            {
                context.AddErrorNotification(ex.Message);
            }

            return form;
        }

        public virtual void InicializarSelectAutomatismo(Form form, FormInitializeContext context)
        {
            var automatismosSelect = form.GetField("automatismo");

            List<SelectOption> opciones = new List<SelectOption>();

            (ValidationsResult validationResult, string automatismosJson) = _automatismoPtlClientService.GetPtlByTipoAutomatismo(PtlTipoDb.PICKING);

            if (validationResult != null && validationResult.Errors.Count() > 0)
                validationResult.Errors.ForEach(x => { context.AddErrorNotification(string.Join('.', x.Messages)); });

            else
            {
                var automatismos = JsonConvert.DeserializeObject<List<AutomatismoPtl>>(automatismosJson);

                foreach (var automatismo in automatismos)
                    opciones.Add(new SelectOption { Value = automatismo.Numero.ToString(), Label = automatismo.Descripcion });

                if (opciones.Count == 1)
                {
                    form.GetField("automatismo").Value = opciones.FirstOrDefault().Value;
                    context.AddParameter("GRID_FETCH_QUERY", form.GetField("automatismo").Value);
                }
            }

            automatismosSelect.Options = opciones;
        }
    }
}
