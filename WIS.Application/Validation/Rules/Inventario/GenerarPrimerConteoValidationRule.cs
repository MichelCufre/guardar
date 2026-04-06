using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Persistence.Database;
using WIS.Validation;

namespace WIS.Application.Validation.Rules
{
    public class GenerarPrimerConteoValidationRule : IValidationRule
    {
        protected readonly string _tipoCierreConteo;
        protected readonly string _value;

        public GenerarPrimerConteoValidationRule(string value, string tipoCierreConteo)
        {
            this._value = value;
            this._tipoCierreConteo = tipoCierreConteo;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            bool generaConteo;
            if (bool.TryParse(_value, out bool parsedValue))
                generaConteo = parsedValue;
            else
            {
                generaConteo = _value == "S";
            }

            if (_tipoCierreConteo == TipoCierreConteoInventario.UnConteo && generaConteo)
                errors.Add(new ValidationError("INV410_msg_Error_TipoConteoNoPermiteGenerarPrimerConteo"));

            return errors;
        }
    }
}
