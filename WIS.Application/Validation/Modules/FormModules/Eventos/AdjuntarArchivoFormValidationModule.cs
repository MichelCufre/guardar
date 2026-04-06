using System;
using System.Collections.Generic;
using System.Text;
using WIS.Application.Validation.Rules;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.FormComponent;
using WIS.FormComponent.Validation;
using WIS.GridComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.FormModules.Eventos
{
    public class AdjuntarArchivoFormValidationModule : FormValidationModule
    {
        protected readonly IUnitOfWork _uow;

        public AdjuntarArchivoFormValidationModule(IUnitOfWork uow)
        {
            this._uow = uow;

            this.Schema = new FormValidationSchema
            {
                ["TP_DOCUMENTO"] = this.ValidateTipoDocumento,
                ["DS_OBSERVACION"] = this.ValidateObservacion,
                ["DS_ANEXO"] = this.ValidateAnexo
            };
        }

        public virtual FormValidationGroup ValidateTipoDocumento(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new StringMaxLengthValidationRule(field.Value, 50),
                    //new TipoDocumentoValidationRule(field.Value,uow),//GERE

                },
            };
        }

        public virtual FormValidationGroup ValidateObservacion(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value)) 
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
            {
                new StringMaxLengthValidationRule(field.Value, 200),

            },
            };
        }

        public virtual FormValidationGroup ValidateAnexo(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value))
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
            {
                new StringMaxLengthValidationRule(field.Value, 200),

            },
            };
        }
    }
}
