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
using WIS.Domain.DataModel.Mappers.Constants;
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
    public class CreatePedidoFormValidationModule : FormValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly IIdentityService _identity;
        protected readonly ISecurityService _security;
        protected readonly ConfiguracionPedido _configuracion;

        public CreatePedidoFormValidationModule(IUnitOfWork uow, IIdentityService identity, ISecurityService security, ConfiguracionPedido configuracion)
        {
            this._uow = uow;
            this._identity = identity;
            this._security = security;
            this._configuracion = configuracion;

            this.Schema = new FormValidationSchema
            {
                ["pedido"] = this.ValidatePedido,
                ["predio"] = this.ValidatePredio,
                ["empresa"] = this.ValidateEmpresa,
                ["cliente"] = this.ValidateCliente,
                ["ruta"] = this.ValidateRuta,
                ["tipoExpedicion"] = this.ValidateTipoExpedicion,
                ["tipoPedido"] = this.ValidateTipoPedido,
                ["liberarDesde"] = this.ValidateLiberarDesde,
                ["liberarHasta"] = this.ValidateLiberarHasta,
                ["fechaEmision"] = this.ValidateFechaEmision,
                ["fechaEntrega"] = this.ValidateFechaEntrega,
                ["memo"] = this.ValidateMemo,
                ["direccionEntrega"] = this.ValidateDireccionEntrega,
                ["anexo"] = this.ValidateAnexo,
                ["idReserva"] = this.ValidateIdentificadorReserva,
                ["ComparteContenedorPicking"] = this.ValidateComparteContenedorPicking,
                ["ComparteContenedorEntrega"] = this.ValidateComparteContenedorEntrega,
                ["telofonoPrincipal"] = this.ValidateTelefonoPrincipal,
                ["telefonoSecundario"] = this.ValidateTelefonoSecundario,
                ["latitud"] = this.ValidateLatitud,
                ["longitud"] = this.ValidateLongitud,
                ["zona"] = this.ValidateZona,
            };
        }

        public virtual FormValidationGroup ValidatePedido(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var empresa = form.GetField("empresa").Value;
            var cliente = form.GetField("cliente").Value;

            if (string.IsNullOrEmpty(field.Value))
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
                Dependencies = { "empresa", "cliente" }
            };
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
                form.GetField("ruta").Value = string.Empty;
                form.GetField("ruta").Options.Clear();
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

            if (!parameters.Any(s => s.Id == "isSubmit") && !string.IsNullOrEmpty(agente.Direccion))
            {
                form.GetField("direccionEntrega").Value = agente.Direccion;

                FormField fieldRuta = form.GetField("ruta");
                if (string.IsNullOrEmpty(codigoPredio))
                {
                    var ruta = _uow.RutaRepository.GetRutaOnda(rutaId);

                    string descRuta = $"{ruta.Id} - {ruta.Descripcion}";
                    descRuta += $" - {(ruta.Onda == null ? "SIN ONDA" : ruta.Onda?.Descripcion)}";

                    if (!string.IsNullOrEmpty(ruta.Zona))
                        descRuta += $" - {ruta.Zona}";

                    fieldRuta.Options = new List<SelectOption>
                {
                    new SelectOption(rutaId.ToString(), descRuta)
                };
                    fieldRuta.Value = rutaId.ToString();
                }
                else
                {
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
                }
            }
        }

        public virtual FormValidationGroup ValidateRuta(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value))
                return null;

            string predio = form.GetField("predio").Value;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                //Dependencies = { "predio" },
                Rules = new List<IValidationRule>
                {
                   new NonNullValidationRule(field.Value),
                   new StringMaxLengthValidationRule(field.Value, 3),
                   new PositiveIntValidationRule(field.Value),
                   new ExisteRutaPedidoValidationRule(this._uow, field.Value, predio)
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

        public virtual FormValidationGroup ValidateLiberarDesde(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value))
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                   new NonNullValidationRule(field.Value),
                   new DateTimeValidationRule(field.Value)
                }
            };
        }

        public virtual FormValidationGroup ValidateLiberarHasta(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value))
                return null;

            string fechaDesde = form.GetField("liberarDesde").Value;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Dependencies = { "liberarDesde" },
                Rules = new List<IValidationRule>
                {
                   new NonNullValidationRule(field.Value),
                   new DateTimeValidationRule(field.Value),
                   new DateTimeGreaterThanValidationRule(field.Value, fechaDesde, "PRE100_frm1_error_FechaHastaMenorDesde"),
                   new DateTimeGreaterThanCurrentDateValidationRule(field.Value)
                }
            };
        }

        public virtual FormValidationGroup ValidateFechaEmision(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value))
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                   new NonNullValidationRule(field.Value),
                   new DateTimeValidationRule(field.Value)
                }
            };
        }

        public virtual FormValidationGroup ValidateFechaEntrega(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value))
                return null;

            string fechaEmision = form.GetField("fechaEmision").Value;

            var rules = new List<IValidationRule>
            {
                new NonNullValidationRule(field.Value),
                new DateTimeValidationRule(field.Value),
                new DateTimeGreaterThanValidationRule(field.Value, fechaEmision, "PRE100_frm1_error_FechaEntregaMenorEmision")
            };

            if (this._configuracion.ValidarHorasEntreEmisionEntrega)
                rules.Add(new FechaEntregaDiferenciaPedidoValidationRule(field.Value, fechaEmision));

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Dependencies = { "fechaEmision" },
                Rules = rules
            };
        }

        public virtual FormValidationGroup ValidateMemo(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value))
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new StringMaxLengthValidationRule(field.Value, 1000)
                }
            };
        }

        public virtual FormValidationGroup ValidateDireccionEntrega(FormField field, Form form, List<ComponentParameter> parameters)
        {
            string tipoExpedicion = form.GetField("tipoExpedicion").Value;

            int empresa = -1;
            if (int.TryParse(form.GetField("empresa")?.Value, out int value))
                empresa = value;

            string idCliente = form.GetField("cliente")?.Value;
            bool trackingActivo = _uow.ParametroRepository.GetParameter("TRACKING_ACTIVO") == CEstadoTracking.Activo ? true : false;
            ConfiguracionExpedicionPedido configuracion = _uow.PedidoRepository.GetConfiguracionExpedicion(tipoExpedicion);

            var rules = new List<IValidationRule>
            {
                new StringMaxLengthValidationRule(field.Value, 400)
            };

            if (string.IsNullOrEmpty(field.Value))
            {
                if (configuracion != null && configuracion.FlTracking && trackingActivo)
                {
                    Agente cliente = _uow.AgenteRepository.GetAgente(empresa, idCliente);

                    if (string.IsNullOrEmpty(cliente?.Direccion))
                        rules.Add(new NonNullValidationRule(field.Value));
                }
                else
                    return null;
            }
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = rules,
                Dependencies = { "empresa", "cliente", "tipoExpedicion" }
            };
        }

        public virtual FormValidationGroup ValidateAnexo(FormField field, Form form, List<ComponentParameter> parameters)
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

        public virtual FormValidationGroup ValidateIdentificadorReserva(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var tipoPedido = form.GetField("tipoPedido").Value;

            var rules = new List<IValidationRule> 
            { 
                new StringMaxLengthValidationRule(field.Value, 200) 
            };

            if (tipoPedido == TipoPedidoDb.Reserva)
                rules.Add(new NonNullValidationRule(field.Value));

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = rules,
                Dependencies = { "tipoPedido" }
            };
        }

        public virtual FormValidationGroup ValidateComparteContenedorPicking(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value))
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new StringMaxLengthValidationRule(field.Value, 200)
                }
            };
        }

        public virtual FormValidationGroup ValidateComparteContenedorEntrega(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value))
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new StringMaxLengthValidationRule(field.Value, 200)
                }
            };
        }

        public virtual FormValidationGroup ValidateTelefonoPrincipal(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (!string.IsNullOrEmpty(field.Value))
                return new FormValidationGroup
                {
                    BreakValidationChain = false,
                    Rules = new List<IValidationRule>
                    {
                        new StringMaxLengthValidationRule(field.Value,30)
                    }
                };
            else
                return null;
        }

        public virtual FormValidationGroup ValidateTelefonoSecundario(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (!string.IsNullOrEmpty(field.Value))
                return new FormValidationGroup
                {
                    BreakValidationChain = false,
                    Rules = new List<IValidationRule>
                    {
                        new StringMaxLengthValidationRule(field.Value,30)
                    }
                };
            else
                return null;
        }

        public virtual FormValidationGroup ValidateLatitud(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (!string.IsNullOrEmpty(field.Value))
                return new FormValidationGroup
                {
                    BreakValidationChain = false,
                    Rules = new List<IValidationRule>
                    {
                        new PositiveOrNegativeDecimalValidationRule(this._identity.GetFormatProvider(), field.Value),
                        new DecimalLengthWithPresicionValidationRule(field.Value, 9, 7, this._identity.GetFormatProvider()),
                        new DecimalLowerThanValidationRule(this._identity.GetFormatProvider(), field.Value, 90),
                        new DecimalGreaterThanValidationRule(this._identity.GetFormatProvider(), field.Value, -90),
                    }
                };
            else
                return null;
        }

        public virtual FormValidationGroup ValidateLongitud(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (!string.IsNullOrEmpty(field.Value))
                return new FormValidationGroup
                {
                    BreakValidationChain = false,
                    Rules = new List<IValidationRule>
                    {
                        new PositiveOrNegativeDecimalValidationRule(this._identity.GetFormatProvider(), field.Value),
                        new DecimalLengthWithPresicionValidationRule(field.Value, 10, 7, this._identity.GetFormatProvider()),
                        new DecimalLowerThanValidationRule(this._identity.GetFormatProvider(), field.Value, 180),
                        new DecimalGreaterThanValidationRule(this._identity.GetFormatProvider(), field.Value, -180),
                    }
                };
            else
                return null;
        }

        public virtual FormValidationGroup ValidateZona(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (!string.IsNullOrEmpty(field.Value))
                return new FormValidationGroup
                {
                    BreakValidationChain = false,
                    Rules = new List<IValidationRule>
                    {
                        new ZonaNoExisteValidationRule(_uow, field.Value)
                    }
                };
            else
                return null;
        }
    }
}
