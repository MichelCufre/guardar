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
    public class MantenimientoCodigoSACGridValidationModule : GridValidationModule
    {
        protected readonly IUnitOfWork _uow;
        public MantenimientoCodigoSACGridValidationModule(IUnitOfWork uow)
        {
            this._uow = uow;

            this.Schema = new GridValidationSchema
            {
                ["CD_NAM"] = this.ValidateIdCodigoSAC,
                ["DS_NAM"] = this.ValidateDescripcionCodigoSAC,
            };
        }

        public virtual GridValidationGroup ValidateIdCodigoSAC(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            var rules = new GridValidationGroup
            {
                Rules = new List<IValidationRule> {
                        new NonNullValidationRule(cell.Value),
                        new StringMaxLengthValidationRule(cell.Value, 20),
                }
            };

            if (row.IsNew || row.IsModified && row.GetCell("CD_NAM").Old != row.GetCell("CD_NAM").Value)
            {
                rules.Rules.Add(new ExisteCodigoSAC(_uow, cell.Value));
            }

            return rules;
        }
        public virtual GridValidationGroup ValidateDescripcionCodigoSAC(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            return new GridValidationGroup
            {
                Rules = new List<IValidationRule> {
                    new NonNullValidationRule(cell.Value),
                    new StringMaxLengthValidationRule(cell.Value, 60)
                }
            };
        }
    }
}
