using System;
using System.Collections.Generic;
using System.Text;
using WIS.Application.Validation.Rules;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.General;
using WIS.GridComponent;
using WIS.GridComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.GridModules
{
    public class INV060GridValidationModule : GridValidationModule
    {
        protected readonly IUnitOfWork _uow;

        public INV060GridValidationModule(IUnitOfWork uow)
        {
            this._uow = uow;

            this.Schema = new GridValidationSchema
            {
                ["DS_MOTIVO"] = this.ValidateDsMotivo,
                ["CD_MOTIVO_AJUSTE"] = this.GridValidateMotivoAjuste
            };
        }

        public virtual GridValidationGroup ValidateDsMotivo(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            List<IValidationRule> rules = new List<IValidationRule>
            {
                new StringMaxLengthValidationRule(cell.Value, 50),
            };

            return new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = rules
            };
        }

        public virtual GridValidationGroup GridValidateMotivoAjuste(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {

            List<IValidationRule> rules = new List<IValidationRule>
            {
                new NonNullValidationRule(cell.Value),
                new StringMaxLengthValidationRule(cell.Value, 3),
                new ExisteMotivoAjusteValidationRule(this._uow, cell.Value)
            };

            return new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = rules,
                OnFailure = this.OnFailureGridValidateMotivoAjuste,
                OnSuccess = this.OnSuccessGridValidateMotivoAjuste
            };
        }

        public virtual void OnFailureGridValidateMotivoAjuste(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            row.GetCell("DS_MOTIVO_AJUSTE").Value = string.Empty;
        }

        public virtual void OnSuccessGridValidateMotivoAjuste(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            MotivoAjuste obj = this._uow.AjusteRepository.GetMotivoAjuste(cell.Value);

            row.GetCell("DS_MOTIVO_AJUSTE").Value = obj.Descripcion;
        }
    }
}
