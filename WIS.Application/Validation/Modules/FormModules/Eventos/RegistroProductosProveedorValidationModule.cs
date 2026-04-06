using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.General;
using WIS.Application.Validation.Rules.Registro;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Repositories;
using WIS.Domain.General;
using WIS.FormComponent;
using WIS.FormComponent.Validation;
using WIS.GridComponent;
using WIS.GridComponent.Validation;
using WIS.Security;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.FormModules.Eventos
{
    public class RegistroProductosProveedorValidationModule : FormValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly ISecurityService _security;

        public RegistroProductosProveedorValidationModule(IUnitOfWork uow, ISecurityService security)
        {
            this._uow = uow;
            this._security = security;

            this.Schema = new FormValidationSchema
            {
                ["empresa"] = this.ValidateEmpresa,
                ["cliente"] = this.ValidateCliente,
                ["producto"] = this.ValidateProducto,
                ["codigoExterno"] = this.ValidateCodigoExterno,
            };
        }

        public virtual FormValidationGroup ValidateEmpresa(FormField field, Form form, List<ComponentParameter> parameters)
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
                }
            };
        }

        public virtual FormValidationGroup ValidateCliente(FormField field, Form form, List<ComponentParameter> parameters)
        {
            string empresa = form.GetField("empresa").Value;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Dependencies = { "empresa" },
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new ExisteClienteValidationRule(this._uow, field.Value, empresa)
                }
            };
        }

        public virtual FormValidationGroup ValidateProducto(FormField field, Form form, List<ComponentParameter> parameters)
        {
            string empresa = form.GetField("empresa").Value;
            string cliente = form.GetField("cliente").Value;

            var rules = new List<IValidationRule>
            {
                new NonNullValidationRule(field.Value),
                new StringMaxLengthValidationRule(field.Value, 40),
                new ProductoExisteEmpresaValidationRule(_uow, empresa, field.Value),
            };

            if ((parameters.FirstOrDefault(p => p.Id == "isUpdate")?.Value ?? "N") != "S")
                rules.Add(new ExisteProductoProveedorValidationRule(this._uow, empresa, cliente, field.Value));

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Dependencies = { "empresa", "cliente" },
                Rules = rules
            };
        }

        public virtual FormValidationGroup ValidateCodigoExterno(FormField field, Form form, List<ComponentParameter> parameters)
        {
            string empresa = form.GetField("empresa").Value;
            string cliente = form.GetField("cliente").Value;
            string producto = form.GetField("producto").Value;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Dependencies = { "empresa", "cliente", "producto" },
                Rules = new List<IValidationRule> {
                    new NonNullValidationRule(field.Value),
                    new StringSoloUpperValidationRule(field.Value),
                    new StringMaxLengthValidationRule(field.Value, 30),
                    new ExisteCodigoExternoProductoProveedorValidationRule(_uow, field.Value, empresa, cliente, producto),
                }
            };
        }
    }
}