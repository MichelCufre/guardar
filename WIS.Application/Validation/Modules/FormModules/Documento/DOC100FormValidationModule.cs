using System.Collections.Generic;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.Documento;
using WIS.Application.Validation.Rules.General;
using WIS.Application.Validation.Rules.Preparacion;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Services.Interfaces;
using WIS.FormComponent;
using WIS.FormComponent.Validation;
using WIS.Security;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.FormModules.Documento
{
    public class DOC100FormValidationModule : FormValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly IIdentityService _identity;
        protected readonly ISecurityService _security;
        protected readonly IParameterService _parameterService;
        protected readonly bool _isEdit;

        public DOC100FormValidationModule(IUnitOfWork uow,
            IIdentityService identity,
            ISecurityService security,
            IParameterService parameterService,
            bool isEdit)
        {
            this._uow = uow;
            this._identity = identity;
            this._security = security;
            this._parameterService = parameterService;
            this._isEdit = isEdit;

            this.Schema = new FormValidationSchema
            {
                ["empresaEgreso"] = this.ValidateEmpresaEgreso,
                ["empresaIngreso"] = this.ValidateEmpresaIngreso,
                ["preparacion"] = this.ValidatePreparacion,
                ["tpOperativa"] = this.ValidateTipoOperativa,
                ["autoDocIngreso"] = this.ValidateAutoDocIngreso,
                ["docIngreso"] = this.ValidateDocumentoIngreso,
                ["autoDocEgreso"] = this.ValidateAutoDocEgreso,
                ["docEgreso"] = this.ValidateDocumentoEgreso
            };
        }

        public virtual FormValidationGroup ValidateEmpresaEgreso(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                   new NonNullValidationRule(field.Value),
                   new StringMaxLengthValidationRule(field.Value, 10),
                   new PositiveIntValidationRule(field.Value),
                   new ExisteEmpresaValidationRule(this._uow, field.Value),
                   new UserCanWorkWithEmpresaValidationRule(this._security, field.Value)
                },
                Dependencies = { "preparacion", "empresaIngreso", "docIngreso", "docEgreso" },
            };
        }

        public virtual FormValidationGroup ValidateEmpresaIngreso(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var tpOperativa = form.GetField("tpOperativa").Value;
            var empresaEgreso = form.GetField("empresaEgreso").Value;

            var reqEmpresaIngreso = !string.IsNullOrEmpty(tpOperativa) && tpOperativa != TipoOperativaDocumental.Produccion;
            parameters.RemoveAll(p => p.Id == "reqEmpresaIngreso");
            parameters.Add(new ComponentParameter() { Id = "reqEmpresaIngreso", Value = reqEmpresaIngreso.ToString().ToLower() });

            if (tpOperativa == TipoOperativaDocumental.Produccion)
            {
                field.Value = null; 
                return null;
            }

            var rules = new List<IValidationRule>
            {
                new NonNullValidationRule(field.Value),
                new StringMaxLengthValidationRule(field.Value, 10),
                new PositiveIntValidationRule(field.Value),
                new ExisteEmpresaValidationRule(this._uow, field.Value),
                new UserCanWorkWithEmpresaValidationRule(this._security, field.Value)
            };

            if (tpOperativa == TipoOperativaDocumental.Transferencia)
                rules.Add(new TransferenciaEmpresaIngresoValidationRule(field.Value, empresaEgreso));

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = rules,
                Dependencies = { "docIngreso" },
            };
        }

        public virtual FormValidationGroup ValidatePreparacion(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var empresa = form.GetField("empresaEgreso").Value;
            var tpOperativa = form.GetField("tpOperativa").Value;

            var rules = new List<IValidationRule>
            {
                new NonNullValidationRule(field.Value),
                new StringMaxLengthValidationRule(field.Value, 6),
                new PositiveIntValidationRule(field.Value),
            };

            if (!string.IsNullOrEmpty(field.Value) && int.TryParse(field.Value, out int preparacion))
            {
                rules.Add(new ExistePreparacionValidationRule(this._uow, preparacion));
                rules.Add(new PreparacionEmpresaValidationRule(field.Value, empresa, this._uow));

                if (!this._isEdit)
                {
                    rules.Add(new PreparacionAsociableValidationRule(field.Value, tpOperativa, this._uow));
                }
            }

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = rules
            };
        }

        public virtual FormValidationGroup ValidateTipoOperativa(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                   new NonNullValidationRule(field.Value),
                   new StringMaxLengthValidationRule(field.Value, 10)
                },
                Dependencies = { "preparacion", "empresaIngreso", "docIngreso", "docEgreso" },
            };
        }

        public virtual FormValidationGroup ValidateAutoDocIngreso(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                   new NonNullValidationRule(field.Value),
                },
                OnSuccess = this.ValidateAutoDocIngreso_OnSuccess
            };
        }

        public virtual FormValidationGroup ValidateDocumentoIngreso(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var rules = new List<IValidationRule>();
            var autoDocIngreso = bool.Parse(form.GetField("autoDocIngreso").Value);
            var tpOperativa = form.GetField("tpOperativa").Value;
            var empresa = form.GetField("empresaIngreso").Value;

            if (tpOperativa == TipoOperativaDocumental.Produccion)
                empresa = form.GetField("empresaEgreso").Value;

            if (!autoDocIngreso)
            {
                rules.Add(new NonNullValidationRule(field.Value));
                rules.Add(new TipoDocumentoOperativaValidationRule(field.Value, tpOperativa, true, empresa, this._parameterService));
                rules.Add(new DocumentoEmpresaValidationRule(field.Value, empresa, this._uow));
                rules.Add(new DocumentoAsociableValidationRule(field.Value, this._uow));
            }
            else if (string.IsNullOrEmpty(field.Value))
            {
                return null;
            }

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = rules,
            };
        }

        public virtual void ValidateAutoDocIngreso_OnSuccess(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (!string.IsNullOrEmpty(field.Value))
            {
                var autoDocIngreso = bool.Parse(field.Value);
                var docIngreso = form.GetField("docIngreso");

                if (autoDocIngreso)
                {
                    docIngreso.Disabled = true;
                    docIngreso.ReadOnly = true;
                    docIngreso.Value = string.Empty;
                }
                else
                {
                    docIngreso.Disabled = false;
                    docIngreso.ReadOnly = false;
                }
            }
        }

        public virtual FormValidationGroup ValidateAutoDocEgreso(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                   new NonNullValidationRule(field.Value),
                },
                OnSuccess = this.ValidateAutoDocEgreso_OnSuccess
            };
        }

        public virtual FormValidationGroup ValidateDocumentoEgreso(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var rules = new List<IValidationRule>();
            var autoDocEgreso = bool.Parse(form.GetField("autoDocEgreso").Value);
            var tpOperativa = form.GetField("tpOperativa").Value;
            var empresa = form.GetField("empresaEgreso").Value;

            if (!autoDocEgreso)
            {
                rules.Add(new NonNullValidationRule(field.Value));
                rules.Add(new TipoDocumentoOperativaValidationRule(field.Value, tpOperativa, false, empresa, this._parameterService));
                rules.Add(new DocumentoEmpresaValidationRule(field.Value, empresa, this._uow));
                rules.Add(new DocumentoAsociableValidationRule(field.Value, this._uow));
            }
            else if (string.IsNullOrEmpty(field.Value))
            {
                return null;
            }

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = rules,
            };
        }

        public virtual void ValidateAutoDocEgreso_OnSuccess(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (!string.IsNullOrEmpty(field.Value))
            {
                var autoDocEgreso = bool.Parse(field.Value);
                var docEgreso = form.GetField("docEgreso");

                if (autoDocEgreso)
                {
                    docEgreso.Disabled = true;
                    docEgreso.ReadOnly = true;
                    docEgreso.Value = string.Empty;
                }
                else
                {
                    docEgreso.Disabled = false;
                    docEgreso.ReadOnly = false;
                }
            }
        }
    }
}
