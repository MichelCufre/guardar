using System;
using System.Collections.Generic;
using System.Text;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Impresion
{
    public class ExisteServidorImpresionValidationRule : IValidationRule
    {
        protected readonly string _servidor;
        protected readonly IUnitOfWork _uow;

        public ExisteServidorImpresionValidationRule(IUnitOfWork uow, string valueServidor)
        {
            this._servidor = valueServidor;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (string.IsNullOrEmpty(this._servidor))
                errors.Add(new ValidationError("General_Sec0_Error_Error25"));

            if (!_uow.ImpresionRepository.ExisteServidorImpresion(int.Parse(this._servidor)))
                errors.Add(new ValidationError("General_Sec0_Error_ServidorImpresionNoExiste"));

            return errors;
        }
    }
}
