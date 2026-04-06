using System;
using System.Collections.Generic;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Produccion;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Produccion
{
    public class ProduccionBlackBoxConsumoStockValidationRule : IValidationRule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly string _ingreso;
        protected readonly string _producto;
        protected readonly int _empresa;
        protected readonly string _identificador;
        protected readonly decimal _faixa;
        protected readonly string _value;
        protected readonly IFormatProvider _culture;

        public ProduccionBlackBoxConsumoStockValidationRule(IUnitOfWork uow, string ingreso, string producto, int empresa, string identificador, decimal faixa, string value, IFormatProvider culture)
        {
            this._uow = uow;
            this._ingreso = ingreso;
            this._producto = producto;
            this._empresa = empresa;
            this._identificador = identificador;
            this._faixa = faixa;
            this._value = value;
            this._culture = culture;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            decimal value = decimal.Parse(this._value, this._culture);

            var dbQuery = new StockConsumirBlackBoxProduccionQuery(this._ingreso);

            this._uow.HandleQuery(dbQuery);

            decimal stockDisponible = dbQuery.GetMaxStockDisponibleProducto(this._producto, this._empresa, this._identificador, this._faixa);

            if (value > stockDisponible)
                errors.Add(new ValidationError("General_Sec0_Error_NotEnoughStockDisponible"));

            return errors;
        }
    }

}
