using System;
using System.Collections.Generic;
using System.Globalization;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Stock
{
    public class SaldoStockSuficienteEntradaBBValidationRule : IValidationRule
    {
        private readonly string _cantidadMovimiento, _cantidadRechazoAveria, _cantidadRechazoSano, _cantidadDisponible;
        protected readonly IUnitOfWork _uow;
        protected readonly IFormatProvider _culture;

        public SaldoStockSuficienteEntradaBBValidationRule(IUnitOfWork uow, string cantidadMovimiento, string cantidadRechazoAveria, string cantidadRechazoSano, string cantidadDisponible, IFormatProvider culture)
        {
            this._cantidadDisponible = cantidadDisponible;
            this._cantidadMovimiento = cantidadMovimiento;
            this._cantidadRechazoAveria = cantidadRechazoAveria;
            this._cantidadRechazoSano = cantidadRechazoSano;
            this._uow = uow;
            this._culture = culture;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            decimal cantidadDisponibleDec, cantidadMovimientoDec, cantidadRechazoAveriaDec, cantidadRechazoSanoDec = 0;

            decimal.TryParse(this._cantidadDisponible, NumberStyles.Number, _culture, out cantidadDisponibleDec);
            decimal.TryParse(this._cantidadMovimiento, NumberStyles.Number, _culture, out cantidadMovimientoDec);
            decimal.TryParse(this._cantidadRechazoAveria, NumberStyles.Number, _culture, out cantidadRechazoAveriaDec);
            decimal.TryParse(this._cantidadRechazoSano, NumberStyles.Number, _culture, out cantidadRechazoSanoDec);

            if (cantidadDisponibleDec < (cantidadMovimientoDec + cantidadRechazoSanoDec + cantidadRechazoAveriaDec))
                errors.Add(new ValidationError("General_Sec0_Error_Error71"));

            return errors;
        }
    }

}
