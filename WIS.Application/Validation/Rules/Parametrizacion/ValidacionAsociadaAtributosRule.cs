using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Domain.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Rules
{
    public class ValidacionAsociadaAtributosRule : IValidationRule
    {
        protected readonly string _value;
        protected readonly int _idAtributo;
        protected readonly bool _invocarAPICustom;
        protected readonly IUnitOfWork _uow;
        protected readonly IFormatProvider _culture;

        public ValidacionAsociadaAtributosRule(IUnitOfWork uow, string value, int idAtributo, IFormatProvider culture, bool invocarAPICustom)
        {
            this._value = value;
            this._idAtributo = idAtributo;
            this._uow = uow;
            this._culture = culture;
            this._invocarAPICustom = invocarAPICustom;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();
            if (!string.IsNullOrEmpty(this._value))
            {
                AtributoValidation.ValidacionAsociadaAtributos(_uow, _idAtributo, _value, _culture, _invocarAPICustom, out List<Error> errores);
                if (errores.Count > 0)
                {
                    foreach (var error in errores)
                    {
                        var arguments = error.GetArgumentos();
                        errors.Add(new ValidationError(error.Mensaje, arguments));
                    }
                }
            }
            return errors;
        }
    }
}
