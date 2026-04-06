using System;
using System.Collections.Generic;
using System.Text;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.Registro;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.GridComponent;
using WIS.GridComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.GridModules
{
    public class MantenimientoEquipoValidationModule : GridValidationModule
    {
        protected readonly IUnitOfWork _uow;

        public MantenimientoEquipoValidationModule(IUnitOfWork uow)
        {
            _uow = uow;

            this.Schema = new GridValidationSchema
            {
                ["DS_EQUIPO"] = this.ValidateDescripcion,
                ["CD_FERRAMENTA"] = this.ValidateTipoHerramienta,
                ["NU_COMPONENTE"] = this.ValidateComponente
            };
        }



        public virtual GridValidationGroup ValidateDescripcion(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            return new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                   new NonNullValidationRule(cell.Value),
                   new StringMaxLengthValidationRule(cell.Value, 40)
                }
            };
        }

        public virtual GridValidationGroup ValidateTipoHerramienta(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            return new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                   new NonNullValidationRule(cell.Value),
                   new StringMaxLengthValidationRule(cell.Value, 4),
                   new ExisteHerramientaValidationRule(this._uow, cell.Value)
                },
                OnSuccess = this.ValidateTipoHerramienta_OnSuccess,
                OnFailure = this.ValidateTipoHerramienta_OnFailure
            };
        }

        public virtual GridValidationGroup ValidateComponente(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            return new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                   new StringSoloUpperValidationRule(cell.Value),
                   new StringMaxLengthValidationRule(cell.Value, 20)
                }
            };
        }

        public virtual void ValidateTipoHerramienta_OnSuccess(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            var herramienta = _uow.EquipoRepository.GetHerramienta(short.Parse(cell.Value));

            row.GetCell("DS_FERRAMENTA").Value = herramienta.Descripcion;
        }
        public virtual void ValidateTipoHerramienta_OnFailure(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            row.GetCell("DS_FERRAMENTA").Value = string.Empty;
        }
    }
}
