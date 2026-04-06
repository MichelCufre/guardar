using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation.Rules;
using WIS.Components.Common;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Extension;
using WIS.FormComponent;
using WIS.FormComponent.Validation;
using WIS.Security;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.FormModules
{
    public class CreateAtributoTipoLpnFormValidationModule : FormValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly IIdentityService _identity;
        protected readonly ISecurityService _security;
        protected readonly IFormatProvider _formatProvider;

        public CreateAtributoTipoLpnFormValidationModule(IUnitOfWork uow, IIdentityService identity, ISecurityService security, IFormatProvider provider)
        {
            this._uow = uow;
            this._identity = identity;
            this._security = security;
            this._formatProvider = provider;

            this.Schema = new FormValidationSchema
            {
                ["ID_ATRIBUTO"] = this.ValidateIdAtributo,
                ["ID_ESTADO_INICIAL"] = this.ValidateEstado,
                ["CD_DOMINIO"] = this.ValidateDominio,
                ["TEXTO"] = this.ValidateTexto,
                ["HORA"] = this.ValidateHora,
                ["FECHA"] = this.ValidateIdFecha,
                ["NUMERO"] = this.ValidateNumero,
                ["ID_CONSOLIDACION_TIPO"] = this.ValidateConsolidacion,
            };
        }

        public virtual FormValidationGroup ValidateIdAtributo(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                   new NonNullValidationRule(field.Value),
                   new StringMaxLengthValidationRule(field.Value, 10),
                },
                OnSuccess = AtributoOnSuccess
            };
        }
        public virtual void AtributoOnSuccess(FormField field, Form form, List<ComponentParameter> parameters)
        {
            parameters.Add(new ComponentParameter { Id = "isDominio", Value = "F" });
            parameters.Add(new ComponentParameter { Id = "isSistema", Value = "F" });
            parameters.Add(new ComponentParameter { Id = "isTexto", Value = "F" });
            parameters.Add(new ComponentParameter { Id = "isHora", Value = "F" });
            parameters.Add(new ComponentParameter { Id = "isFecha", Value = "F" });
            parameters.Add(new ComponentParameter { Id = "isNumerico", Value = "F" });

            form.GetField("FL_REQUERIDO").Disabled = false;
            form.GetField("ID_ESTADO_INICIAL").Disabled = false;
            form.GetField("VL_VALIDO_INTERFAZ").Disabled = false;

            var atributo = _uow.AtributoRepository.GetAtributo(int.Parse(form.GetField("ID_ATRIBUTO").Value));
            var descripcionAtributoTipo = _uow.AtributoRepository.GetDescripcionAtributoTipo(atributo.IdTipo);

            var lpnTipo = _uow.ManejoLpnRepository.GetTipoLpn(parameters.FirstOrDefault(s => s.Id == "LpnTipo").Value);
            if (lpnTipo.PermiteConsolidar == "S")
            {
                parameters.Add(new ComponentParameter { Id = "isRequiredConsolidado", Value = "T" });
                var listaConsolidadorTipo = _uow.ManejoLpnRepository.GetListaLpnConsolidadorTipo();

                var selectConsolidadorTipo = form.GetField("ID_CONSOLIDACION_TIPO");
                selectConsolidadorTipo.Options = new List<SelectOption>();

                foreach (var consolidador in listaConsolidadorTipo)
                {
                    if ((atributo.IdTipo == TipoAtributoDb.NUMERICO)
                        || (atributo.IdTipo != TipoAtributoDb.NUMERICO && consolidador.IdConsolidador != ConsolidadorTipoDb.SUMAR_VALOR_SÓLO_NUMERICOS))
                    {
                        selectConsolidadorTipo.Options.Add(new SelectOption(consolidador.IdConsolidador.ToString(), $"{consolidador.IdConsolidador} - {consolidador.NombreConsolidador}"));
                    }
                }
            }
            else
                parameters.Add(new ComponentParameter { Id = "isRequiredConsolidado", Value = "F" });

            form.GetField("DS_ATRIBUTO_TIPO").Value = descripcionAtributoTipo;

            switch (atributo.IdTipo)
            {
                case TipoAtributoDb.NUMERICO:

                    parameters.RemoveAll(w => w.Id == "isNumerico");
                    parameters.Add(new ComponentParameter { Id = "isNumerico", Value = "T" });

                    break;
                case TipoAtributoDb.FECHA:

                    parameters.RemoveAll(w => w.Id == "isFecha");
                    parameters.Add(new ComponentParameter { Id = "isFecha", Value = "T" });

                    break;
                case TipoAtributoDb.HORA:

                    parameters.RemoveAll(w => w.Id == "isHora");
                    parameters.Add(new ComponentParameter { Id = "isHora", Value = "T" });

                    break;
                case TipoAtributoDb.TEXTO:

                    parameters.RemoveAll(w => w.Id == "isTexto");
                    parameters.Add(new ComponentParameter { Id = "isTexto", Value = "T" });

                    break;
                case TipoAtributoDb.DOMINIO:

                    parameters.RemoveAll(w => w.Id == "isDominio");
                    parameters.Add(new ComponentParameter { Id = "isDominio", Value = "T" });

                    var selectConsolidadorTipo = form.GetField("CD_DOMINIO");
                    selectConsolidadorTipo.Options = new List<SelectOption>();

                    var detallesDominio = _uow.DominioRepository.GetDominios(atributo.CodigoDominio);

                    foreach (var d in detallesDominio)
                    {
                        selectConsolidadorTipo.Options.Add(new SelectOption(d.Id.ToString(), $"{d.Id} - {d.Descripcion}"));
                    }

                    break;
                case TipoAtributoDb.SISTEMA:

                    parameters.RemoveAll(w => w.Id == "isSistema");
                    parameters.Add(new ComponentParameter { Id = "isSistema", Value = "T" });

                    form.GetField("FL_REQUERIDO").Disabled = true;
                    form.GetField("VL_VALIDO_INTERFAZ").Disabled = true;

                    form.GetField("ID_ESTADO_INICIAL").Value = EstadoLpnAtributo.Pendiente;
                    form.GetField("ID_ESTADO_INICIAL").Disabled = true;
                    break;
            }
        }

        public virtual FormValidationGroup ValidateEstado(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new StringMaxLengthValidationRule(field.Value, 6),
                    new EstadoInicialAtributoValidationRule(_uow, field.Value),
                },
            };
        }

        public virtual FormValidationGroup ValidateDominio(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(form.GetField("ID_ATRIBUTO").Value))
                return null;

            var atributo = _uow.AtributoRepository.GetAtributo(int.Parse(form.GetField("ID_ATRIBUTO").Value));

            if (atributo.IdTipo != TipoAtributoDb.DOMINIO)
                return null;

            var estadoInicial = form.GetField("ID_ESTADO_INICIAL").Value;
            var invocarApiCustom = estadoInicial == EstadoLpnAtributo.Pendiente;

            var rules = new List<IValidationRule>
            {
                new StringMaxLengthValidationRule(field.Value, DefaultDb.LARGO_MAXIMO_CHAR_LPN_ATRIBUTO),
                new ValidacionAsociadaAtributosRule(_uow, field.Value, atributo.Id, _formatProvider, invocarApiCustom)
            };

            if (form.GetField("FL_REQUERIDO").Value == "true" && estadoInicial == EstadoLpnAtributo.Ingresado)
                rules.Add(new NonNullValidationRule(field.Value));
            else if (string.IsNullOrEmpty(field.Value))
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Dependencies = { "ID_ESTADO_INICIAL" },
                Rules = rules
            };
        }

        public virtual FormValidationGroup ValidateIdFecha(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(form.GetField("ID_ATRIBUTO").Value))
                return null;

            var atributo = _uow.AtributoRepository.GetAtributo(int.Parse(form.GetField("ID_ATRIBUTO").Value));

            if (atributo.IdTipo != TipoAtributoDb.FECHA)
                return null;

            var estadoInicial = form.GetField("ID_ESTADO_INICIAL").Value;
            var invocarApiCustom = estadoInicial == EstadoLpnAtributo.Pendiente;
            var date = DateTimeExtension.ParseFromIso(field.Value)?.Date.ToString(atributo.MascaraIngreso);

            var rules = new List<IValidationRule>
            {
                new StringMaxLengthValidationRule(field.Value, DefaultDb.LARGO_MAXIMO_CHAR_LPN_ATRIBUTO),
                new ValidacionAsociadaAtributosRule(_uow,date , atributo.Id, _formatProvider, invocarApiCustom)
            };

            if (form.GetField("FL_REQUERIDO").Value == "true" && estadoInicial == EstadoLpnAtributo.Ingresado)
                rules.Add(new NonNullValidationRule(field.Value));
            else if (string.IsNullOrEmpty(field.Value))
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Dependencies = { "ID_ESTADO_INICIAL" },
                Rules = rules
            };
        }

        public virtual FormValidationGroup ValidateTexto(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(form.GetField("ID_ATRIBUTO").Value))
                return null;

            var atributo = _uow.AtributoRepository.GetAtributo(int.Parse(form.GetField("ID_ATRIBUTO").Value));

            if (atributo.IdTipo != TipoAtributoDb.TEXTO)
                return null;

            var estadoInicial = form.GetField("ID_ESTADO_INICIAL").Value;
            var invocarApiCustom = estadoInicial == EstadoLpnAtributo.Pendiente;

            var rules = new List<IValidationRule>
            {
                new StringMaxLengthValidationRule(field.Value, DefaultDb.LARGO_MAXIMO_CHAR_LPN_ATRIBUTO),
                new ValidacionAsociadaAtributosRule(_uow, field.Value, atributo.Id, _formatProvider, invocarApiCustom)
            };

            if (form.GetField("FL_REQUERIDO").Value == "true" && estadoInicial == EstadoLpnAtributo.Ingresado)
                rules.Add(new NonNullValidationRule(field.Value));
            else if (string.IsNullOrEmpty(field.Value))
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Dependencies = { "ID_ESTADO_INICIAL" },
                Rules = rules
            };
        }

        public virtual FormValidationGroup ValidateNumero(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(form.GetField("ID_ATRIBUTO").Value))
                return null;

            var atributo = _uow.AtributoRepository.GetAtributo(int.Parse(form.GetField("ID_ATRIBUTO").Value));

            if (atributo.IdTipo != TipoAtributoDb.NUMERICO)
                return null;

            var estadoInicial = form.GetField("ID_ESTADO_INICIAL").Value;
            var invocarApiCustom = estadoInicial == EstadoLpnAtributo.Pendiente;

            var rules = new List<IValidationRule>
            {
                new DecimalLengthWithPresicionValidationRule(field.Value, atributo.Largo ?? 0, int.Parse((atributo.Decimales ?? 0).ToString()), this._formatProvider, separador:atributo.Separador),
                new ValidacionAsociadaAtributosRule(_uow, field.Value, atributo.Id, _formatProvider, invocarApiCustom)
            };

            if (form.GetField("FL_REQUERIDO").Value == "true" && estadoInicial == EstadoLpnAtributo.Ingresado)
                rules.Add(new NonNullValidationRule(field.Value));
            else if (string.IsNullOrEmpty(field.Value))
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Dependencies = { "ID_ESTADO_INICIAL" },
                Rules = rules
            };
        }

        public virtual FormValidationGroup ValidateHora(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(form.GetField("ID_ATRIBUTO").Value))
                return null;

            var atributo = _uow.AtributoRepository.GetAtributo(int.Parse(form.GetField("ID_ATRIBUTO").Value));

            if (atributo.IdTipo != TipoAtributoDb.HORA)
                return null;

            var estadoInicial = form.GetField("ID_ESTADO_INICIAL").Value;
            var invocarApiCustom = estadoInicial == EstadoLpnAtributo.Pendiente;

            if (field.Value == "__:__")
                field.Value = "";

            var rules = new List<IValidationRule>
            {
                new StringMaxLengthValidationRule(field.Value, DefaultDb.LARGO_MAXIMO_CHAR_LPN_ATRIBUTO),
                new TimeSpanValidationRule(field.Value, _formatProvider),
            };

            if (form.GetField("FL_REQUERIDO").Value == "true" && estadoInicial == EstadoLpnAtributo.Ingresado)
                rules.Add(new NonNullValidationRule(field.Value));
            else if (string.IsNullOrEmpty(field.Value))
                return null;

            rules.Add(new ValidacionAsociadaAtributosRule(_uow, DateTime.ParseExact(field.Value, CDateFormats.HORA_MINUTOS, _identity.GetFormatProvider()).ToString(atributo.MascaraIngreso), atributo.Id, _formatProvider, invocarApiCustom));

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Dependencies = { "ID_ESTADO_INICIAL" },
                Rules = rules
            };
        }

        public virtual FormValidationGroup ValidateConsolidacion(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(form.GetField("ID_ATRIBUTO").Value))
                return null; 
            
            var atributo = _uow.AtributoRepository.GetAtributo(int.Parse(form.GetField("ID_ATRIBUTO").Value));
            var lpnTipo = _uow.ManejoLpnRepository.GetTipoLpn(parameters.FirstOrDefault(s => s.Id == "LpnTipo").Value);

            if (lpnTipo.PermiteConsolidar == "N")
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new StringMaxLengthValidationRule(field.Value, DefaultDb.LARGO_MAXIMO_CHAR_LPN_ATRIBUTO)
                },
            };
        }
    }
}
