using DocumentFormat.OpenXml.InkML;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation.Rules;
using WIS.Components.Common;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Parametrizacion;
using WIS.Extension;
using WIS.FormComponent;
using WIS.FormComponent.Validation;
using WIS.Persistence.Database;
using WIS.Security;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.FormModules
{
    public class PedidoDefinicionAtributosFormValidationModule : FormValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly IIdentityService _identity;

        public PedidoDefinicionAtributosFormValidationModule(IUnitOfWork uow, IIdentityService identity)
        {
            this._uow = uow;
            this._identity = identity;

            this.Schema = new FormValidationSchema
            {
                ["inputField"] = this.ValidateTextoNumero,
                ["inputFieldTime"] = this.ValidateHora,
                ["inputFieldDateTime"] = this.ValidateFecha,
                ["inputFieldSelect"] = this.ValidateDominio
            };
        }

        public virtual FormValidationGroup ValidateTextoNumero(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var listaAtributos = JsonConvert.DeserializeObject<List<KeyAtributoTipo>>(parameters.FirstOrDefault(x => x.Id == "listAtributos").Value);
            var atributoDefinicion = listaAtributos.OrderBy(x => x.IdAtributo).FirstOrDefault();
            var atributo = _uow.AtributoRepository.GetAtributo(atributoDefinicion.IdAtributo);

            var rules = new List<IValidationRule>
            {
                new NonNullValidationRule(field.Value)
            };

            switch (atributo.IdTipo)
            {
                case TipoAtributoDb.TEXTO:
                    rules.Add(new StringMaxLengthValidationRule(field.Value, DefaultDb.LARGO_MAXIMO_CHAR_LPN_ATRIBUTO));
                    break;
                case TipoAtributoDb.NUMERICO:
                    rules.Add(new DecimalLengthWithPresicionValidationRule(field.Value, atributo.Largo ?? 0, int.Parse((atributo.Decimales ?? 0).ToString()), _identity.GetFormatProvider()));
                    break;
                default: return null;
            }

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = rules
            };
        }

        public virtual FormValidationGroup ValidateHora(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var listaAtributos = JsonConvert.DeserializeObject<List<KeyAtributoTipo>>(parameters.FirstOrDefault(x => x.Id == "listAtributos").Value);
            var atributoDefinicion = listaAtributos.OrderBy(x => x.IdAtributo).FirstOrDefault();
            var atributo = _uow.AtributoRepository.GetAtributo(atributoDefinicion.IdAtributo);

            if (atributo.IdTipo != TipoAtributoDb.HORA)
                return null;

            if (field.Value == "__:__")
                field.Value = string.Empty;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                   new NonNullValidationRule(field.Value),
                   new StringMaxLengthValidationRule(field.Value, DefaultDb.LARGO_MAXIMO_CHAR_LPN_ATRIBUTO),
                   new TimeSpanValidationRule(field.Value,_identity.GetFormatProvider())
                },
            };

        }

        public virtual FormValidationGroup ValidateFecha(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var listaAtributos = JsonConvert.DeserializeObject<List<KeyAtributoTipo>>(parameters.FirstOrDefault(x => x.Id == "listAtributos").Value);
            var atributoDefinicion = listaAtributos.OrderBy(x => x.IdAtributo).FirstOrDefault();
            var atributo = _uow.AtributoRepository.GetAtributo(atributoDefinicion.IdAtributo);

            if (atributo.IdTipo != TipoAtributoDb.FECHA)
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

        public virtual FormValidationGroup ValidateDominio(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var listaAtributos = JsonConvert.DeserializeObject<List<KeyAtributoTipo>>(parameters.FirstOrDefault(x => x.Id == "listAtributos").Value);
            var atributoDefinicion = listaAtributos.OrderBy(x => x.IdAtributo).FirstOrDefault();
            var atributo = _uow.AtributoRepository.GetAtributo(atributoDefinicion.IdAtributo);

            if (atributo.IdTipo != TipoAtributoDb.DOMINIO)
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
