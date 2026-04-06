using System;
using System.Collections.Generic;
using System.Text;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.Registro;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.GridComponent;
using WIS.GridComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.GridModules.Registro
{
    public class GruposValidationModule : GridValidationModule
    {
        protected readonly IUnitOfWork _uow;

        public GruposValidationModule(IUnitOfWork uow)
        {
            this._uow = uow;

            this.Schema = new GridValidationSchema
            {
                ["DS_GRUPO"] = this.ValidateDescripcion,
                ["CD_CLASSE"] = this.ValidateClase,
            };
        }

        public virtual GridValidationGroup ValidateDescripcion(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            return new GridValidationGroup
            {
                Rules = new List<IValidationRule> {
                    new NonNullValidationRule(cell.Value),
                    new StringMaxLengthValidationRule(cell.Value, 100),
                },
            };
        }
        public virtual GridValidationGroup ValidateClase(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            return new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(cell.Value),
                        new StringMaxLengthValidationRule(cell.Value, 2),
                        new ClaseNoExistenteValidationRule(_uow, cell.Value),
                },
                OnSuccess = this.ValidateClase_OnSuccess,
                OnFailure = this.ValidateClase_OnFailure
            };
        }

        public virtual void ValidateClase_OnSuccess(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            var clase = _uow.ClaseRepository.GetClaseById(cell.Value);
            row.GetCell("DS_CLASSE").Value = clase.Descripcion;
        }
        public virtual void ValidateClase_OnFailure(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            row.GetCell("DS_CLASSE").Value = string.Empty;
        }
    }
}
