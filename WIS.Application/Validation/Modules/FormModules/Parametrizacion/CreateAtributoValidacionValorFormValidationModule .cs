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
using WIS.FormComponent;
using WIS.FormComponent.Validation;
using WIS.Security;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.FormModules
{
    public class CreateAtributoValidacionValorFormValidationModule : FormValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly IIdentityService _identity;
        protected readonly ISecurityService _security;
        protected readonly AtributoMapper _atributoMapper;
        protected readonly IFormatProvider _culture;
        public CreateAtributoValidacionValorFormValidationModule(IUnitOfWork uow, IIdentityService identity, ISecurityService security)
        {
            _culture = identity.GetFormatProvider();
            this._uow = uow;
            this._identity = identity;
            this._security = security;
            _atributoMapper = new AtributoMapper();
            this.Schema = new FormValidationSchema
            {

                ["TEXTO"] = this.ValidateTexto,
                ["HORA"] = this.ValidateHora,
                ["FECHA"] = this.ValidateIdFecha,
                ["NUMERO"] = this.ValidateLargo,
                ["DECIMAL"] = this.ValidateDecimal,
                ["URL"] = this.ValidateURL,
            };
        }
        public virtual FormValidationGroup ValidateIdFecha(FormField field, Form form, List<ComponentParameter> parameters)
        {


            List<AtributoValidacion> listaAtributoValidacion = JsonConvert.DeserializeObject<List<AtributoValidacion>>(parameters.FirstOrDefault(x => x.Id == "listValidaciones").Value);
            AtributoValidacion registro = listaAtributoValidacion.OrderBy(x => x.Id).FirstOrDefault();
            if (registro.TipoArgumento != TipoAtributoValidacionDb.FECHA)
                return null;


            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                   new NonNullValidationRule(field.Value),
                   new StringMaxLengthValidationRule(field.Value, DefaultDb.LARGO_MAXIMO_CHAR_VL_ARGUMENTO),
                },
            };

        }

        public virtual FormValidationGroup ValidateLargo(FormField field, Form form, List<ComponentParameter> parameters)
        {
            List<AtributoValidacion> listaAtributoValidacion = JsonConvert.DeserializeObject<List<AtributoValidacion>>(parameters.FirstOrDefault(x => x.Id == "listValidaciones").Value);
            AtributoValidacion registro = listaAtributoValidacion.OrderBy(x => x.Id).FirstOrDefault();
            if (registro.TipoArgumento != TipoAtributoValidacionDb.NUMERICO)
                return null;

            decimal.TryParse(field.Value, out decimal valor);
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                   new NonNullValidationRule(field.Value),
                   new PositiveNumberMaxLengthValidationRule(field.Value,3),
                   new PositiveIntLessThanValidationRule(field.Value, DefaultDb.LARGO_MAXIMO_CHAR_VL_ARGUMENTO)
                },
            };

        }
        public virtual FormValidationGroup ValidateTexto(FormField field, Form form, List<ComponentParameter> parameters)
        {
            List<AtributoValidacion> listaAtributoValidacion = JsonConvert.DeserializeObject<List<AtributoValidacion>>(parameters.FirstOrDefault(x => x.Id == "listValidaciones").Value);
            AtributoValidacion registro = listaAtributoValidacion.OrderBy(x => x.Id).FirstOrDefault();
            if (registro.TipoArgumento != TipoAtributoValidacionDb.TEXTO)
                return null;


            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                   new NonNullValidationRule(field.Value),
                   new StringMaxLengthValidationRule(field.Value, DefaultDb.LARGO_MAXIMO_CHAR_VL_ARGUMENTO)
                },
            };

        }
        public virtual FormValidationGroup ValidateURL(FormField field, Form form, List<ComponentParameter> parameters)
        {
            List<AtributoValidacion> listaAtributoValidacion = JsonConvert.DeserializeObject<List<AtributoValidacion>>(parameters.FirstOrDefault(x => x.Id == "listValidaciones").Value);
            AtributoValidacion registro = listaAtributoValidacion.OrderBy(x => x.Id).FirstOrDefault();
            if (registro.TipoArgumento != TipoAtributoValidacionDb.URL)
                return null;


            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                   new NonNullValidationRule(field.Value),
                   new StringMaxLengthValidationRule(field.Value, DefaultDb.LARGO_MAXIMO_CHAR_VL_ARGUMENTO),
                   new StringUrlValidationRule(field.Value)
                },
            };

        }

        public virtual FormValidationGroup ValidateDecimal(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var atributo = _uow.AtributoRepository.GetAtributo(int.Parse(parameters.FirstOrDefault(x => x.Id == "codigoAtributo").Value));
            List<AtributoValidacion> listaAtributoValidacion = JsonConvert.DeserializeObject<List<AtributoValidacion>>(parameters.FirstOrDefault(x => x.Id == "listValidaciones").Value);
            AtributoValidacion registro = listaAtributoValidacion.OrderBy(x => x.Id).FirstOrDefault();
            if (registro.TipoArgumento != TipoAtributoValidacionDb.DECIMAL)
                return null;


            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                   new NonNullValidationRule(field.Value),
                    new DecimalLengthWithPresicionValidationRule(field.Value, atributo.Largo??0, int.Parse((atributo.Decimales ?? 0).ToString()), this._culture),
                },
            };

        }

        public virtual FormValidationGroup ValidateHora(FormField field, Form form, List<ComponentParameter> parameters)
        {
            List<AtributoValidacion> listaAtributoValidacion = JsonConvert.DeserializeObject<List<AtributoValidacion>>(parameters.FirstOrDefault(x => x.Id == "listValidaciones").Value);
            AtributoValidacion registro = listaAtributoValidacion.OrderBy(x => x.Id).FirstOrDefault();
            if (registro.TipoArgumento != TipoAtributoValidacionDb.HORA)
                return null;

            if (field.Value == "__:__")
                field.Value = "";
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                   new NonNullValidationRule(field.Value),
                   new StringMaxLengthValidationRule(field.Value, DefaultDb.LARGO_MAXIMO_CHAR_VL_ARGUMENTO),
                   new TimeSpanValidationRule(field.Value,_culture)
                },
            };

        }

    }
}
