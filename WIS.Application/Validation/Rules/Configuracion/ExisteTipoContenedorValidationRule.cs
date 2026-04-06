using System;
using System.Collections.Generic;
using System.Text;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Configuracion
{
    public class ExisteTipoContenedorValidationRule : IValidationRule
    {
        protected readonly string _tipo;
        protected readonly IUnitOfWork _uow;

        public ExisteTipoContenedorValidationRule(string valueTipo, IUnitOfWork uow)
        {
            this._tipo = valueTipo;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (!_uow.ContenedorRepository.ExisteTipoContenedor(this._tipo))
                errors.Add(new ValidationError("General_Sec0_Error_TipoContenedorNoExiste"));

            return errors;
        }
    }
}
