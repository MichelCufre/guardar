using System;
using System.Collections.Generic;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.General;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.GridComponent;
using WIS.GridComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.GridModules
{
    public class DetalleAgendaGridValidationModule : GridValidationModule
    {
        protected readonly IFormatProvider _culture;
        protected readonly IUnitOfWork _uow;

        public DetalleAgendaGridValidationModule(IUnitOfWork uow, IFormatProvider culture )
        {
            this._uow = uow;
            this._culture = culture;

            this.Schema = new GridValidationSchema
            {
                ["QT_ETIQUETA_IMPRIMIR"] = this.ValidateCantidadImprimir
            };
        }

        public virtual GridValidationGroup ValidateCantidadImprimir(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            return new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new PositiveIntValidationRule(cell.Value, false),
                    new CantidadMaximaImpresionesValidationRule(cell.Value, _uow, _culture),
                }
            };
        }
    }
}
