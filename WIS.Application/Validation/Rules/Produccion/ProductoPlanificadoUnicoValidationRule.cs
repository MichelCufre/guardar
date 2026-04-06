using System.Collections.Generic;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Produccion
{
    public class ProductoPlanificadoUnicoValidationRule : IValidationRule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly int _empresa;
        protected readonly string _producto;
        protected readonly string _idIngreso;
        protected readonly bool _planificacionPedido;

        public ProductoPlanificadoUnicoValidationRule(IUnitOfWork uow, int empresa, string idIngreso, string producto, bool planificacionPedido)
        {
            this._uow = uow;
            this._empresa = empresa;
            this._producto = producto;
            this._idIngreso = idIngreso;
            this._planificacionPedido = planificacionPedido;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            var producto = _uow.ProductoRepository.GetProducto(_empresa, _producto);

            if (producto != null && producto.IsIdentifiedByProducto())
            {
                if (this._uow.ProduccionRepository.AnyInsumoPlanificacion(this._idIngreso, this._empresa, this._producto, _planificacionPedido))
                    errors.Add(new ValidationError("PRD110DetallePedido_grid1_error_ProductoYaExisteInsumos"));
            }
            return errors;
        }
    }
}
