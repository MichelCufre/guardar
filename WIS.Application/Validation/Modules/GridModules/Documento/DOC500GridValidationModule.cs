using System;
using System.Collections.Generic;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.Documento;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.GridComponent;
using WIS.GridComponent.Validation;

namespace WIS.Application.Validation.Modules.GridModules.Documento
{
    public class DOC500GridValidationModule : GridValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly IFormatProvider _culture;

        public DOC500GridValidationModule(IUnitOfWork uow,
            IFormatProvider culture)
        {
            this._uow = uow;
            this._culture = culture;

            Schema = new GridValidationSchema
            {
                ["TP_DOC"] = this.ValidateTPDOC,
                ["NU_DOC"] = this.ValidateDOC,
                ["NU_DOC_CONF"] = this.ValidateDOCCONF
            };
        }

        public virtual GridValidationGroup ValidateTPDOC(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            return new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = {
                    new NonNullValidationRule(cell.Value)
                }
            };
        }

        public virtual GridValidationGroup ValidateDOCCONF(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            string valor = row.GetCell("NU_DOC").Value;
            return new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = {
                    new NonNullValidationRule(cell.Value),
                    new ValidationRuleConfDoc(cell.Value,valor)
                }
            };
        }

        public virtual GridValidationGroup ValidateDOC(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            return new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = {
                    new NonNullValidationRule(cell.Value)
                }
            };
        }
    }
}
