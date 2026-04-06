using System;
using System.Collections.Generic;
using System.Text;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.Evento;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.FormComponent;
using WIS.FormComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.FormModules
{
    public class EVT030ValidationModule : FormValidationModule
    {
        protected readonly IUnitOfWork _uow;
        public EVT030ValidationModule(IUnitOfWork uow)
        {
            this._uow = uow;

            this.Schema = new FormValidationSchema
            {
                ["NM_GRUPO"] = this.ValidateNombreGrupo,
            };
        }

        public virtual FormValidationGroup ValidateNombreGrupo(FormField field, Form form, List<ComponentParameter> parameters)
        {
            int nuGrupo = int.TryParse(form.GetField("NU_CONTACTO_GRUPO").Value, out int grupo) ? grupo : -1;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new StringMaxLengthValidationRule(field.Value, 100),
                    new NombreGrupoExistenteValidationRule(this._uow, field.Value, nuGrupo)
                },
            };
        }
    }
}
