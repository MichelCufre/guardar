using System;
using System.Collections.Generic;
using System.Text;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.Facturacion;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.General;
using WIS.GridComponent;
using WIS.GridComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.GridModules
{
    public class MantenimientoFacturacionUnidadMedidaGridValidationModule : GridValidationModule
    {
        protected readonly IUnitOfWork _uow;
        public MantenimientoFacturacionUnidadMedidaGridValidationModule(IUnitOfWork uow)
        {
            this._uow = uow;

            this.Schema = new GridValidationSchema
            {
                ["CD_UNIDADE_MEDIDA"] = this.ValidateUnidadMedida,
                ["NU_COMPONENTE"] = this.ValidateComponente,
                ["ID_USO"] = this.ValidateUso,
            };
        }

        public virtual GridValidationGroup ValidateUnidadMedida(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {

            var rules = new GridValidationGroup
            {
                Rules = new List<IValidationRule> {
                    new NonNullValidationRule(cell.Value),
                    new StringMaxLengthValidationRule(cell.Value, 10),
                },
                OnSuccess = ValidateUnidadMedida_OnSuccess
            };

            if (row.IsNew || row.IsModified && row.GetCell("CD_UNIDADE_MEDIDA").Old != row.GetCell("CD_UNIDADE_MEDIDA").Value)
            {
                rules.Rules.Add(new ExisteFacturacionUnidadMedidaRegistradoValidationRule(_uow, cell.Value));
            }

            return rules;
        }

        public virtual GridValidationGroup ValidateComponente(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            return new GridValidationGroup
            {
                Rules = new List<IValidationRule> {
                    new StringMaxLengthValidationRule(cell.Value, 20),
                    new StringSoloUpperValidationRule(cell.Value)
                }
            };
        }
        public virtual GridValidationGroup ValidateUso(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            if (row.IsNew)
                return null;

            return new GridValidationGroup
            {
                Rules = new List<IValidationRule> {
                    new StringToBooleanValidationRule(cell.Value),
                }
            };
        }
        public virtual void ValidateUnidadMedida_OnSuccess(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            UnidadMedida unidadMedida = this._uow.UnidadMedidaRepository.GetById(cell.Value);

            row.GetCell("DS_UNIDADE_MEDIDA").Value = unidadMedida.Descripcion;
        }
    }
}
