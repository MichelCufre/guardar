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
    public class MantenimientoTipoCodigoBarraGridValidationModule : GridValidationModule
    {
        protected readonly IUnitOfWork _uow;
        public MantenimientoTipoCodigoBarraGridValidationModule(IUnitOfWork uow)
        {
            this._uow = uow;

            this.Schema = new GridValidationSchema
            {
                ["TP_CODIGO_BARRAS"] = this.ValidateTipoCodigo,
                ["DS_CODIGO_BARRAS"] = this.ValidateDescripcion,
            };
        }

        public virtual GridValidationGroup ValidateTipoCodigo(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            var rules = new GridValidationGroup
            {
                Rules = new List<IValidationRule> {
                    new NonNullValidationRule(cell.Value),
                    new PositiveIntValidationRule(cell.Value),
                }
            };

            if (!string.IsNullOrEmpty(cell.Value) && (row.IsNew || row.IsModified && row.GetCell("TP_CODIGO_BARRAS").Old != row.GetCell("TP_CODIGO_BARRAS").Value))
            {
                rules.Rules.Add(new ExisteTipoCodigoBarraRegistradaValidationRule(int.Parse(cell.Value), _uow));
            }

            return rules;
        }
        public virtual GridValidationGroup ValidateDescripcion(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            return new GridValidationGroup
            {
                Rules = new List<IValidationRule> {
                    new NonNullValidationRule(cell.Value),
                    new StringMaxLengthValidationRule(cell.Value, 40),
                }
            };
        }
    }
}
