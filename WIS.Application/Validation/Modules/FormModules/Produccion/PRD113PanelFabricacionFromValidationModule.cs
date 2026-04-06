using DocumentFormat.OpenXml.Vml.Office;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.General;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.Produccion.Constants;
using WIS.FormComponent;
using WIS.FormComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.FormModules.Produccion
{
    public class PRD113PanelFabricacionFromValidationModule : FormValidationModule
    {
        protected readonly IFormatProvider _culture;
        protected readonly IUnitOfWork _uow;

        public PRD113PanelFabricacionFromValidationModule(IUnitOfWork uow, IFormatProvider culture)
        {
            _uow = uow;
            _culture = culture;
            Schema = new FormValidationSchema
            {
                ["idModalidadLoteProduccion"] = ValidateModalidadIngresoLote,
                ["loteUtilizar"] = ValidateLote,
                ["fechaVencimiento"] = ValidateVencimiento,
            };
        }
        public virtual FormValidationGroup ValidateVencimiento(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (parameters.FirstOrDefault(x => x.Id == "isRequiredVencimiento")?.Value == "S" && (parameters.FirstOrDefault(x => x.Id == "validarModalidad")?.Value == "S" || !string.IsNullOrEmpty(field.Value)))
            {
                return new FormValidationGroup
                {
                    BreakValidationChain = true,
                    Rules = new List<IValidationRule>
                    {
                        new NonNullValidationRule(field.Value),
                    },
                };
            }
            else
            {
                return null;
            }
        }
        public virtual FormValidationGroup ValidateLote(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (parameters.FirstOrDefault(x => x.Id == "isRequiredModalidadLote")?.Value == "S" && (parameters.FirstOrDefault(x => x.Id == "validarModalidad")?.Value == "S" || !string.IsNullOrEmpty(field.Value)))
            {
                return new FormValidationGroup
                {
                    BreakValidationChain = false,
                    Rules = new List<IValidationRule>
                    {
                        new NonNullValidationRule(field.Value),
                        new StringMaxLengthValidationRule(field.Value, 40),
                        new IdentificadorValidationRule(_uow, field.Value)
                    },
                };
            }
            else
            {
                return null;
            }
        }
        public virtual FormValidationGroup ValidateModalidadIngresoLote(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if ((parameters.FirstOrDefault(x => x.Id == "isRequiredModalidadLote")?.Value == "S" && parameters.FirstOrDefault(x => x.Id == "validarModalidad")?.Value == "S") || !string.IsNullOrEmpty(field.Value))
            {
                return new FormValidationGroup
                {
                    BreakValidationChain = false,
                    Rules = new List<IValidationRule>
                    {
                        new NonNullValidationRule(field.Value)
                    },
                    OnSuccess = this.ValidateModalidadIngresoLoteOnSuccess
                };
            }
            else
            {
                return null;
            }
        }

        public virtual void ValidateModalidadIngresoLoteOnSuccess(FormField field, Form form, List<ComponentParameter> componentparams)
        {
            if (!componentparams.Any(s => s.Id == "isSubmit"))
            {
                var lote = form.GetField("loteUtilizar");
                var idIngreso = form.GetField("idInternoProduccion").Value;
                var ingreso = this._uow.IngresoProduccionRepository.GetIngresoById(idIngreso);

                lote.ReadOnly = true;
                switch (field.Value)
                {
                    case CTipoIngresoModalidadLote.ID_INTERNO:
                        lote.Value = ingreso.Id;
                        break;

                    case CTipoIngresoModalidadLote.ID_EXTERNO:
                        lote.Value = ingreso.IdProduccionExterno;
                        break;

                    case CTipoIngresoModalidadLote.FECHA_DIA:
                        lote.Value = DateTime.Now.ToString("d", this._culture);
                        break;

                    case CTipoIngresoModalidadLote.MES_PROD:
                        var mesProduccion = ingreso.FechaAlta.Value.Month;
                        lote.Value = mesProduccion.ToString();
                        break;

                    default:
                        lote.Value = string.Empty;
                        lote.ReadOnly = false;
                        break;
                }
            }
        }
    }
}
