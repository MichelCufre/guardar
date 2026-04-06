using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.Configuracion;
using WIS.Application.Validation.Rules.Registro;
using WIS.Components.Common;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.General;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.FormComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.FormModules
{
    public class ClienteVentanaLiberacionFormValidationModule : FormValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly int _idUsuario;
        protected readonly IFormatProvider _culture;

        Func<Form, FormSelectSearchContext, List<SelectOption>> _searchPais;
        Func<Form, FormSelectSearchContext, List<SelectOption>> _searchPaisSubdivision;

        public ClienteVentanaLiberacionFormValidationModule(IUnitOfWork uow, int idUsuario, IFormatProvider culture)
        {
            this._uow = uow;
            this._idUsuario = idUsuario;
            this._culture = culture;

            this.Schema = new FormValidationSchema
            {
                ["empresa"] = this.ValidateCodigoEmpresa,
                ["cliente"] = this.ValidateCodigoAgente,
                ["ventanaLiberacion"] = this.ValidateVentanaLiberacion,
                ["cantidadDiasValidacion"] = this.ValidateCantidadDiasValidacion
            };
        }

        public virtual FormValidationGroup ValidateCodigoEmpresa(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (field.ReadOnly) return null;

            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new PositiveNumberMaxLengthValidationRule(field.Value,10),
                    new ExisteEmpresaValidationRule(_uow, field.Value)
                },

            };
        }

        public virtual FormValidationGroup ValidateCodigoAgente(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (field.ReadOnly) return null;

            string empresa = form.GetField("empresa").Value;

            field.Value = field.Value.Trim();

            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule>
                {
                   new NonNullValidationRule(field.Value),
                   new StringMaxLengthValidationRule(field.Value, 10),
                   new ExisteClienteValidationRule(_uow, field.Value,empresa),
                },
                Dependencies = { "empresa" }
            };
        }

        public virtual FormValidationGroup ValidateCantidadDiasValidacion(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (!string.IsNullOrEmpty(field.Value))
            {

                return new FormValidationGroup
                {
                    BreakValidationChain = false,
                    Rules = new List<IValidationRule>
                    {
                       new NonNullValidationRule(field.Value),
                       new PositiveShortNumberMaxLengthValidationRule(field.Value,3),
                    },
                };
            }
            else
                return null;
        }

        public virtual FormValidationGroup ValidateVentanaLiberacion(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (!string.IsNullOrEmpty(field.Value) && form.Id != "REG221Update_form_1")
            {
                string empresa = form.GetField("empresa").Value;
                string cliente = form.GetField("cliente").Value;
                return new FormValidationGroup
                {
                    BreakValidationChain = false,
                    Rules = new List<IValidationRule>
                    {
                        new NonNullValidationRule(field.Value),
                        new StringMaxLengthValidationRule(field.Value,20),
                        new ExisteVentanaLiberacionValidationRule(_uow,field.Value,empresa,cliente)
                    },
                    Dependencies = { "empresa", "cliente" }
                };
            }
            else
                return null;
        }
    }
}
