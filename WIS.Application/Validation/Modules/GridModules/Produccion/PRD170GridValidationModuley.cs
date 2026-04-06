using System;
using System.Collections.Generic;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.Produccion;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.Produccion;
using WIS.GridComponent;
using WIS.GridComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.GridModules.Produccion
{
    public class PRD170GridValidationModule : GridValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly IFormatProvider _culture;

        public PRD170GridValidationModule(IUnitOfWork uow,
            IFormatProvider culture)
        {
            _uow = uow;
            _culture = culture;

            Schema = new GridValidationSchema
            {
                ["CD_PRDC_LINEA"] = this.ValidateLinea,
            };
        }

        public virtual GridValidationGroup ValidateLinea(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            var rules = new List<IValidationRule>
            {
                new StringMaxLengthValidationRule(cell.Value, 10),
                new LineaExistsValidationRule(this._uow, cell.Value)
            };

            return new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = rules,
                OnSuccess = this.ValidateLineaOnSuccess,
                OnFailure = this.ValidateLineaOnFailure
            };
        }

        public virtual void ValidateLineaOnSuccess(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            var cellLinea = row.GetCell("CD_PRDC_LINEA");
            var cellDsLinea = row.GetCell("DS_PRDC_LINEA");
            ILinea linea = this._uow.LineaRepository.GetLinea(cellLinea.Value);

            cellDsLinea.Value = linea.Descripcion;
        }

        public virtual void ValidateLineaOnFailure(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            row.GetCell("DS_PRDC_LINEA").Value = string.Empty;
        }
    }
}
