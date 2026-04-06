using System;
using System.Collections.Generic;
using System.Text;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.Registro;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.General;
using WIS.GridComponent;
using WIS.GridComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.GridModules
{
    public class MantenimientoUbicacionClaseGridValidationModule : GridValidationModule
    {
        protected readonly IUnitOfWork _uow;
        public MantenimientoUbicacionClaseGridValidationModule(IUnitOfWork uow)
        {
            this._uow = uow;

            this.Schema = new GridValidationSchema
            {
                ["CD_CLASSE"] = this.ValidateCodigo,
                ["DS_CLASSE"] = this.ValidateDescripcion,
                ["CD_SUB_CLASSE"] = this.ValidateSuperClase,
            };
        }
        public virtual GridValidationGroup ValidateCodigo(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            if (!row.IsNew)
                return null;

            return new GridValidationGroup
            {
                Rules = new List<IValidationRule> {
                    new NonNullValidationRule(cell.Value),
                    new StringMaxLengthValidationRule(cell.Value, 2),
                    new ClaseExistenteValidationRule(_uow, cell.Value)
                }
            };
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
        public virtual GridValidationGroup ValidateSuperClase(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            if (!string.IsNullOrEmpty(cell.Value))
            {
                var rules = new List<IValidationRule>();
                rules.Add(new SuperClaseNoExistenteValidationRule(_uow, cell.Value));

                return new GridValidationGroup
                {
                    Rules = rules,
                    OnSuccess = this.ValidateSuperClase_OnSuccess,
                    OnFailure = this.ValidateSuperClase_OnFailure
                };
            }
            else
                return null;
        }

        public virtual void ValidateSuperClase_OnSuccess(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            var celdaDesSuperClase = row.GetCell("DS_SUB_CLASSE");

            SuperClase superClase = _uow.ClaseRepository.GetSuperClaseById(cell.Value);
            celdaDesSuperClase.Value = superClase?.Descripcion;
        }
        public virtual void ValidateSuperClase_OnFailure(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            row.GetCell("DS_SUB_CLASSE").Value = string.Empty;
        }
    }
}
