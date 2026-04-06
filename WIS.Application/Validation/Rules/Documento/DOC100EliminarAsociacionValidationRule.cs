using System;
using System.Collections.Generic;
using System.Text;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Documento
{
    public class DOC100EliminarAsociacionValidationRule : IValidationRule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly string _preparacion;

        public DOC100EliminarAsociacionValidationRule(IUnitOfWork uow, string preparacion)
        {
            _uow = uow;
            _preparacion = preparacion;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            //int prep = int.Parse(_preparacion);

            //if (_uow.DocumentoRepository.AnyContenedorEnsamblado(prep))
            //    errors.Add(new ValidationError("DOC100_Sec0_Error_PrepConContendoresEnsamblados"));

            return errors;
        }
    }
}
