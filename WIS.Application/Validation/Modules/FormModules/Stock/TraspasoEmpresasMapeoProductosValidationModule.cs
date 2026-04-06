using System;
using System.Collections.Generic;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.General;
using WIS.Application.Validation.Rules.Registro;
using WIS.Application.Validation.Rules.Stock;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.FormComponent;
using WIS.FormComponent.Validation;
using WIS.Security;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.FormModules.Stock
{
    public class TraspasoEmpresasMapeoProductosValidationModule : FormValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly IIdentityService _identity;
        protected readonly ISecurityService _security;
        protected readonly IFormatProvider _formatProvider;

        public TraspasoEmpresasMapeoProductosValidationModule(IUnitOfWork uow, IFormatProvider formatProvider, IIdentityService identity, ISecurityService security)
        {
            this.Schema = new FormValidationSchema
            {
                ["cdEmpresaOrigen"] = this.ValidateEmpresaOrigen,
                ["cdEmpresaDestino"] = this.ValidateEmpresaDestino,
                ["cdProdutoOrigen"] = this.ValidateProductoOrigen,
                ["cdProdutoDestino"] = this.ValidateProductoDestino,
                ["cantidadOrigen"] = this.ValidateCantidadOrigen,
                ["cantidadDestino"] = this.ValidateCantidadDestino,

            };

            this._uow = uow;
            this._identity = identity;
            this._security = security;
            this._formatProvider = formatProvider;
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
                }
            };
        }

        public virtual FormValidationGroup ValidateEmpresaDestino(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (field.ReadOnly)
                return null;

            var rules = new List<IValidationRule>
            {
                new NonNullValidationRule(field.Value),
                new StringMaxLengthValidationRule(field.Value, 10),
                new PositiveIntValidationRule(field.Value),
                new ExisteEmpresaValidationRule(this._uow, field.Value),
                new UserCanWorkWithEmpresaValidationRule(this._security, field.Value),
            };

            var cdProdutoOrigen = form.GetField("cdProdutoOrigen")?.Value;

            if (!string.IsNullOrEmpty(cdProdutoOrigen) && !string.IsNullOrEmpty(field.Value))
            {
                var empresaOrigen = int.Parse(form.GetField("cdEmpresaOrigen").Value);
                var cdFaixa = 1;

                rules.Add(new ExisteMapeoProductosValidationRule(this._uow, empresaOrigen, int.Parse(field.Value), cdFaixa, cdProdutoOrigen));
            }

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = rules
            };
        }

        public virtual FormValidationGroup ValidateProductoOrigen(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (field.ReadOnly)
                return null;

            string empresa = form.GetField("cdEmpresaOrigen").Value;

            var rules = new List<IValidationRule>
            {
                new NonNullValidationRule(field.Value),
                new StringMaxLengthValidationRule(field.Value, 40),
                new ProductoExisteEmpresaValidationRule(_uow, empresa, field.Value),
            };

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Dependencies = { "cdEmpresaOrigen" },
                Rules = rules
            };
        }

        public virtual FormValidationGroup ValidateProductoDestino(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var empresa = form.GetField("cdEmpresaDestino").Value;
            var cdProdutoOrigen = form.GetField("cdProdutoOrigen")?.Value;

            var rules = new List<IValidationRule>
            {
                new NonNullValidationRule(field.Value),
                new StringMaxLengthValidationRule(field.Value, 40),
                new ProductoExisteEmpresaValidationRule(_uow, empresa, field.Value),
            };

            if (!string.IsNullOrEmpty(cdProdutoOrigen))
            {
                var empresaOrigen = form.GetField("cdEmpresaOrigen").Value;

                if (empresaOrigen == empresa)
                    rules.Add(new ProductosCoincidenValidationRule(cdProdutoOrigen, field.Value));
            }

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Dependencies = { "cdEmpresaDestino" },
                Rules = rules
            };
        }

        public virtual FormValidationGroup ValidateCantidadOrigen(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var cdProdutoOrigen = form.GetField("cdProdutoOrigen")?.Value;

            if (string.IsNullOrEmpty(cdProdutoOrigen))
                return null;

            var empresaOrigen = int.Parse(form.GetField("cdEmpresaOrigen").Value);
            var productoOrigen = this._uow.ProductoRepository.GetProducto(empresaOrigen, cdProdutoOrigen);

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new PositiveDecimalValidationRule(this._formatProvider, field.Value, false),
                    new DecimalLengthWithPresicionValidationRule(field.Value, 12, 3, this._formatProvider,productoOrigen.AceptaDecimales),
                    new ManejoIdentificadorSerieCantidadValidationRule(field.Value, productoOrigen.ManejoIdentificador, _formatProvider)
                },
                Dependencies = { "cdProdutoOrigen" }
            };
        }

        public virtual FormValidationGroup ValidateCantidadDestino(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var cdProdDestino = form.GetField("cdProdutoDestino")?.Value;

            if (string.IsNullOrEmpty(cdProdDestino))
                return null;

            var empresaDestino = int.Parse(form.GetField("cdEmpresaDestino").Value);
            var productoDestino = this._uow.ProductoRepository.GetProducto(empresaDestino, cdProdDestino);

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new PositiveDecimalValidationRule(this._formatProvider, field.Value, false),
                    new DecimalLengthWithPresicionValidationRule(field.Value, 12, 3, this._formatProvider,productoDestino.AceptaDecimales),
                    new ManejoIdentificadorSerieCantidadValidationRule(field.Value, productoDestino.ManejoIdentificador, _formatProvider)
                },
                Dependencies = { "cdProdutoDestino" }
            };
        }

    }
}
