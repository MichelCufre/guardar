using System;
using System.Collections.Generic;
using System.Text;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Parametrizacion
{
    public class ExisteUnidadMedidaRegistradaValidationRule : IValidationRule
    {
        protected readonly string _unidadMedida;
        protected readonly IUnitOfWork _uow;

        public ExisteUnidadMedidaRegistradaValidationRule(string valueUnidadMedida, IUnitOfWork uow)
        {
            this._unidadMedida = valueUnidadMedida;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (_uow.UnidadMedidaRepository.ExisteUnidadMedida(this._unidadMedida))
                errors.Add(new ValidationError("General_Sec0_Error_UnidadMedidaExiste"));

            return errors;
        }
    }
}
