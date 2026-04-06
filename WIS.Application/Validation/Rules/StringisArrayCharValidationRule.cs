using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Validation;

namespace WIS.Application.Validation.Rules
{
    public class StringisArrayCharValidationRule : IValidationRule
    {
        protected readonly string _value;
        protected readonly string _TipoArgumento;
        protected readonly string _SeparadorCaracteres;


        public StringisArrayCharValidationRule(string value, string TipoArgumento, string SeparadorCaracteres)
        {
            this._value = value;
            this._TipoArgumento = TipoArgumento;
            this._SeparadorCaracteres = SeparadorCaracteres;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();
            if (_TipoArgumento == TipoAtributoValidacionDb.TEXTO && !string.IsNullOrEmpty(this._value))
            {
                string[] arraystring = _value.Split(_SeparadorCaracteres);
                foreach (var valorArray in arraystring)
                {
                    if (valorArray.Length > 1)
                    {
                        errors.Add(new ValidationError("General_Sec0_Error_StringisArrayChar", new List<string> { _SeparadorCaracteres }));
                        return errors;
                    }
                }

            }

            return errors;
        }
    }
}
