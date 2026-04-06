using System;
using System.Collections.Generic;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.Produccion;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.Produccion;
using WIS.GridComponent;
using WIS.GridComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.GridModules.Produccion
{
    public class PRD120GridValidationModule : GridValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly IFormatProvider _culture;

        public PRD120GridValidationModule(IUnitOfWork uow,
            IFormatProvider culture)
        {
            _uow = uow;
            _culture = culture;

            Schema = new GridValidationSchema
            {
                ["DS_ACCION_INSTANCIA"] = this.ValidateDS_ACCION_INSTANCIA,
                ["CD_ACCION"] = this.ValidateCD_ACCION,
                ["VL_PARAMETRO1"] = this.ValidateParametro,
                ["VL_PARAMETRO2"] = this.ValidateParametro,
                ["VL_PARAMETRO3"] = this.ValidateParametro
            };
        }

        public virtual GridValidationGroup ValidateDS_ACCION_INSTANCIA(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            return new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                    {
                        new NonNullValidationRule(cell.Value),
                        new StringMaxLengthValidationRule(cell.Value, 100),
                    }
            };
        }

        public virtual GridValidationGroup ValidateCD_ACCION(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {

            var Rules = new List<IValidationRule> 
            {
                 new NonNullValidationRule(cell.Value),
                 new AccionExistsValidationRule(this._uow,cell.Value),
            };

            return new GridValidationGroup
            {
                BreakValidationChain = true,
                OnSuccess = this.ValidateCodigoAccion_OnSucess,
            };
        }

        public virtual GridValidationGroup ValidateParametro(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(cell.Value))
                return null;

            return new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                    {
                        new StringMaxLengthValidationRule(cell.Value, 100)
                    }
            };
        }

        public virtual void ValidateCodigoAccion_OnSucess(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            string codigoAccion = row.GetCell("CD_ACCION").Value;
            if (codigoAccion != "")
            {
                Accion accion = this._uow.AccionRepository.GetAccion(codigoAccion);
                row.GetCell("DS_ACCION").Value = accion?.Descripcion;
            }
        }
    }
}
