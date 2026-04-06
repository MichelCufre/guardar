using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Recepcion
{
    public class ProductoAgendaExistsValidationRule : IValidationRule
    {
        protected readonly string _valueAgenda;
        protected readonly int _valueEmpresa;
        protected readonly string _valueProducto;
        protected readonly string _faixa;
        protected readonly string _identificador;
        protected readonly IUnitOfWork _uow;
        protected readonly IFormatProvider _culture;

        public ProductoAgendaExistsValidationRule(IUnitOfWork uow, string valueAgenda, int valueEmpresa, string valueProducto, string faixa, string identificador, IFormatProvider culture)
        {
            this._valueAgenda = valueAgenda;
            this._valueEmpresa = valueEmpresa;
            this._valueProducto = valueProducto;
            this._faixa = faixa;
            this._identificador = identificador;
            this._uow = uow;
            this._culture = culture;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (this._uow.AgendaRepository.AnyAgendaDetalle(int.Parse(_valueAgenda), _valueEmpresa , _valueProducto, decimal.Parse(_faixa, _culture), _identificador))
                errors.Add(new ValidationError("General_Sec0_Error_DetalleAgendaExistente"));

            return errors;
        }
    }
}
