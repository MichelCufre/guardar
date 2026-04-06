using System;
using System.Collections.Generic;
using System.Text;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Impresion
{
    public class ExisteLenguajeImpresionValidationRule : IValidationRule
    {
        protected readonly string _lenguaje;
        protected readonly IUnitOfWork _uow;

        public ExisteLenguajeImpresionValidationRule(IUnitOfWork uow, string valueLenguaje)
        {
            this._lenguaje = valueLenguaje;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if(string.IsNullOrEmpty(this._lenguaje))
                errors.Add(new ValidationError("General_Sec0_Error_Error25"));

            if (!_uow.ImpresionRepository.ExisteLenguajeImpresion(this._lenguaje))
                errors.Add(new ValidationError("General_Sec0_Error_LenguajeImpresionNoExiste"));
            
            return errors;
        }
    }
}
