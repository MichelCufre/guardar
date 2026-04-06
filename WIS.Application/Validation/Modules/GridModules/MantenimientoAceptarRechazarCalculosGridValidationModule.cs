using System;
using System.Collections.Generic;
using System.Text;
using WIS.Application.Validation.Rules;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.GridComponent;
using WIS.GridComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.GridModules
{
    public class MantenimientoAceptarRechazarCalculosGridValidationModule : GridValidationModule
    {
        protected readonly IUnitOfWork _uow;

        public MantenimientoAceptarRechazarCalculosGridValidationModule(IUnitOfWork uow)
        {
            this._uow = uow;

            this.Schema = new GridValidationSchema
            {
                ["CD_SITUACAO"] = this.ValidateSituacion,
            };
        }

        public virtual GridValidationGroup ValidateSituacion(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            return new GridValidationGroup
            {
                Rules = new List<IValidationRule> {
                    new NonNullValidationRule(cell.Value),
                },
                OnSuccess = this.ValidateSituacion_OnSuccess
            };
        }
        public virtual void ValidateSituacion_OnSuccess(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
             string descripcionSituacion = this._uow.SituacionRepository.GetSituacionDescripcion(short.Parse(cell.Value));

            row.GetCell("DS_SITUACAO").Value = descripcionSituacion;
        }
    }
}
