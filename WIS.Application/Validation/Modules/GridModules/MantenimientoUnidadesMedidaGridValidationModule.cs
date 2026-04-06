using System;
using System.Collections.Generic;
using System.Text;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.Parametrizacion;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.GridComponent;
using WIS.GridComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.GridModules
{
    public class MantenimientoUnidadesMedidaGridValidationModule : GridValidationModule
    {
        protected readonly IUnitOfWork _uow;
        public MantenimientoUnidadesMedidaGridValidationModule(IUnitOfWork uow)
        {
            this._uow = uow;

            this.Schema = new GridValidationSchema
            {
                ["CD_UNIDADE_MEDIDA"] = this.ValidateCodigo,
                ["DS_UNIDADE_MEDIDA"] = this.ValidateDescripcion,
                ["FG_ACEITA_DECIMAL"] = this.ValidateFlagDecimal,
                ["CD_UNIDAD_MEDIDA_EXTERNA"] = this.ValidateUnidadExterna,
            };
        }

        public virtual GridValidationGroup ValidateCodigo(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            var rules = new GridValidationGroup
            {
                Rules = new List<IValidationRule> {
                    new NonNullValidationRule(cell.Value),
                    new StringMaxLengthValidationRule(cell.Value, 10),
                }
            };

            if (row.IsNew || row.IsModified && row.GetCell("CD_UNIDADE_MEDIDA").Old != row.GetCell("CD_UNIDADE_MEDIDA").Value)
            {
                rules.Rules.Add(new ExisteUnidadMedidaRegistradaValidationRule(cell.Value, _uow));
            }

            return rules;
        }
        public virtual GridValidationGroup ValidateDescripcion(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            return new GridValidationGroup
            {
                Rules = new List<IValidationRule> {
                    new NonNullValidationRule(cell.Value),
                    new StringMaxLengthValidationRule(cell.Value, 30),
                }
            };
        }
        public virtual GridValidationGroup ValidateFlagDecimal(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            return new GridValidationGroup
            {
                Rules = new List<IValidationRule> {
                    new NonNullValidationRule(cell.Value),
                    new StringMaxLengthValidationRule(cell.Value, 1),
                    new StringToBooleanValidationRule(cell.Value)
                }
            };
        }
        public virtual GridValidationGroup ValidateUnidadExterna(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            return new GridValidationGroup
            {
                Rules = new List<IValidationRule> {
                    new StringMaxLengthValidationRule(cell.Value, 10)
                }
            };
        }
    }
}
