using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.Documento;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.FormComponent;
using WIS.FormComponent.Validation;
using WIS.Security;
using WIS.Session;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.FormModules
{
    public class EXP052FormValidationModule : FormValidationModule //TODO: Rehacer clase
    {
        protected readonly bool _editMode;
        protected readonly IUnitOfWork _uow;
        protected readonly ISessionAccessor _sessionAccessor;
        protected readonly IIdentityService _securityService;

        public EXP052FormValidationModule(
            bool editMode, 
            IUnitOfWork uow, 
            ISessionAccessor sessionAccessor, 
            IIdentityService securityService)
        {
            this._editMode = editMode;
            this._uow = uow;
            this._sessionAccessor = sessionAccessor;
            this._securityService = securityService;

            this.Schema = new FormValidationSchema
            {
                ["tpEgreso"] = this.ValidateTP_EGRESO,
                ["nroDocNoGenerado"] = this.ValidateNU_DOCUMENTO_MANUAL,
            };
        }

        public virtual FormValidationGroup ValidateTP_EGRESO(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                   new NonNullValidationRule(field.Value),
                   new StringMaxLengthValidationRule(field.Value,2),
                },
                OnSuccess = this.ValidateTP_EGRESO_OnSuccess
            };
        }

        public virtual void ValidateTP_EGRESO_OnSuccess(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (!string.IsNullOrEmpty(field.Value))
            {
                var tipoDocumento = this._uow.DocumentoTipoRepository.GetTipoDocumento(field.Value);
                parameters.Add(new ComponentParameter() { Id = "isAutoGenerado", Value = tipoDocumento.NumeroAutogenerado.ToString().ToLower() });
                parameters.Add(new ComponentParameter() { Id = "formatoNumDoc", Value = tipoDocumento.Mask });
            }
        }

        public virtual FormValidationGroup ValidateNU_DOCUMENTO_MANUAL(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var tpIngreso = form.GetField("tpEgreso").Value;
            var tipoDocumento = this._uow.DocumentoTipoRepository.GetTipoDocumento(tpIngreso);

            if (!this._editMode && !tipoDocumento.NumeroAutogenerado)
            {
                var mask = tipoDocumento.Mask;
                if (!string.IsNullOrEmpty(mask) && parameters.Any(a => a.Id == "nroDocNoGenerado"))
                {
                    var value = parameters.FirstOrDefault(a => a.Id == "nroDocNoGenerado").Value;

                    if (!string.IsNullOrEmpty(value))
                    {
                        var maskChars = tipoDocumento.MaskChars ?? string.Empty;
                        foreach (var c in tipoDocumento.MaskChars)
                        {
                            value = value.Replace(c.ToString(), string.Empty);
                        }
                    }

                    field.Value = value;
                }

                return new FormValidationGroup
                {
                    Dependencies = { "tpEgreso" },
                    BreakValidationChain = true,
                    Rules = new List<IValidationRule>
                    {
                        new NonNullValidationRule(field.Value),
                        new PositiveLongValidationRule(field.Value),
                        new NumeroDocumentoIngresoNoAutogeneradoValidationRule(tpIngreso, field.Value, this._uow)
                    }
                };
            }
            else
            {
                return null;
            }
        }
    }
}
