using System.Collections.Generic;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.General;
using WIS.Application.Validation.Rules.Stock;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.FormComponent;
using WIS.FormComponent.Validation;
using WIS.Security;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.FormModules.Stock
{
    public class ConfiguracionTraspasoEmpresasValidationModule : FormValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly IIdentityService _identity;
        protected readonly ISecurityService _security;

        public ConfiguracionTraspasoEmpresasValidationModule(IUnitOfWork uow, IIdentityService identity, ISecurityService security)
        {
            this.Schema = new FormValidationSchema
            {
                ["cdEmpresaOrigen"] = this.ValidateEmpresaOrigen,
                ["flTodaEmpresa"] = this.ValidateTodaEmpresa,
                ["flTodoTipoTraspaso"] = this.ValidateTodoTipoTraspaso,
                ["flCabezalAuto"] = this.ValidateCabezalAuto,
                ["flReplicaProductos"] = this.ValidateReplicaProductos,
                ["flReplicaCB"] = this.ValidateReplicaCB,
                ["flCtrlCaractIguales"] = this.ValidateCtrlCaractIguales,
                ["flReplicaAgentes"] = this.ValidateReplicaAgentes,
                ["cdTipoDocuIngreso"] = this.ValidateTipoDocuIngreso,
                ["cdTipoDocuEgreso"] = this.ValidateTipoDocuEgreso,

            };

            this._uow = uow;
            this._identity = identity;
            this._security = security;
        }

        public virtual FormValidationGroup ValidateEmpresaOrigen(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (field.ReadOnly)
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                   new NonNullValidationRule(field.Value),
                   new StringMaxLengthValidationRule(field.Value, 10),
                   new PositiveIntValidationRule(field.Value),
                   new ExisteEmpresaValidationRule(this._uow, field.Value),
                   new UserCanWorkWithEmpresaValidationRule(this._security, field.Value),
                   new EmpresaTieneConfiguracionTraspasoValidationRule(this._uow, field.Value),
                }
            };
        }

        public virtual FormValidationGroup ValidateTodaEmpresa(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                   new NonNullValidationRule(field.Value),
                   new BooleanStringValidationRule(field.Value)
                }
            };
        }

        public virtual FormValidationGroup ValidateTodoTipoTraspaso(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                   new NonNullValidationRule(field.Value),
                   new BooleanStringValidationRule(field.Value)
                }
            };
        }

        public virtual FormValidationGroup ValidateCabezalAuto(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                   new NonNullValidationRule(field.Value),
                   new BooleanStringValidationRule(field.Value)
                }
            };
        }

        public virtual FormValidationGroup ValidateReplicaProductos(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                   new NonNullValidationRule(field.Value),
                   new BooleanStringValidationRule(field.Value)
                }
            };
        }

        public virtual FormValidationGroup ValidateReplicaCB(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                   new NonNullValidationRule(field.Value),
                   new BooleanStringValidationRule(field.Value)
                }
            };
        }

        public virtual FormValidationGroup ValidateCtrlCaractIguales(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                   new NonNullValidationRule(field.Value),
                   new BooleanStringValidationRule(field.Value)
                }
            };
        }

        public virtual FormValidationGroup ValidateReplicaAgentes(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                   new NonNullValidationRule(field.Value),
                   new BooleanStringValidationRule(field.Value)
                }
            };
        }

        public virtual FormValidationGroup ValidateTipoDocuIngreso(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var empresa = form.GetField("cdEmpresaOrigen").Value;
            if (string.IsNullOrEmpty(empresa))
                return null;

            var rules = new List<IValidationRule>();

            int.TryParse(empresa, out int codEmpresa);
            if (_uow.EmpresaRepository.IsEmpresaDocumental(codEmpresa))
                rules.Add(new NonNullValidationRule(field.Value));

            if (!string.IsNullOrEmpty(field.Value))
            {
                rules.Add(new StringMaxLengthValidationRule(field.Value, 6));
                rules.Add(new ExisteTipoDocumentoValidationRule(this._uow, field.Value));
                rules.Add(new TipoDocumentoOperacionIngresoValidationRule(this._uow, field.Value));
                rules.Add(new TipoDocumentoHabilitadoValidationRule(this._uow, field.Value));
            }



            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = rules
            };
        }

        public virtual FormValidationGroup ValidateTipoDocuEgreso(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var empresa = form.GetField("cdEmpresaOrigen").Value;
            if (string.IsNullOrEmpty(empresa))
                return null;

            var rules = new List<IValidationRule>();

            int.TryParse(empresa, out int codEmpresa);
            if (_uow.EmpresaRepository.IsEmpresaDocumental(codEmpresa))
                rules.Add(new NonNullValidationRule(field.Value));
            if (!string.IsNullOrEmpty(field.Value))
            {
                rules.Add(new StringMaxLengthValidationRule(field.Value, 6));
                rules.Add(new ExisteTipoDocumentoValidationRule(this._uow, field.Value));
                rules.Add(new TipoDocumentoOperacionEgresoValidationRule(this._uow, field.Value));
                rules.Add(new TipoDocumentoHabilitadoValidationRule(this._uow, field.Value));
            }

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = rules
            };
        }
    }
}
