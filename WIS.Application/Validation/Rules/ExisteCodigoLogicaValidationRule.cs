using System;
using System.Collections.Generic;
using System.Text;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules
{
    public class ExisteCodigoLogicaValidationRule : IValidationRule
    {
        protected readonly string _idLogica;
        protected readonly IUnitOfWork _uow;

        public ExisteCodigoLogicaValidationRule(IUnitOfWork uow, string valueCodigo)
        {
            this._idLogica = valueCodigo;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (!string.IsNullOrEmpty(this._idLogica))
            {
                if (short.TryParse(_idLogica, out short nuLogica))
                {
                    if (!_uow.EstrategiaRepository.AnyCodigoLogica(nuLogica))
                        errors.Add(new ValidationError("General_Sec0_Error_NoExisteCodigoLogica"));
                }
            }

            return errors;
        }
    }
}
