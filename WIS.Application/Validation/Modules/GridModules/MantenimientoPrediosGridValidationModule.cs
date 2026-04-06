using System.Collections.Generic;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.Registro;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.General.Configuracion;
using WIS.GridComponent;
using WIS.GridComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.GridModules
{
    public class MantenimientoPrediosGridValidationModule : GridValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly UbicacionConfiguracion _ubicacionConfiguracion;
        public MantenimientoPrediosGridValidationModule(IUnitOfWork uow)
        {
            this._uow = uow;
            this._ubicacionConfiguracion = this._uow.UbicacionRepository.GetUbicacionConfiguracion();

            this.Schema = new GridValidationSchema
            {
                ["NU_PREDIO"] = this.ValidateCodigo,
                ["DS_PREDIO"] = this.ValidateDescripcion,
                ["ID_EXTERNO"] = this.ValidateIdExterno
            };
        }

        public virtual GridValidationGroup ValidateCodigo(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            if (!row.IsNew)
                return null;

            var rules = new List<IValidationRule>()
            {
                new NonNullValidationRule(cell.Value),
                new StringMaxLengthValidationRule(cell.Value,  this._ubicacionConfiguracion.PredioLargo),
                new IdPredioExistenteValidationRule(this._uow, cell.Value)
            };

            if (_ubicacionConfiguracion.PredioNumerico)
            {
                rules.Add(new PositiveNumberMaxLengthValidationRule(cell.Value, _ubicacionConfiguracion.PredioLargo));
            }
            else
            {
                cell.Value = cell.Value.ToUpper();
                rules.Add(new StringSoloLetrasValidationRule(cell.Value.ToUpper()));
            }

            var validateGroup = new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = rules,
            };

            return validateGroup;
        }
        public virtual GridValidationGroup ValidateDescripcion(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            return new GridValidationGroup
            {
                Rules = new List<IValidationRule> {
                    new NonNullValidationRule(cell.Value),
                    new StringMaxLengthValidationRule(cell.Value, 100),
                },
            };
        }
        public virtual GridValidationGroup ValidateIdExterno(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(cell.Value))
                return null;

            return new GridValidationGroup
            {
                Rules = new List<IValidationRule> {
                    new StringMaxLengthValidationRule(cell.Value, 50),
                },
            };
        }
    }
}
