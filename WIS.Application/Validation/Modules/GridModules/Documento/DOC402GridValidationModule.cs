using System;
using System.Collections.Generic;
using System.Globalization;
using WIS.Application.Validation.Rules;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.GridComponent;
using WIS.GridComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.GridModules.Documento
{
    public class DOC402GridValidationModule : GridValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly IFormatProvider _culture;

        public DOC402GridValidationModule(
            IUnitOfWork uow,
            IFormatProvider culture)
        {
            this._uow = uow;
            this._culture = culture;

            Schema = new GridValidationSchema
            {
                ["QT_MOVIMIENTO"] = this.ValidateMovimiento,
                ["NU_DOC"] = this.ValidateDOC
            };
        }

        public virtual GridValidationGroup ValidateMovimiento(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            if (cell.Value == null)
                return null;
            
            decimal move = 0;
            var errors = new List<IValidationError>();
            
            if (!decimal.TryParse(row.GetCell("QT_MOVIMIENTO").Value, NumberStyles.Any, this._culture, out move))
            {
                errors.Add(new ValidationError("General_Sec0_Error_DebeSerUnNumero"));
            }
            
            decimal saldo = decimal.Parse(row.GetCell("QT_SALDO").Value, this._culture);
            
            return new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = {
                     new NonNullValidationRule(cell.Value),
                     new NumeroDecimalMenorOIgualQueValidationRule(move,saldo),
                     new PositiveDecimalValidationRule(this._culture, cell.Value)
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
