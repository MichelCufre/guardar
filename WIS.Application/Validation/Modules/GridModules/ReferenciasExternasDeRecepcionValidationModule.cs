using System.Collections.Generic;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.Registro;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.GridComponent;
using WIS.GridComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.GridModules
{
    public class ReferenciasExternasDeRecepcionValidationModule : GridValidationModule
    {
        protected readonly IUnitOfWork _uow;

        public ReferenciasExternasDeRecepcionValidationModule(IUnitOfWork uow)
        {
            this._uow = uow;

            this.Schema = new GridValidationSchema
            {
                ["TP_RECEPCION_EXTERNO"] = this.ValidateTipoExterno,
                ["DS_RECEPCION_EXTERNO"] = this.ValidateDescExterno,
            };
        }

        public virtual GridValidationGroup ValidateTipoExterno(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {

            if (!int.TryParse(row.GetCell("CD_EMPRESA").Value, out int cdEmpresa))
                row.GetCell("CD_EMPRESA").SetError("General_Sec0_Error_Error25", new List<string>());

            if(cell.Value == cell.Old)
            {
                cell.SetOk();
                return null;
            }

            return new GridValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule> {
                    new StringMaxLengthValidationRule(cell.Value, 20),
                    new ExisteCodigoExternoEmpresaValidationRule(_uow, cell.Value, cdEmpresa),
                },
            };
        }

        public virtual GridValidationGroup ValidateDescExterno(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            return new GridValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule> {
                    new StringMaxLengthValidationRule(cell.Value, 60),
                }
            };
        }
    }
}
