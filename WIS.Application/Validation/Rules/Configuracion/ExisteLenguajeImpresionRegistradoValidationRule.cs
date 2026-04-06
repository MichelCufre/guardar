using System;
using System.Collections.Generic;
using System.Text;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Configuracion
{
    public class ExisteLenguajeImpresionRegistradoValidationRule : IValidationRule
    {
        protected readonly string _lenguaje;
        protected readonly IUnitOfWork _uow;

        public ExisteLenguajeImpresionRegistradoValidationRule(IUnitOfWork uow, string valueLenguaje)
        {
            this._lenguaje = valueLenguaje;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (_uow.LenguajeImpresionRepository.ExisteLenguajeImpresion(this._lenguaje))
                errors.Add(new ValidationError("General_Sec0_Error_LenguajeImpresionExiste"));

            return errors;
        }
    }
}
