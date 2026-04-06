using System;
using System.Collections.Generic;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.Configuracion;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.GridComponent;
using WIS.GridComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.GridModules
{
    public class AutomatismoInterfacesGridValidationModule : GridValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly IFormatProvider _formatProvider;

        public AutomatismoInterfacesGridValidationModule(IUnitOfWork uow, IFormatProvider formatProvider)
        {
            this._uow = uow;
            this._formatProvider = formatProvider;

            this.Schema = new GridValidationSchema
            {
                ["NU_INTEGRACION_SERVICIO"] = this.ValidateIntegracionServicio,
                ["VL_URL"] = this.ValidateURL,
            };
        }

        public virtual GridValidationGroup ValidateIntegracionServicio(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(cell.Value))
            {
                return null;
            }
            return new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(cell.Value),
                    new IntegracionServicioValidationRule(_uow, int.Parse(cell.Value))
                },
            };
        }

        public virtual GridValidationGroup ValidateURL(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            return new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                     new StringMaxLengthValidationRule(cell.Value, 400),
                },
            };
        }
    }
}
