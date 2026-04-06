using System.Collections.Generic;
using WIS.Domain.StockEntities.Constants;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Stock
{
    internal class ExisteConfiguracionPedidoDestinoValidationRule : IValidationRule
    {
        protected string _opcion;
        protected readonly int _nuPreparacion;

        public ExisteConfiguracionPedidoDestinoValidationRule(string opcion)
        {
            this._opcion = opcion;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (this._opcion != TraspasoEmpresasDb.MISMO_NUMERO &&
                this._opcion != TraspasoEmpresasDb.MISMO_CRITERIO &&
                this._opcion != TraspasoEmpresasDb.ESPECIFICA_MANUALMENTE)
                errors.Add(new ValidationError("STO820_Sec0_Error_ConfiguracionPedidoDestinoNoExiste"));

            return errors;
        }
    }
}