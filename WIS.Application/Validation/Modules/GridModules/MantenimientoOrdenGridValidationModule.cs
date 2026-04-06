using DocumentFormat.OpenXml;
using System;
using System.Collections.Generic;
using System.Text;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.General;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Exceptions;
using WIS.GridComponent;
using WIS.GridComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.GridModules
{
    public class MantenimientoOrdenGridValidationModule : GridValidationModule
    {
        protected readonly IUnitOfWork _uow;
        public MantenimientoOrdenGridValidationModule(IUnitOfWork uow)
        {
            this._uow = uow;

            this.Schema = new GridValidationSchema
            {
                ["DS_ORT_ORDEN"] = this.ValidateDescipcion,
                ["TP_ORDEN"] = this.ValidateTipo,
                ["DT_INICIO"] = this.ValidateFechaInicio,
                ["DS_REFERENCIA"] = this.ValidateDsReferencia,
            };
        }
        public virtual GridValidationGroup ValidateDescipcion(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            return new GridValidationGroup
            {
                Rules = new List<IValidationRule> {
                    new StringMaxLengthValidationRule(cell.Value, 60)
                }
            };
        }
        public virtual GridValidationGroup ValidateTipo(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            return new GridValidationGroup
            {
                Rules = new List<IValidationRule> {
                    new NonNullValidationRule(cell.Value),
                    new StringMaxLengthValidationRule(cell.Value, 4)
                }
            };
        }
        public virtual GridValidationGroup ValidateFechaInicio(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            return new GridValidationGroup
            {
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(cell.Value),
                    new DateTimeValidationRule(cell.Value)
                }
            };
        }
        public virtual GridValidationGroup ValidateDsReferencia(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            return new GridValidationGroup
            {
                Rules = new List<IValidationRule>
                {
                    new StringMaxLengthValidationRule(cell.Value, 100)
                }
            };
        }
       
    }
}
