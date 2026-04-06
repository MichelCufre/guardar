using System;
using System.Collections.Generic;
using System.Text;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Parametrizacion
{
    public class ExisteTipoCodigoBarraRegistradaValidationRule : IValidationRule
    {
        protected readonly int _tipoCodigoBarra;
        protected readonly IUnitOfWork _uow;

        public ExisteTipoCodigoBarraRegistradaValidationRule(int valueTipoCodigoBarra, IUnitOfWork uow)
        {
            this._tipoCodigoBarra = valueTipoCodigoBarra;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (_uow.ProductoCodigoBarraRepository.ExisteTipoCodigoBarras(this._tipoCodigoBarra))
                errors.Add(new ValidationError("General_Sec0_Error_TipoCodBarraExiste"));

            return errors;
        }
    }
}
