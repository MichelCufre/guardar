using System;
using System.Collections.Generic;
using WIS.Application.Validation.Rules;
using WIS.Components.Common;
using WIS.GridComponent;
using WIS.GridComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.GridModules
{
    public class ConsultaAvancePedidosGridValidationModule : GridValidationModule
    {
        protected readonly IFormatProvider _formatProvider;

        public ConsultaAvancePedidosGridValidationModule(IFormatProvider formatProvider)
        {
            this.Schema = new GridValidationSchema
            {
                ["QT_PEDIDOS_REAB_PRED1"] = this.ValidateCantidad,
                ["QT_PEDIDOS_REAB_PRED2"] = this.ValidateCantidad,
                ["QT_PEDIDOS_REAB_PRED3"] = this.ValidateCantidad,
                ["QT_PEDIDOS_REAB_PRED4"] = this.ValidateCantidad,
                ["QT_PEDIDOS_REAB_PRED5"] = this.ValidateCantidad,
                ["QT_PEDIDOS_REAB_PRED6"] = this.ValidateCantidad,
                ["QT_PEDIDOS_REAB_PRED7"] = this.ValidateCantidad,
                ["QT_PEDIDOS_REAB_PRED8"] = this.ValidateCantidad,
                ["QT_PEDIDOS_REAB_PRED9"] = this.ValidateCantidad,
                ["QT_PEDIDOS_REAB_PRED10"] = this.ValidateCantidad,
            };

            this._formatProvider = formatProvider;
        }

        public virtual GridValidationGroup ValidateCantidad(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            return new GridValidationGroup
            {
                Rules = new List<IValidationRule> {
                    new DecimalLengthWithPresicionValidationRule(cell.Value, 15, 3, this._formatProvider),
                    new PositiveDecimalValidationRule(this._formatProvider, cell.Value)
                }
            };
        }
    }
}
