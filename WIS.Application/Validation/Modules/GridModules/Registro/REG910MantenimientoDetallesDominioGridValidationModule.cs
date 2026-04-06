using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

using WIS.Application.Validation.Rules;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.GridComponent;
using WIS.GridComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.GridModules.Registro
{
    public class REG910MantenimientoDetallesDominioGridValidationModule : GridValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly IFormatProvider _culture;
        public REG910MantenimientoDetallesDominioGridValidationModule(IUnitOfWork uow, IFormatProvider culture)
        {
            this._uow = uow;
            this._culture = culture;

            this.Schema = new GridValidationSchema
            {
                ["NU_DOMINIO"] = this.ValidateNumeroDominio,
                ["DS_DOMINIO_VALOR"] = this.ValidateDescripcionDominio,
                ["CD_DOMINIO_VALOR"] = this.ValidateCodigoDominioValor,
            };
        }
        public virtual GridValidationGroup ValidateNumeroDominio(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            NoExisteDominioValidationRule validacionNumeroExistente = new NoExisteDominioValidationRule(cell.Value, this._uow);

            var rules = new List<IValidationRule> {
                    new NonNullValidationRule(cell.Value),
                    new StringMaxLengthValidationRule(cell.Value, 50),
                };

            if (row.IsNew)
                rules.Add(validacionNumeroExistente);
            else
                rules.Remove(validacionNumeroExistente);

            return new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = rules,
            };
        }

        public virtual GridValidationGroup ValidateDescripcionDominio(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {

            var rules = new List<IValidationRule> {
                    new NonNullValidationRule(cell.Value),
                    new StringMaxLengthValidationRule(cell.Value, 100),
                };

            return new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = rules,
            };
        }

        public virtual GridValidationGroup ValidateCodigoDominioValor(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {

            var rules = new List<IValidationRule> {
                    new NonNullValidationRule(cell.Value),
                    new StringMaxLengthValidationRule(cell.Value, 20),
                };

            return new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = rules,
            };
        }

    }
}
