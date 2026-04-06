using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Rules
{
    public class EstadoInicialAtributoValidationRule : IValidationRule
    {
        protected readonly string _value;
        protected readonly IUnitOfWork _uow;

        public EstadoInicialAtributoValidationRule(IUnitOfWork uow, string value)
        {
            this._uow = uow;
            this._value = value;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();
            if (!string.IsNullOrEmpty(_value))
            {
                var estadosPermitidos = new List<string>() { EstadoLpnAtributo.Pendiente, EstadoLpnAtributo.Ingresado };
                if (!estadosPermitidos.Contains(_value))
                    errors.Add(new ValidationError("PAR401_Sec0_Error_EstadoInicialNoPermitido", new List<string>() { _value }));
            }
            return errors;
        }
    }
}
