using System;
using System.Collections.Generic;
using System.Text;
using WIS.Application.Validation.Rules;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.GridComponent;
using WIS.GridComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.GridModules
{
    public class MantenimientoCotizacionListasGridValidationModule : GridValidationModule
    {
        protected readonly IFormatProvider _culture;

        public MantenimientoCotizacionListasGridValidationModule(IFormatProvider culture)
        {
            this._culture = culture;

            this.Schema = new GridValidationSchema
            {
                ["QT_IMPORTE"] = this.ValidateImporte,
                ["QT_IMPORTE_MINIMO"] = this.ValidateImporteMinimo,
            };
        }

        public virtual GridValidationGroup ValidateImporte(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            return new GridValidationGroup
            {
                Rules = new List<IValidationRule> {
                    new NonNullValidationRule(cell.Value),
                    new PositiveDecimalValidationRule(this._culture, cell.Value),
                }
            };
        }

        public virtual GridValidationGroup ValidateImporteMinimo(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            return new GridValidationGroup
            {
                Rules = new List<IValidationRule> {
                    new NonNullValidationRule(cell.Value),
                    new PositiveDecimalValidationRule(this._culture, cell.Value),
                }
            };
        }
    }
}
