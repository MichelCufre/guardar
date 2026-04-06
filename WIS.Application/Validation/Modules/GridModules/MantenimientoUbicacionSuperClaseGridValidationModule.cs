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

namespace WIS.Application.Validation.Modules.GridModules
{
    public class MantenimientoUbicacionSuperClaseGridValidationModule : GridValidationModule
    {
        protected readonly IUnitOfWork _uow;
        public MantenimientoUbicacionSuperClaseGridValidationModule(IUnitOfWork uow)
        {
            this._uow = uow;

            this.Schema = new GridValidationSchema
            {
                ["CD_SUB_CLASSE"] = this.ValidateCodigo,
                ["DS_SUB_CLASSE"] = this.ValidateDescripcion
            };
        }

        public virtual GridValidationGroup ValidateCodigo(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            if (!row.IsNew)
                return null;

            return new GridValidationGroup
            {
                Rules = new List<IValidationRule> {
                    new NonNullValidationRule(cell.Value),
                    new StringMaxLengthValidationRule(cell.Value, 2),
                    new SuperClaseExistenteValidationRule(_uow,cell.Value)
                }
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

    }
}
