using System;
using System.Collections.Generic;
using System.Text;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.Expedicion;
using WIS.Application.Validation.Rules.General;
using WIS.Application.Validation.Rules.Preparacion;
using WIS.Components.Common;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.FormComponent;
using WIS.FormComponent.Validation;
using WIS.Security;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.FormModules
{
    public class CreatePrepManualLibreValidationModule : FormValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly IIdentityService _identity;
        protected readonly ISecurityService _security;

        public CreatePrepManualLibreValidationModule(IUnitOfWork uow, IIdentityService identity, ISecurityService security)
        {
            this.Schema = new FormValidationSchema
            {
                ["descripcion"] = this.ValidateDesc,
                ["predio"] = this.ValidatePredio,
                ["empresa"] = this.ValidateEmpresa,
                ["cliente"] = this.ValidateCliente,
                ["tipoExpedicion"] = this.ValidateTipoExpedicion,
                ["tipoPedido"] = this.ValidateTipoPedido,
                ["idReserva"] = this.ValidateIdentificadorReserva
            };

            this._uow = uow;
            this._identity = identity;
            this._security = security;
        }


        public virtual FormValidationGroup ValidateDesc(FormField field, Form form, List<ComponentParameter> parameters)
        {
            string empresa = form.GetField("empresa").Value;
            string cliente = form.GetField("cliente").Value;

            if (string.IsNullOrEmpty(field.Value))
                return null;

            var rules = new List<IValidationRule>
            {
                //new NonNullValidationRule(field.Value),
                new StringMaxLengthValidationRule(field.Value, 60)
            };

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = rules,
            };
        }
        public virtual FormValidationGroup ValidatePredio(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                   new NonNullValidationRule(field.Value),
                   new StringMaxLengthValidationRule(field.Value, 15),
                   new ExistePredioValidationRule(this._uow, this._identity.UserId, this._identity.Predio, field.Value)
                }
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

        public virtual FormValidationGroup ValidateTipoExpedicion(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                   new NonNullValidationRule(field.Value),
                   new StringMaxLengthValidationRule(field.Value, 6),
                   //new PositiveIntValidationRule(field.Value),
                   new ExisteTipoExpedicionValidationRule(this._uow, field.Value)
                },
                OnSuccess = this.ValidateTipoExpedicion_OnSucess
            };
        }
        public virtual void ValidateTipoExpedicion_OnSucess(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var listaTipos = new List<SelectOption>();

            PedidoMapper pedidoMapper = new PedidoMapper();

            Dictionary<string, string> tipos = this._uow.PedidoRepository.GetTiposPedido(field.Value);

            foreach (var tipo in tipos)
            {
                listaTipos.Add(new SelectOption(tipo.Key, tipo.Value));
            }

            form.GetField("tipoPedido").Options = listaTipos;
        }
        public virtual FormValidationGroup ValidateTipoPedido(FormField field, Form form, List<ComponentParameter> parameters)
        {
            string tipoExpedicion = form.GetField("tipoExpedicion").Value;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                   new NonNullValidationRule(field.Value),
                   new StringMaxLengthValidationRule(field.Value, 6),
                   new TipoPedidoCompatibilidadValidationRule(this._uow, field.Value, tipoExpedicion)
                },
                Dependencies = { "tipoExpedicion" }
            };
        }
        public virtual FormValidationGroup ValidateIdentificadorReserva(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value))
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new StringMaxLengthValidationRule(field.Value, 200)
                }
            };
        }
    }
}
