using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.Security;

namespace WIS.Application.Controllers.REG
{
    public class REG104ModificarZona : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IFormValidationService _formValidationService;

        public REG104ModificarZona(IIdentityService identity, IUnitOfWorkFactory uowFactory, IFormValidationService formValidationService)
        {
            this._uowFactory = uowFactory;
            this._formValidationService = formValidationService;
        }

        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            var cdZona = context.Parameters.FirstOrDefault(x => x.Id == "cdZona")?.Value;
            var fieldCdZona = form.GetField("CD_ZONA");
            var fieldLocalidad = form.GetField("CD_LOCALIDAD");
            fieldLocalidad.Options = OptionSelectLocalidad();

            if (!string.IsNullOrEmpty(cdZona))
            {
                using var uow = this._uowFactory.GetUnitOfWork();

                var zona = uow.ZonaRepository.GetZona(cdZona);
                fieldCdZona.Value = zona.CdZona;
                form.GetField("NM_ZONA").Value = zona.NmZona;

                if (zona.DsZona != null)
                    form.GetField("DS_ZONA").Value = zona.DsZona;

                fieldLocalidad.Value = zona.CdLocalidad;
                fieldCdZona.ReadOnly = true;
            }

            return form;
        }

        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            return this._formValidationService.Validate(new MantenimientoZonaFormValidationModule(uow), form, context);
        }

        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            string cdZona = form.GetField("CD_ZONA").Value;
            var zona = uow.ZonaRepository.GetZona(cdZona);

            if (zona != null)
            {
                zona.NmZona = form.GetField("NM_ZONA").Value;
                zona.DsZona = form.GetField("DS_ZONA").Value;
                zona.CdLocalidad = form.GetField("CD_LOCALIDAD").Value;
                uow.ZonaRepository.UpdateZona(zona);
            }

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
