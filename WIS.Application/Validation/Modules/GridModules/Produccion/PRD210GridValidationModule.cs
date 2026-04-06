using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.Produccion;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.General;
using WIS.GridComponent;
using WIS.GridComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.GridModules.Produccion
{
    public class PRD210GridValidationModule : GridValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly IFormatProvider _culture;

        public PRD210GridValidationModule(IUnitOfWork uow,
            IFormatProvider culture)
        {
            _uow = uow;
            _culture = culture;

            Schema = new GridValidationSchema
            {
                ["QT_CONSUMIDO"] = ValidateCantidad
            };
        }

        public virtual GridValidationGroup ValidateCantidad(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            string cdProducto = row.GetCell("CD_PRODUTO").Value;
            string identificador = row.GetCell("NU_IDENTIFICADOR").Value;
            decimal faixa = decimal.Parse(row.GetCell("CD_FAIXA").Value, this._culture);
            int empresa = int.Parse(row.GetCell("CD_EMPRESA").Value);

            string nroIngreso = parameters.Where(d => d.Id == "NU_PRDC_INGRESO").FirstOrDefault().Value;

            Producto producto = _uow.ProductoRepository.GetProductoEmpresaAsignadaUsuario(empresa, cdProducto);

            if (producto == null)
                return null;

            if (producto.AceptaDecimales)
            {
                return new GridValidationGroup
                {
                    Rules = new List<IValidationRule> {
                        new NonNullValidationRule(cell.Value),
                        new DecimalCultureSeparatorValidationRule(this._culture, cell.Value),
                        new PositiveDecimalValidationRule(this._culture, cell.Value),
                        new DecimalLengthWithPresicionValidationRule(cell.Value, 12, 3, this._culture),
                        new ProduccionBlackBoxConsumoStockValidationRule(_uow, nroIngreso, cdProducto, empresa, identificador, faixa, cell.Value, this._culture)
                    }
                };
            }

            return new GridValidationGroup
            {
                Rules = new List<IValidationRule> {
                    new NonNullValidationRule(cell.Value),
                    new PositiveIntValidationRule(cell.Value),
                    new ProduccionBlackBoxConsumoStockValidationRule(_uow, nroIngreso, cdProducto, empresa, identificador, faixa, cell.Value, this._culture)
                }
            };
        }
    }
}
