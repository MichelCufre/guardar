using System;
using System.Collections.Generic;
using System.Text;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Configuracion
{
    public class NoExisteEstiloEtiquetaValidationRule : IValidationRule
    {
        protected readonly string _estilo;
        protected readonly IUnitOfWork _uow;

        public NoExisteEstiloEtiquetaValidationRule(string valueEstilo, IUnitOfWork uow)
        {
            this._estilo = valueEstilo;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (!_uow.EstiloEtiquetaRepository.ExisteEtiquetaEstilo(this._estilo))
                errors.Add(new ValidationError("General_Sec0_Error_EstiloEtiquetaNoExiste"));

            return errors;
        }
    }
}
