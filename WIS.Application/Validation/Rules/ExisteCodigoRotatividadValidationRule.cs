using System.Collections.Generic;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules
{
    public class ExisteCodigoRotatividadValidationRule : IValidationRule
    {
        protected readonly string _idCodigo;
        protected readonly IUnitOfWork _uow;

        public ExisteCodigoRotatividadValidationRule(IUnitOfWork uow, string valueCodigo)
        {
            this._idCodigo = valueCodigo;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();
            if (!string.IsNullOrEmpty(this._idCodigo))
            {
                if (!short.TryParse(this._idCodigo, out short parsedValue) || parsedValue < 0)
                    errors.Add(new ValidationError("General_Sec0_Error_Error27"));
                else
                {
                    if (!_uow.ProductoRotatividadRepository.AnyProductoRotatividad(parsedValue))
                        errors.Add(new ValidationError("General_Sec0_Error_NoExisteCodigoRotatividad"));
                }
            }

            return errors;
        }
    }
}
