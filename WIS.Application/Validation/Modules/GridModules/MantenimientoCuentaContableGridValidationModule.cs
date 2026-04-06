using System;
using System.Collections.Generic;
using System.Text;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.Facturacion;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.GridComponent;
using WIS.GridComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.GridModules
{
    public class MantenimientoCuentaContableGridValidationModule : GridValidationModule
    {
        protected readonly IUnitOfWork _uow;
        public MantenimientoCuentaContableGridValidationModule(IUnitOfWork uow)
        {
            this._uow = uow;

            this.Schema = new GridValidationSchema
            {
                ["NU_CUENTA_CONTABLE"] = this.ValidateIdCuentaContable,
                ["DS_CUENTA_CONTABLE"] = this.ValidateDescipcionCuentaContable,
            };
        }

        public virtual GridValidationGroup ValidateIdCuentaContable(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {

            var rules = new GridValidationGroup
            {
                Rules = new List<IValidationRule> {
                    new NonNullValidationRule(cell.Value),
                    new StringMaxLengthValidationRule(cell.Value, 10),
                }
            };

            if (row.IsNew || row.IsModified && row.GetCell("NU_CUENTA_CONTABLE").Old != row.GetCell("NU_CUENTA_CONTABLE").Value)
            {
                rules.Rules.Add(new ExisteCuentaContableRegistradoValidationRule(_uow, cell.Value));
            }

            return rules;
        }

        public virtual GridValidationGroup ValidateDescipcionCuentaContable(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            return new GridValidationGroup
            {
                Rules = new List<IValidationRule> {
                    new NonNullValidationRule(cell.Value),
                    new StringMaxLengthValidationRule(cell.Value, 100),
                }
            };
        }
    }
}
