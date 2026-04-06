using System.Collections.Generic;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Produccion
{
    public class ProduccionProductoProducidoNotExistsValidationRule : IValidationRule
    {
        protected readonly string _nroIngreso;
        protected readonly int _empresa;
        protected readonly string _producto;
        protected readonly string _identificador;
        protected readonly decimal _faixa;
        protected readonly IUnitOfWork _uow;

        public ProduccionProductoProducidoNotExistsValidationRule(IUnitOfWork uow, string nroIngreso, int empresa, string producto, string identificador, decimal faixa)
        {
            this._nroIngreso = nroIngreso;
            this._empresa = empresa;
            this._producto = producto;
            this._identificador = identificador;
            this._faixa = faixa;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (this._uow.ProduccionRepository.AnyProductoProducido(this._nroIngreso, this._empresa, this._producto, this._identificador, this._faixa))
                errors.Add(new ValidationError("General_Sec0_Error_Error16"));

            return errors;
        }
    }
}
