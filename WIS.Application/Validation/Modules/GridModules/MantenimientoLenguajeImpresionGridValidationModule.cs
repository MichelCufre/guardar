using System;
using System.Collections.Generic;
using System.Text;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.Configuracion;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.GridComponent;
using WIS.GridComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.GridModules
{
    public class MantenimientoLenguajeImpresionGridValidationModule : GridValidationModule
    {
        protected readonly IUnitOfWork _uow;
        public MantenimientoLenguajeImpresionGridValidationModule(IUnitOfWork uow)
        {
            this._uow = uow;

            this.Schema = new GridValidationSchema
            {
                ["CD_LENGUAJE_IMPRESION"] = this.ValidateIdLenguajeImpresion,
                ["DS_LENGUAJE_IMPRESION"] = this.ValidateDescipcionLenguajeImpresion,
            };
        }

        public virtual GridValidationGroup ValidateIdLenguajeImpresion(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {

            var rules = new GridValidationGroup
            {
                Rules = new List<IValidationRule> {
                    new NonNullValidationRule(cell.Value),
                    new StringMaxLengthValidationRule(cell.Value, 10),
                }
            };

            if (row.IsNew || row.IsModified && row.GetCell("CD_LENGUAJE_IMPRESION").Old != row.GetCell("CD_LENGUAJE_IMPRESION").Value)
            {
                rules.Rules.Add(new ExisteLenguajeImpresionRegistradoValidationRule(_uow, cell.Value));
            }

            return rules;
        }

        public virtual GridValidationGroup ValidateDescipcionLenguajeImpresion(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            return new GridValidationGroup
            {
                Rules = new List<IValidationRule> {
                    new StringMaxLengthValidationRule(cell.Value, 30),
                }
            };
        }
    }
}
