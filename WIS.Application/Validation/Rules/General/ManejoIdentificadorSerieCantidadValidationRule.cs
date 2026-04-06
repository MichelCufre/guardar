using System;
using System.Collections.Generic;
using WIS.Domain.General.Enums;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.General
{
    public class ManejoIdentificadorSerieCantidadValidationRule : IValidationRule
    {
        protected readonly string _cantidad;
        protected readonly ManejoIdentificador _manejoIdentificador;
        protected readonly IFormatProvider _culture;
        protected readonly bool _allowZero;

        public ManejoIdentificadorSerieCantidadValidationRule(string cantidad, ManejoIdentificador manejoIdentificador, IFormatProvider culture, bool allowZero = false)
        {
            this._cantidad = cantidad;
            this._manejoIdentificador = manejoIdentificador;
            this._culture = culture;
            this._allowZero = allowZero;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (string.IsNullOrEmpty(_cantidad))
                return errors;

            var cantidad = decimal.Parse(_cantidad, _culture);

            if (_manejoIdentificador == ManejoIdentificador.Serie)
            {
                if (_allowZero && (cantidad != 1 && cantidad != 0))
                    errors.Add(new ValidationError("General_msg_Error_TipoSerieCantidadDistintaAUnoyCero"));
                else if (!_allowZero && cantidad != 1)
                    errors.Add(new ValidationError("General_msg_Error_TipoSerieCantidadDistintaAUno"));
            }

            return errors;
        }
    }
}
