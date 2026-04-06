using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.Evento;
using WIS.Components.Common;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.Eventos;
using WIS.FormComponent;
using WIS.FormComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.FormModules
{
    class EVT040CrearInstanciaFormValidationModule : FormValidationModule
    {
        protected readonly IUnitOfWork _uow;

        public EVT040CrearInstanciaFormValidationModule(IUnitOfWork uow)
        {
            this._uow = uow;
            this.Schema = new FormValidationSchema
            {
                ["evento"] = this.ValidateEvento,
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
                OnSuccess = this.ValidateEventoOnSuccess
            };
        }

        public virtual void ValidateEventoOnSuccess(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (!parameters.Any(s => s.Id == "isSubmit"))
            {
                form.GetField("plantilla").Value = string.Empty;
                form.GetField("plantilla").Options.Clear();

                var tipoNotificacion = form.GetField("tipoNotificacion").Value;
                
                if(!string.IsNullOrEmpty(tipoNotificacion))
                {
                    var selectTemplate = form.GetField("plantilla");
                    var templates = _uow.EventoRepository.GeEventoTemplatoByEventoTipoNotificacion(int.Parse(field.Value), tipoNotificacion);

                    foreach (var template in templates)
                    {
                        selectTemplate.Options.Add(new SelectOption(template.CdEstilo, template.CdEstilo + " " + template.dsEstilo));
                    }
                }
            }
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
                },
                OnSuccess = this.ValidateTipoNotificacionSuccess
            };
        }

        public virtual void ValidateTipoNotificacionSuccess(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (!parameters.Any(s => s.Id == "isSubmit"))
            {
                form.GetField("plantilla").Value = string.Empty;
                form.GetField("plantilla").Options.Clear();

                var evento = form.GetField("evento").Value;

                if (!string.IsNullOrEmpty(evento))
                {
                    var selectTemplate = form.GetField("plantilla");
                    var templates = _uow.EventoRepository.GeEventoTemplatoByEventoTipoNotificacion(int.Parse(evento), field.Value);

                    foreach (var template in templates)
                    {
                        selectTemplate.Options.Add(new SelectOption(template.CdEstilo, template.CdEstilo + " " + template.dsEstilo));
                    }
                }
            }
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
