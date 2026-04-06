using System;
using System.Collections.Generic;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.Produccion;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.GridComponent;
using WIS.GridComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.GridModules.Produccion
{
    public class PRD113ConsumirInsumosGridValidationModule : GridValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly IFormatProvider _culture;

        public PRD113ConsumirInsumosGridValidationModule(IUnitOfWork uow, IFormatProvider culture)
        {
            this._uow = uow;
            this._culture = culture;

            this.Schema = new GridValidationSchema
            {
                ["ND_MOTIVO"] = this.ValidateMotivo,
            };
        }

        public virtual GridValidationGroup ValidateMotivo(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            return new GridValidationGroup
            {
                Rules = new List<IValidationRule> {
                    new NonNullValidationRule(cell.Value),
                    new ExisteDominioValidationRule(cell.Value, this._uow),
                    new MotivoConsumoPermitidoValidationRule(this._uow, cell.Value, row.GetCell("FL_CONSUMIBLE").Value=="S")
                },
                OnSuccess = this.ValidateMotivoProducirOnSuccess
            };
        }

        public virtual void ValidateMotivoProducirOnSuccess(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            var descripcionMotivo = this._uow.DominioRepository.GetDominio(cell.Value).Descripcion;
            row.GetCell("DS_MOTIVO").Value = descripcionMotivo;
        }
    }
}
