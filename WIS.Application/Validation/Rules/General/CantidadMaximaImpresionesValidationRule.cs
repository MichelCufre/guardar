using System;
using System.Collections.Generic;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General.Enums;
using WIS.Exceptions;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.General
{
    public class CantidadMaximaImpresionesValidationRule : IValidationRule
    {
        protected readonly string _value;
        protected readonly IUnitOfWork _uow;
        protected readonly IFormatProvider _culture;

        public CantidadMaximaImpresionesValidationRule(string value, IUnitOfWork uow, IFormatProvider culture)
        {
            _uow = uow;
            _value = value;
            _culture = culture;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            var cantidad = int.Parse(_value, _culture);

            var paramValue = _uow.ParametroRepository.GetParameter(ParamManager.CANTIDAD_MAXIMA_IMPRESIONES);

            if (int.TryParse(paramValue, out int cantidadMaxima) && cantidad > cantidadMaxima)
                errors.Add(new ValidationError("General_Sec0_Error_CantMaximaImpresiones", [cantidadMaxima.ToString()]));

            return errors;
        }
    }
}
