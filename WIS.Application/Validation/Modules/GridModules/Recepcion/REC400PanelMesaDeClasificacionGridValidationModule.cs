using System;
using System.Collections.Generic;
using System.Text;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.Facturacion;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.GridComponent;
using WIS.GridComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.GridModules.Recepcion
{

    public class REC400PanelMesaDeClasificacionGridValidationModule : GridValidationModule
    {
        protected readonly IUnitOfWork _uow;
        public REC400PanelMesaDeClasificacionGridValidationModule(IUnitOfWork uow)
        {
            this._uow = uow;

            this.Schema = new GridValidationSchema
            {
                ["DS_ESTACION"] = this.ValidateDescripcion,
            };
        }

        public virtual GridValidationGroup ValidateDescripcion(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            return new GridValidationGroup
            {
                Rules = new List<IValidationRule> {
                    new NonNullValidationRule(cell.Value),
                    new StringMaxLengthValidationRule(cell.Value, 50),
                }
            };
        }
    }
}
