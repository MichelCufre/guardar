using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.Facturacion;
using WIS.Components.Common;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.Facturacion;
using WIS.Domain.General;
using WIS.GridComponent;
using WIS.GridComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.GridModules
{
    public class MantenimientoIngresoResultadosManualesGridValidationModule : GridValidationModule
    {
        protected readonly IFormatProvider _culture;
        public MantenimientoIngresoResultadosManualesGridValidationModule(IFormatProvider culture)
        {
            this._culture = culture;

            this.Schema = new GridValidationSchema
            {
                ["QT_RESULTADO"] = this.ValidateResultado,
                ["DS_ADICIONAL"] = this.ValidateDescripcionAdicional,
            };
        }
        public virtual GridValidationGroup ValidateResultado(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            return new GridValidationGroup
            {
                Rules = new List<IValidationRule> {
                    new NonNullValidationRule(cell.Value),
                    new PositiveDecimalValidationRule(this._culture, cell.Value)
                }
            };
        }
        public virtual GridValidationGroup ValidateDescripcionAdicional(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            return new GridValidationGroup
            {
                Rules = new List<IValidationRule> {
                    new StringMaxLengthValidationRule(cell.Value, 1000),
                }
            };
        }
    }
}
