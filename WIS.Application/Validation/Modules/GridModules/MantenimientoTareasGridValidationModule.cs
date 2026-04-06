using System;
using System.Collections.Generic;
using System.Text;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.OrdenTarea;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.GridComponent;
using WIS.GridComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.GridModules
{
    public class MantenimientoTareasGridValidationModule : GridValidationModule
    {
        protected readonly IUnitOfWork _uow;

        public MantenimientoTareasGridValidationModule(IUnitOfWork uow)
        {
            this._uow = uow;

            this.Schema = new GridValidationSchema
            {
                ["CD_TAREA"] = this.ValidateTarea,
                ["DS_TAREA"] = this.ValidateDsTarea,
                ["CD_SITUACAO"] = this.ValidateSituacion,
                ["NU_COMPONENTE"] = this.ValidateNumeroComponente,
                ["FL_REGISTRO_HORAS"] = this.ValidateRegistroHoras,
                ["FL_REGISTRO_HORAS_EQUIPO"] = this.ValidateRegistroHorasEquipo,
                ["FL_REGISTRO_MANIPULEO"] = this.ValidateRegistroManipuleo,
                ["FL_REGISTRO_INSUMOS"] = this.ValidateRegistroInsumos,
            };
        }

        public virtual GridValidationGroup ValidateTarea(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            if (!row.IsNew)
                return null;

            return new GridValidationGroup
            {
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(cell.Value),
                    new ExisteTareaValidationRule(this._uow, cell.Value),
                    new StringMaxLengthValidationRule(cell.Value, 10),
                    new StringSoloUpperValidationRule(cell.Value)
                }
            };
        }

        public virtual GridValidationGroup ValidateDsTarea(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            return new GridValidationGroup
            {
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(cell.Value),
                    new StringMaxLengthValidationRule(cell.Value, 60),
                }
            };
        }

        public virtual GridValidationGroup ValidateSituacion(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            if (row.IsNew)
                return null;

            return new GridValidationGroup
            {
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(cell.Value)
                },
                OnSuccess = this.ValidateSituacion_OnSucess
            };
        }

        public virtual GridValidationGroup ValidateNumeroComponente(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            return new GridValidationGroup
            {
                Rules = new List<IValidationRule>
                {
                    new StringMaxLengthValidationRule(cell.Value, 20),
                    new StringSoloUpperValidationRule(cell.Value)
                }
            };
        }

        public virtual GridValidationGroup ValidateRegistroHoras(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(cell.Value))
                return null;

            return new GridValidationGroup
            {
                Rules = new List<IValidationRule> {
                    new BooleanStringGridValidationRule(cell.Value)
                }
            };
        }

        public virtual GridValidationGroup ValidateRegistroHorasEquipo(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(cell.Value))
                return null;

            return new GridValidationGroup
            {
                Rules = new List<IValidationRule>
                {
                    new BooleanStringGridValidationRule(cell.Value)
                }
            };
        }

        public virtual GridValidationGroup ValidateRegistroManipuleo(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(cell.Value))
                return null;

            return new GridValidationGroup
            {
                Rules = new List<IValidationRule>
                {
                    new BooleanStringGridValidationRule(cell.Value)
                }
            };
        }

        public virtual GridValidationGroup ValidateRegistroInsumos(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(cell.Value))
                return null;

            return new GridValidationGroup
            {
                Rules = new List<IValidationRule>
                {
                    new BooleanStringGridValidationRule(cell.Value)
                }
            };
        }

        public virtual void ValidateSituacion_OnSucess(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            row.GetCell("DS_SITUACAO").Value = this._uow.SituacionRepository.GetSituacionDescripcion(short.Parse(cell.Value));
        }
    }
}
