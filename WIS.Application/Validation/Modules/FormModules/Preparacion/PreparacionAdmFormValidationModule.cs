using DocumentFormat.OpenXml.InkML;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.Expedicion;
using WIS.Application.Validation.Rules.General;
using WIS.Application.Validation.Rules.Preparacion;
using WIS.Components.Common;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.Expedicion;
using WIS.Domain.General;
using WIS.Domain.Picking;
using WIS.Domain.Tracking.Models;
using WIS.FormComponent;
using WIS.FormComponent.Validation;
using WIS.Security;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.FormModules
{
    public class PreparacionAdmFormValidationModule : FormValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly IIdentityService _identity;
        protected readonly ISecurityService _security;
        protected readonly ConfiguracionPedido _configuracion;

        public PreparacionAdmFormValidationModule(IUnitOfWork uow, IIdentityService identity, ISecurityService security, ConfiguracionPedido configuracion)
        {
            this._uow = uow;
            this._identity = identity;
            this._security = security;
            this._configuracion = configuracion;

            this.Schema = new FormValidationSchema
            {
                ["descripcion"] = this.ValidateDescripcion,
                ["pedido"] = this.ValidatePedido,
                ["predio"] = this.ValidatePredio,
                ["empresa"] = this.ValidateEmpresa,
                ["cliente"] = this.ValidateCliente,
                ["ruta"] = this.ValidateRuta,
                ["tipoExpedicion"] = this.ValidateTipoExpedicion,
                ["tipoPedido"] = this.ValidateTipoPedido,
                ["fechaEntrega"] = this.ValidateFechaEntrega,
                ["condicionStock"] = this.ValidateCondicionStock,
                ["cantidadPrevVenc"] = this.ValidateCantidadPrevVenc,
                ["permitePickearVencido"] = this.ValidatePermitePickearVencido,

            };
        }

        public virtual FormValidationGroup ValidateDescripcion(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value))
                return null;

            field.Value = field.Value.Trim();

            var rules = new List<IValidationRule>
            {
                new StringMaxLengthValidationRule(field.Value, 40),
            };

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = rules,
                OnSuccess = this.ValidateDescripcionOnSuccess
            };
        }

        public virtual void ValidateDescripcionOnSuccess(FormField field, Form form, List<ComponentParameter> parameters)
        {
            parameters.Add(new ComponentParameter() { Id = "descripcion", Value = field.Value });
        }

        public virtual FormValidationGroup ValidatePedido(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var empresa = form.GetField("empresa").Value;
            var cliente = form.GetField("cliente").Value;

            if (string.IsNullOrEmpty(field.Value) || field.Disabled)
                return null;

            field.Value = field.Value.Trim();

            var rules = new List<IValidationRule>
            {
                new StringMaxLengthValidationRule(field.Value, 40),
                new NoExistePedidoValidationRule(this._uow, field.Value, cliente, empresa)
            };

            if (this._configuracion.PedidoDebeSerNumerico)
                rules.Add(new PositiveIntValidationRule(field.Value));

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = rules,
                Dependencies = { "empresa", "cliente" },
                OnSuccess = this.ValidatePedidoOnSuccess
            };
        }

        public virtual void ValidatePedidoOnSuccess(FormField field, Form form, List<ComponentParameter> parameters)
        {
            parameters.Add(new ComponentParameter() { Id = "pedido", Value = field.Value });
        }

        public virtual FormValidationGroup ValidatePredio(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value))
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                   new NonNullValidationRule(field.Value),
                   new StringMaxLengthValidationRule(field.Value, 15),
                   new ExistePredioValidationRule(this._uow, this._identity.UserId, this._identity.Predio, field.Value)
                },
                OnSuccess = this.ValidatePredioOnSuccess
            };
        }

        public virtual void ValidatePredioOnSuccess(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (!parameters.Any(s => s.Id == "isSubmit"))
            {
                parameters.Add(new ComponentParameter() { Id = "predio", Value = field.Value });
            }
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
                ,
                OnSuccess = this.ValidateEmpresa_OnSucess
            };
        }

        public virtual void ValidateEmpresa_OnSucess(FormField field, Form form, List<ComponentParameter> parameters)
        {
            bool isEmpresaDocumental = this._uow.EmpresaRepository.IsEmpresaDocumental(int.Parse(field.Value));
            parameters.Add(new ComponentParameter() { Id = "isEmpresaDocumental", Value = isEmpresaDocumental ? "S" : "N" });
            parameters.Add(new ComponentParameter() { Id = "empresa", Value = field.Value });
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
                },
                OnSuccess = this.ValidateClienteOnSuccess
            };


        }

        public virtual void ValidateClienteOnSuccess(FormField field, Form form, List<ComponentParameter> parameters)
        {
            string codigoPredio = form.GetField("predio").Value;
            int empresa = int.Parse(form.GetField("empresa").Value);

            Agente agente = this._uow.AgenteRepository.GetAgente(empresa, field.Value);
            var rutaId = agente.Ruta.Id;

            if (!parameters.Any(s => s.Id == "isSubmit"))
            {
                FormField fieldRuta = form.GetField("ruta");

                if (agente.RutasPorDefecto.Any(d => d.Predio == codigoPredio))
                {
                    AgenteRutaPredio rutaPredio = agente.RutasPorDefecto.Where(d => d.Predio == codigoPredio).FirstOrDefault();

                    Ruta ruta = this._uow.RutaRepository.GetRutaOnda(rutaPredio.Ruta);

                    string descRuta = $"{ruta.Id} - {ruta.Descripcion}";
                    descRuta += $" - {(ruta.Onda == null ? "SIN ONDA" : ruta.Onda?.Descripcion)}";

                    if (!string.IsNullOrEmpty(ruta.Zona))
                        descRuta += $" - {ruta.Zona}";

                    fieldRuta.Options = new List<SelectOption>
                    {
                        new SelectOption(ruta.Id.ToString(), descRuta)
                    };
                    fieldRuta.Value = rutaId.ToString();
                }

                parameters.Add(new ComponentParameter() { Id = "cliente", Value = field.Value });
            }
        }

        public virtual FormValidationGroup ValidateRuta(FormField field, Form form, List<ComponentParameter> parameters)
        {

            string predio = form.GetField("predio").Value;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Dependencies = { "predio" },
                Rules = new List<IValidationRule>
                {
                   new NonNullValidationRule(field.Value),
                   new StringMaxLengthValidationRule(field.Value, 3),
                   new PositiveIntValidationRule(field.Value),
                   new ExisteRutaPedidoValidationRule(this._uow, field.Value, predio)
                },
                OnSuccess = this.ValidateRutaOnSuccess
            };
        }

        public virtual void ValidateRutaOnSuccess(FormField field, Form form, List<ComponentParameter> parameters)
        {
            parameters.Add(new ComponentParameter() { Id = "ruta", Value = field.Value });
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
            parameters.Add(new ComponentParameter() { Id = "tpExpedicion", Value = field.Value });
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
                ,
                OnSuccess = this.ValidateTipoPedido_OnSucess
            };
        }

        public virtual void ValidateTipoPedido_OnSucess(FormField field, Form form, List<ComponentParameter> parameters)
        {
            parameters.Add(new ComponentParameter() { Id = "tpPedido", Value = field.Value });
        }

        public virtual FormValidationGroup ValidateFechaEntrega(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value))
                return null;

            var rules = new List<IValidationRule>
            {
                new NonNullValidationRule(field.Value),
                new DateTimeValidationRule(field.Value),

            };


            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = rules,
                OnSuccess = this.ValidateFechaEntrega_OnSucess
            };
        }

        public virtual void ValidateFechaEntrega_OnSucess(FormField field, Form form, List<ComponentParameter> parameters)
        {
            parameters.Add(new ComponentParameter() { Id = "fechaEntrega", Value = field.Value });
        }

        public virtual FormValidationGroup ValidateCondicionStock(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value))
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                   new StringMaxLengthValidationRule(field.Value, 1),
                },
                OnSuccess = this.ValidateCondicionStockOnSuccess
            };
        }

        public virtual void ValidateCondicionStockOnSuccess(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (!parameters.Any(s => s.Id == "isSubmit"))
            {
                parameters.Add(new ComponentParameter() { Id = "condicionStock", Value = field.Value });
            }
        }

        public virtual FormValidationGroup ValidateCantidadPrevVenc(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value))
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                   new NumeroEnteroValidationRule(field.Value),
                   new PositiveIntValidationRule(field.Value)
                },
                OnSuccess = this.ValidateCantidadPrevVencOnSuccess
            };
        }

        public virtual void ValidateCantidadPrevVencOnSuccess(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (!parameters.Any(s => s.Id == "isSubmit"))
            {
                parameters.Add(new ComponentParameter() { Id = "cantidadPrevVenc", Value = field.Value });
            }
        }

        public virtual FormValidationGroup ValidatePermitePickearVencido(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value))
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                   new BooleanStringValidationRule(field.Value),
                },
                OnSuccess = this.ValidatePermitePickearVencidoOnSuccess
            };
        }

        public virtual void ValidatePermitePickearVencidoOnSuccess(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (!parameters.Any(s => s.Id == "isSubmit"))
            {
                var cantidadPrevVenc = form.GetField("cantidadPrevVenc");
                if (field.Value == "false")
                {
                    cantidadPrevVenc.Disabled = false;
                    cantidadPrevVenc.ReadOnly = false;
                }
                else
                {
                    cantidadPrevVenc.ReadOnly = true;
                    cantidadPrevVenc.Disabled = true;
                    cantidadPrevVenc.Value = "";
                }

                parameters.Add(new ComponentParameter() { Id = "permitePickearVencido", Value = field.Value });
            }
        }
    }
}
