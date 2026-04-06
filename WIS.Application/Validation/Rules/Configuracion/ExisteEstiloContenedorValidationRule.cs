using System;
using System.Collections.Generic;
using System.Text;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Configuracion
{
    public class ExisteEstiloContenedorValidationRule : IValidationRule
    {
        protected readonly string _estilo;
        protected readonly string _tipo;
        protected readonly IUnitOfWork _uow;

        public ExisteEstiloContenedorValidationRule(string valueEstiloContenedor, string tipoContenedor, IUnitOfWork uow)
        {
            this._estilo = valueEstiloContenedor;
            this._tipo = tipoContenedor;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (_uow.EstiloContenedorRepository.ExisteTipoContenedor(this._estilo, this._tipo))
                errors.Add(new ValidationError("General_Sec0_Error_EstiloContenedorExiste"));

            return errors;
        }
    }
}
