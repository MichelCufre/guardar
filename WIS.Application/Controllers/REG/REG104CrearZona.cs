using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.General;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.Security;

namespace WIS.Application.Controllers.REG
{
    public class REG104CrearZona : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IFormValidationService _formValidationService;

        public REG104CrearZona(IIdentityService identity, IUnitOfWorkFactory uowFactory, IFormValidationService formValidationService)
        {
            this._uowFactory = uowFactory;
            this._formValidationService = formValidationService;
        }

        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            var fieldLocalidad = form.GetField("CD_LOCALIDAD");
            fieldLocalidad.Options = OptionSelectLocalidad();
            
            return form;
        }

        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            return this._formValidationService.Validate(new MantenimientoZonaFormValidationModule(uow), form, context);
        }

        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            string cdZona = form.GetField("CD_ZONA").Value;
            using var uow = this._uowFactory.GetUnitOfWork();

            var newZona = new Zona
            {
                CdZona = form.GetField("CD_ZONA").Value,
                NmZona = form.GetField("NM_ZONA").Value,
                DsZona = form.GetField("DS_ZONA").Value,
                CdLocalidad = form.GetField("CD_LOCALIDAD").Value
            };
            uow.ZonaRepository.AddZona(newZona);

            uow.SaveChanges();
            context.AddSuccessNotification("General_Sec0_msg_CambiosGuardados");

            return form;
        }

        public virtual List<SelectOption> OptionSelectLocalidad()
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            List<SelectOption> opciones = new List<SelectOption>();
            var list = uow.DominioRepository.GetDominios("TLOC").Select(w => new SelectOption(w.Valor, w.Descripcion)).OrderBy(a => a.Value).ToList();

            foreach (var l in list)
            {
                opciones.Add(new SelectOption(l.Value, l.Label));
            }

            return opciones;
        }

    }
}
