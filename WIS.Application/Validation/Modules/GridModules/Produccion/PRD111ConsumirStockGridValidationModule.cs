using System;
using System.Collections.Generic;
using WIS.Application.Validation.Rules;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.GridComponent.Validation;
using WIS.GridComponent;
using WIS.Validation;
using WIS.Application.Validation.Rules.Registro;
using WIS.Application.Validation.Rules.General;

namespace WIS.Application.Validation.Modules.GridModules.Produccion
{
    public class PRD111ConsumirStockGridValidationModule : GridValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly IFormatProvider _formatProvider;

        public PRD111ConsumirStockGridValidationModule(IUnitOfWork uow, IFormatProvider formatProvider)
        {
            this._uow = uow;
            _formatProvider = formatProvider;

            this.Schema = new GridValidationSchema
            {
                ["QT_AJUSTAR"] = this.ValidateCantidadAjustar,
            };
        }

        public virtual GridValidationGroup ValidateCantidadAjustar(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(row.GetCell("CD_EMPRESA").Value) || string.IsNullOrEmpty(row.GetCell("CD_PRODUTO").Value))
                return null;

            var codigoProducto = row.GetCell("CD_PRODUTO").Value;
            var empresa = int.Parse(row.GetCell("CD_EMPRESA").Value);

            var producto = _uow.ProductoRepository.GetProducto(empresa, codigoProducto);

            var rules = new List<IValidationRule> {
                new NonNullValidationRule(cell.Value),
                new PositiveDecimalValidationRule(this._formatProvider, cell.Value, false),
                new DecimalLengthWithPresicionValidationRule(cell.Value, 12, 3, this._formatProvider, producto.AceptaDecimales),
                new ManejoIdentificadorSerieCantidadValidationRule(cell.Value, producto.ManejoIdentificador, _formatProvider)
            };

            if (!string.IsNullOrEmpty(cell.Value))
                rules.Add(new NumeroDecimalMenorOIgualQueValidationRule(decimal.Parse(cell.Value, _formatProvider), decimal.Parse(row.GetCell("QT_DISPONIBLE").Value, _formatProvider)));

            return new GridValidationGroup
            {
                Rules = rules,
            };
        }
    }
}
