using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.Facturacion;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.Facturacion;
using WIS.Domain.OrdenTarea;
using WIS.GridComponent;
using WIS.GridComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.GridModules
{
    public class OrdenTareaManipuleoInsumoValidationModule : GridValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly IFormatProvider _culture;

        public OrdenTareaManipuleoInsumoValidationModule(IUnitOfWork uow, IFormatProvider culture)
        {
            this._uow = uow;
            this._culture = culture;

            this.Schema = new GridValidationSchema
            {
                ["CD_INSUMO_MANIPULEO"] = this.ValidateCodigoInsumo,
                ["QT_INSUMO_MANIPULEO"] = this.ValidateCantidad,

            };
        }

        public virtual GridValidationGroup ValidateCodigoInsumo(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            return new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new StringMaxLengthValidationRule(cell.Value, 10),
                },
                OnSuccess = this.OnSuccessValidateCodigoInsumo
            };

        }

        public virtual void OnSuccessValidateCodigoInsumo(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            if (!string.IsNullOrEmpty(cell.Value))
            {
                InsumoManipuleo insumoManipuleo = this._uow.InsumoManipuleoRepository.GetInsumoManipuleo(cell.Value);
                row.GetCell("DS_INSUMO_MANIPULEO").Value = insumoManipuleo.Descripcion;
            }
        }

        public virtual GridValidationGroup ValidateCantidad(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {

            var rules = new List<IValidationRule>
                {
                    new NonNullValidationRule( cell.Value),
                    new DecimalLengthWithPresicionValidationRule(cell.Value, 15, 3, this._culture),
                };

            return new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = rules,
            };
        }
    }
}
