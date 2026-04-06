using System.Collections.Generic;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Stock
{
    public class ProductosCoincidenValidationRule : IValidationRule
    {
        protected readonly string _cdProductoOrigen;
        protected readonly string _cdProductoDestino;

        public ProductosCoincidenValidationRule( string cdProductoOrigen, string cdProductoDestino)
        {
            _cdProductoOrigen = cdProductoOrigen;
            _cdProductoDestino = cdProductoDestino;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (_cdProductoOrigen == _cdProductoDestino)
                errors.Add(new ValidationError("STO810_frm1_Error_ProductosCoinciden"));

            return errors;
        }
    }
}
