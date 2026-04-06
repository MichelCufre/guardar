using System.Collections.Generic;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.Evento;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.FormComponent;
using WIS.FormComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.FormModules
{
    public class EVT040ModificarInstanciaFormValidationModule : FormValidationModule
    {
        protected readonly IUnitOfWork _uow;

        public EVT040ModificarInstanciaFormValidationModule(IUnitOfWork uow)
        {
            this._uow = uow;
            this.Schema = new FormValidationSchema
            {
                ["evento"] = this.ValidateEvento,
                ["instancia"] = this.ValidateInstancia,
                ["tipoNotificacion"] = this.ValidateTipoNotificacion,
                ["descripcion"] = this.ValidateDescripcion,
                ["plantilla"] = this.ValidatePlantilla,
            };
        }

        public virtual FormValidationGroup ValidateEvento(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new PositiveIntValidationRule(field.Value, true),
                    new ExisteEventoValidationRule(this._uow, field.Value)
                },
            };
        }

        public virtual FormValidationGroup ValidateInstancia(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new PositiveIntValidationRule(field.Value, true),
                    new ExisteEventoInstanciaValidationRule(this._uow, field.Value)
                },
            };
        }

        public virtual FormValidationGroup ValidateTipoNotificacion(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                     new NonNullValidationRule(field.Value),
                     new StringMaxLengthValidationRule(field.Value, 15),
                     new ExisteEventoTipoNotificacionValidationRule(_uow, field.Value),
                }
            };
        }

        public virtual FormValidationGroup ValidatePlantilla(FormField field, Form form, List<ComponentParameter> parameters)
        {
            string tpNotificacion = form.GetField("tipoNotificacion").Value;
            string nuEvento = form.GetField("evento").Value;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new StringMaxLengthValidationRule(field.Value, 15),
                    new NoExisteEventoTemplateValidationRule(_uow, nuEvento, tpNotificacion, field.Value),
                }
            };
        }

        public virtual FormValidationGroup ValidateDescripcion(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new StringMaxLengthValidationRule(field.Value, 200),
                }
            };
        }
    }
}
