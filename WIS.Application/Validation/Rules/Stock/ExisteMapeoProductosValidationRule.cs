using System.Collections.Generic;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Stock
{
    public class ExisteMapeoProductosValidationRule : IValidationRule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly int _empresaOrigen;
        protected readonly int _empresaDestino;
        protected readonly decimal _faixa;
        protected readonly string _producto;

        public ExisteMapeoProductosValidationRule(IUnitOfWork uow, int empresaOrigen, int empresaDestino, decimal faixa, string producto)
        {
            this._uow = uow;
            this._empresaOrigen = empresaOrigen;
            this._empresaDestino = empresaDestino;
            this._faixa = faixa;
            this._producto = producto;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (this._uow.TraspasoEmpresasRepository.AnyMapeoProducto(_empresaOrigen, _producto, _faixa, _empresaDestino))
                errors.Add(new ValidationError("STO810_Sec0_Error_MapeoYaExiste"));

            return errors;
        }
    }
}
