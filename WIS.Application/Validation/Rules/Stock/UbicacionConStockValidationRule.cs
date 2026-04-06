using System;
using System.Collections.Generic;
using System.Text;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Stock
{
    public class UbicacionConStockValidationRule : IValidationRule
    {
        protected readonly string _value;
        protected readonly IUnitOfWork _uow;

        public UbicacionConStockValidationRule(IUnitOfWork uow, string idUbicacion)
        {
            this._value = idUbicacion.ToUpper();
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (_uow.StockRepository.AnyStockUbicacion(_value))
                errors.Add(new ValidationError("General_Sec0_Error_UbicacionConStock"));

            return errors;
        }
    }
}