using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation.Rules;
using WIS.Components.Common;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Extension;
using WIS.FormComponent;
using WIS.FormComponent.Validation;
using WIS.Security;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.FormModules
{
    public class LpnDetalleAtributoFormValidationModule : FormValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly IIdentityService _identity;
        protected readonly ISecurityService _security;
        protected readonly IFormatProvider _formatProvider;

        public LpnDetalleAtributoFormValidationModule(IUnitOfWork uow, IIdentityService identity, ISecurityService security, IFormatProvider provider)
        {
            this._uow = uow;
            this._identity = identity;
            this._security = security;
            this._formatProvider = provider;

            this.Schema = new FormValidationSchema
            {
                ["CD_DOMINIO"] = this.ValidateDominio,
                ["TEXTO"] = this.ValidateTexto,
                ["HORA"] = this.ValidateHora,
                ["FECHA"] = this.ValidateIdFecha,
                ["NUMERO"] = this.ValidateNumero,
            };
        }

        public virtual FormValidationGroup ValidateDominio(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var atributo = _uow.AtributoRepository.GetAtributo(int.Parse(parameters.FirstOrDefault(x => x.Id == "IdAtributo").Value));
            var tpLpnTipo = parameters.FirstOrDefault(x => x.Id == "LpnTipo").Value;

            if (atributo.IdTipo != TipoAtributoDb.DOMINIO)
                return null;

            var rules = new List<IValidationRule>
            {
                new StringMaxLengthValidationRule(field.Value, DefaultDb.LARGO_MAXIMO_CHAR_LPN_ATRIBUTO),
                new ValidacionAsociadaAtributosRule(_uow, field.Value, atributo.Id, _formatProvider, invocarAPICustom:true)
            };

            var requerido = (_uow.ManejoLpnRepository.GetLpnAtributoTipoDet(atributo.Id, tpLpnTipo).Requerido == "S");

            if (requerido)
                rules.Add(new NonNullValidationRule(field.Value));
            else if (string.IsNullOrEmpty(field.Value))
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = rules
            };
        }

        public virtual FormValidationGroup ValidateIdFecha(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var atributo = _uow.AtributoRepository.GetAtributo(int.Parse(parameters.FirstOrDefault(x => x.Id == "IdAtributo").Value));
            var tpLpnTipo = parameters.FirstOrDefault(x => x.Id == "LpnTipo").Value;

            if (atributo.IdTipo != TipoAtributoDb.FECHA)
                return null;

            string date = DateTimeExtension.ParseFromIso(field.Value)?.Date.ToString(atributo.MascaraIngreso);

            var rules = new List<IValidationRule>
            {
                new StringMaxLengthValidationRule(field.Value, DefaultDb.LARGO_MAXIMO_CHAR_LPN_ATRIBUTO),
                new ValidacionAsociadaAtributosRule(_uow, date, atributo.Id, _formatProvider, invocarAPICustom:true)
            };

            var requerido = (_uow.ManejoLpnRepository.GetLpnAtributoTipoDet(atributo.Id, tpLpnTipo).Requerido == "S");

            if (requerido)
                rules.Add(new NonNullValidationRule(field.Value));
            else if (string.IsNullOrEmpty(field.Value))
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = rules
            };
        }

        public virtual FormValidationGroup ValidateTexto(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var atributo = _uow.AtributoRepository.GetAtributo(int.Parse(parameters.FirstOrDefault(x => x.Id == "IdAtributo").Value));
            var tpLpnTipo = parameters.FirstOrDefault(x => x.Id == "LpnTipo").Value;

            if (atributo.IdTipo != TipoAtributoDb.TEXTO)
                return null;

            var rules = new List<IValidationRule>
            {
                new StringMaxLengthValidationRule(field.Value, DefaultDb.LARGO_MAXIMO_CHAR_LPN_ATRIBUTO),
                new ValidacionAsociadaAtributosRule(_uow, field.Value, atributo.Id, _formatProvider, invocarAPICustom:true)
            };

            var requerido = (_uow.ManejoLpnRepository.GetLpnAtributoTipoDet(atributo.Id, tpLpnTipo).Requerido == "S");
            if (requerido)
                rules.Add(new NonNullValidationRule(field.Value));
            else if (string.IsNullOrEmpty(field.Value))
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = rules
            };
        }

        public virtual FormValidationGroup ValidateNumero(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var atributo = _uow.AtributoRepository.GetAtributo(int.Parse(parameters.FirstOrDefault(x => x.Id == "IdAtributo").Value));
            var tpLpnTipo = parameters.FirstOrDefault(x => x.Id == "LpnTipo").Value;

            if (atributo.IdTipo != TipoAtributoDb.NUMERICO)
                return null;

            var rules = new List<IValidationRule>
            {
                new DecimalLengthWithPresicionValidationRule(field.Value, atributo.Largo??0, int.Parse((atributo.Decimales ?? 0).ToString()),  this._formatProvider),
                new ValidacionAsociadaAtributosRule(_uow, field.Value, atributo.Id, _formatProvider, invocarAPICustom:true)
            };

            var requerido = (_uow.ManejoLpnRepository.GetLpnAtributoTipoDet(atributo.Id, tpLpnTipo).Requerido == "S");

            if (requerido)
                rules.Add(new NonNullValidationRule(field.Value));
            else if (string.IsNullOrEmpty(field.Value))
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = rules,
            };
        }

        public virtual FormValidationGroup ValidateHora(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var atributo = _uow.AtributoRepository.GetAtributo(int.Parse(parameters.FirstOrDefault(x => x.Id == "IdAtributo").Value));
            var tpLpnTipo = parameters.FirstOrDefault(x => x.Id == "LpnTipo").Value;

            if (atributo.IdTipo != TipoAtributoDb.HORA)
                return null;

            if (field.Value == "__:__")
                field.Value = "";

            var rules = new List<IValidationRule>
            {
                new StringMaxLengthValidationRule(field.Value, DefaultDb.LARGO_MAXIMO_CHAR_LPN_ATRIBUTO),
                new TimeSpanValidationRule(field.Value,_formatProvider),
            };

            var requerido = (_uow.ManejoLpnRepository.GetLpnAtributoTipoDet(atributo.Id, tpLpnTipo).Requerido == "S");
            if (requerido)
                rules.Add(new NonNullValidationRule(field.Value));
            else if (string.IsNullOrEmpty(field.Value))
                return null;

            rules.Add(new ValidacionAsociadaAtributosRule(_uow, DateTime.ParseExact(field.Value, CDateFormats.HORA_MINUTOS, _identity.GetFormatProvider()).ToString(atributo.MascaraIngreso), atributo.Id, _formatProvider, invocarAPICustom: true));

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = rules
            };
        }
    }
}
