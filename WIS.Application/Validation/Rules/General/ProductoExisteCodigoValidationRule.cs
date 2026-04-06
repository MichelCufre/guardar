using System.Collections.Generic;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules
{
    public class ProductoExisteCodigoValidationRule : IValidationRule
    {
        protected readonly int _empresa;
        protected readonly string _cdBarras;
        protected readonly IUnitOfWork _uow;

        public ProductoExisteCodigoValidationRule(IUnitOfWork uow, int empresa, string cdBarras)
        {
            this._empresa = empresa;
            this._cdBarras = cdBarras?.ToUpper();
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (!this._uow.ProductoCodigoBarraRepository.ExisteCodigoBarra(this._cdBarras, this._empresa))
                errors.Add(new ValidationError("General_Sec0_Error_Error05"));

            return errors;
        }
    }
}
