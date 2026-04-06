using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.Registro;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General;
using WIS.Domain.Liberacion;
using WIS.GridComponent;
using WIS.GridComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.GridModules
{
    public class MantenimientoProblemasDeRecepcionValidationModule : GridValidationModule
    {

        protected readonly IUnitOfWork _uow;

        public MantenimientoProblemasDeRecepcionValidationModule(IUnitOfWork uow)
        {
            this._uow = uow;

            this.Schema = new GridValidationSchema
            {

                ["FL_ACEPTADO"] = this.ValidateAceptado,

            };
        }

        public virtual GridValidationGroup ValidateAceptado(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {

            List<IValidationRule> reglas;

            reglas = new List<IValidationRule> {
                    new NonNullValidationRule(cell.Value),
                    new StringMaxLengthValidationRule(cell.Value, 1),

                };

            return new GridValidationGroup
            {
                Rules = reglas,
                OnSuccess = this.ValidateAceptado_OnSucess,
                OnFailure = this.ValidateAceptado_OnFailure
            };
        }

        public virtual void ValidateAceptado_OnSucess(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            row.CssClass = row.CssClass + "aceptado";
        }

        public virtual void ValidateAceptado_OnFailure(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
        }

    }
}
