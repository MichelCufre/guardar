using System;
using System.Collections.Generic;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.Registro;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.GridComponent;
using WIS.GridComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.GridModules
{
    public class MantenimientoControlDeCalidadClaseGridValidationModule : GridValidationModule
    {
        protected readonly IUnitOfWork _uow;
        public MantenimientoControlDeCalidadClaseGridValidationModule(IUnitOfWork uow)
        {
            this._uow = uow;

            this.Schema = new GridValidationSchema
            {
                ["CD_CONTROL"] = this.ValidateCodigo,
                ["DS_CONTROL"] = this.ValidateDescripcion,
                ["SG_CONTROL"] = this.ValidateSigla,
                ["ID_BLOQUEIO"] = this.ValidateBloqueo,
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
                    new PositiveNumberMaxLengthValidationRule(cell.Value,10),
                    new IdControlDeCalidadClaseExistenteValidationRule(_uow, cell.Value)
                }
            };
        }

        public virtual GridValidationGroup ValidateDescripcion(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            return new GridValidationGroup
            {
                Rules = new List<IValidationRule> {
                    new NonNullValidationRule(cell.Value),
                    new StringMaxLengthValidationRule(cell.Value, 40)
                },
            };
        }

        public virtual GridValidationGroup ValidateSigla(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            return new GridValidationGroup
            {
                Rules = new List<IValidationRule>{
                    new NonNullValidationRule(cell.Value),
                    new StringMaxLengthValidationRule(cell.Value, 20)
                }
            };
        }

        public virtual GridValidationGroup ValidateBloqueo(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            if ((string.IsNullOrEmpty(cell.Value)))
                return null;

            return new GridValidationGroup
            {
                Rules = new List<IValidationRule> {
                   new BooleanStringGridValidationRule(cell.Value)
                }
            };

        }
    }
}
