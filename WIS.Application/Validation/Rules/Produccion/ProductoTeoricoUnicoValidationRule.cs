using System;
using System.Collections.Generic;
using System.Text;
using WIS.Domain.DataModel;
using WIS.Domain.General;
using WIS.Domain.Produccion.Constants;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Produccion
{
    public class ProductoTeoricoUnicoValidationRule : IValidationRule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly int _empresa;
        protected readonly string _producto;
        protected readonly string _idIngreso;
        protected readonly string _tipoRegistroTeorico;

        public ProductoTeoricoUnicoValidationRule(IUnitOfWork uow, int empresa, string idIngreso, string producto, string tipoRegistroTeorico)
        {
            this._uow = uow;
            this._empresa = empresa;
            this._producto = producto;
            this._idIngreso = idIngreso;
            this._tipoRegistroTeorico = tipoRegistroTeorico;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            var producto = _uow.ProductoRepository.GetProducto(_empresa, _producto);

            if (producto != null && producto.IsIdentifiedByProducto())
            {
                if (this._uow.ProduccionRepository.AnyInsumoTeorico(this._idIngreso, this._empresa, this._producto, this._tipoRegistroTeorico))
                    errors.Add(new ValidationError("PRD110DetallePedido_grid1_error_ProductoYaExisteInsumos"));
            }
            return errors;
        }
    }
}
