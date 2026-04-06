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
    public class MantenimientoValorParametrosSistemaGridValidationModule : GridValidationModule
    {
        protected readonly IUnitOfWork _uow;
        public MantenimientoValorParametrosSistemaGridValidationModule(IUnitOfWork uow)
        {
            this._uow = uow;

            this.Schema = new GridValidationSchema
            {
                ["ND_ENTIDAD"] = this.ValidateEntidad,
                ["VL_PARAMETRO"] = this.ValidateValorParametro,
            };
        }

        public virtual GridValidationGroup ValidateValorParametro(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            return new GridValidationGroup
            {
                Rules = new List<IValidationRule> {
                    new NonNullValidationRule(cell.Value),
                    new StringMaxLengthValidationRule(cell.Value, 4000),
                }
            };
        }

        public virtual GridValidationGroup ValidateEntidad(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            return new GridValidationGroup
            {
                Rules = new List<IValidationRule> {
                    new NonNullValidationRule(cell.Value),
                    new StringMaxLengthValidationRule(cell.Value, 50),
                },
                OnSuccess = this.OnSuccessGridValidateEntidad
            };
        }
        public virtual void OnSuccessGridValidateEntidad(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            row.GetCell("CD_DOMINIO_VALOR").Value = this._uow.DominioRepository.GetDominio(cell.Value)?.Valor;
            row.GetCell("DS_DOMINIO_VALOR").Value = this._uow.DominioRepository.GetDominio(cell.Value)?.Descripcion;
        }

    }
}
