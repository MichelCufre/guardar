using System;
using System.Collections.Generic;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.General
{
    public class ManejoIdentificadorSerieAceptaDecimalesValidationRule : IValidationRule
    {
        protected readonly string _field;
        protected readonly string _manejoIdentificador;

        public ManejoIdentificadorSerieAceptaDecimalesValidationRule(string field, string manejoIdentificador)
        {
            this._field = field;
            this._manejoIdentificador = manejoIdentificador;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();
            if (_manejoIdentificador == ManejoIdentificadorDb.Serie && _field == "S")
            {
                errors.Add(new ValidationError("REG009_Sec0_Error_TipoIdentificadorSerieNoAceptaDecimales"));
            }

            return errors;
        }
    }
}
