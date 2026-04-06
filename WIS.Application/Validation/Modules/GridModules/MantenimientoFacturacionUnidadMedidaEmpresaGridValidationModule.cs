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
    public class MantenimientoFacturacionUnidadMedidaEmpresaGridValidationModule : GridValidationModule
    {
        protected readonly IUnitOfWork _uow;
        public MantenimientoFacturacionUnidadMedidaEmpresaGridValidationModule(IUnitOfWork uow)
        {
            this._uow = uow;

            this.Schema = new GridValidationSchema
            {
                ["CD_UNIDADE_MEDIDA"] = this.ValidateUnidadMedida,
                ["CD_EMPRESA"] = this.ValidateEmpresa,
            };
        }

        public virtual GridValidationGroup ValidateUnidadMedida(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            var rules = new GridValidationGroup
            {
                Rules = new List<IValidationRule> {
                    new NonNullValidationRule(cell.Value),
                    new NoExisteFacturacionUnidadMedidaRegistradoValidationRule(this._uow, cell.Value)
                },
                OnSuccess = ValidateUnidadMedida_OnSuccess
            };

            string cdEmpresa = row.GetCell("CD_EMPRESA").Value;

            if(!string.IsNullOrEmpty(cdEmpresa))
                rules.Rules.Add(new ExisteFacturacionUnidadMedidaEmpresaRegistradoValidationRule(this._uow, cell.Value, int.Parse(cdEmpresa)));

            return rules;
        }
        public virtual GridValidationGroup ValidateEmpresa(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            var rules = new GridValidationGroup
            {
                Rules = new List<IValidationRule> {
                    new NonNullValidationRule(cell.Value),
                    new ExisteEmpresaValidationRule(this._uow, cell.Value)
                },
                OnSuccess = ValidateEmpresa_OnSuccess
            };

            string unidadMedida = row.GetCell("CD_UNIDADE_MEDIDA").Value;

            if (!string.IsNullOrEmpty(unidadMedida))
                rules.Rules.Add(new ExisteFacturacionUnidadMedidaEmpresaRegistradoValidationRule(this._uow, unidadMedida, int.Parse(cell.Value)));

            return rules;
        }
        public virtual void ValidateUnidadMedida_OnSuccess(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            UnidadMedida unidadMedida = this._uow.UnidadMedidaRepository.GetById(cell.Value);

            row.GetCell("DS_UNIDADE_MEDIDA").Value = unidadMedida.Descripcion;
        }
        public virtual void ValidateEmpresa_OnSuccess(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            Empresa empresa = this._uow.EmpresaRepository.GetEmpresa(int.Parse(cell.Value));

            row.GetCell("NM_EMPRESA").Value = empresa.Nombre;
        }
    }
}
