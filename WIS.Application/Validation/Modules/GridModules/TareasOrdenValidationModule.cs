using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.OrdenTarea;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.General;
using WIS.Domain.OrdenTarea;
using WIS.GridComponent;
using WIS.GridComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.GridModules
{
    class TareasOrdenValidationModule : GridValidationModule
    {
        protected readonly IUnitOfWork _uow;
        public TareasOrdenValidationModule(IUnitOfWork uow)
        {
            this._uow = uow;

            this.Schema = new GridValidationSchema
            {
                ["CD_EMPRESA"] = this.ValidateCodigoEmpresa,
                ["CD_TAREA"] = this.ValidateCodigoTarea,
            };
        }

        public virtual GridValidationGroup ValidateCodigoEmpresa(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            var rules = new GridValidationGroup
            {
                Rules = new List<IValidationRule> {
                    new NonNullValidationRule(cell.Value),
                    new PositiveIntValidationRule(cell.Value),
                    new ExisteEmpresaValidationRule(this._uow, cell.Value)
                },
                OnSuccess = this.ValidateCodigoEmpresa_OnSuccess,
            };

            return rules;
        }

        public virtual void ValidateCodigoEmpresa_OnSuccess(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            Empresa empresa = this._uow.EmpresaRepository.GetEmpresa(int.Parse(cell.Value));

            row.GetCell("NM_EMPRESA").Value = empresa.Nombre;
        }

        public virtual GridValidationGroup ValidateCodigoTarea(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {

            string codigoEmpresa = row.GetCell("CD_EMPRESA").Value;
            string numeroOrden  = parameters.Any(s => s.Id == "numeroOrden") ? parameters.FirstOrDefault(s => s.Id == "numeroOrden").Value : "";

            TareaYaAsociadaOrden tareaYaAsociadaOrden = new TareaYaAsociadaOrden(this._uow, int.Parse(numeroOrden), codigoEmpresa, cell.Value);
            var rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(cell.Value),
                    new ValidarTipoTareaManual(this._uow, cell.Value),
                    new ExisteCodigoDeTarea(this._uow, cell.Value),
                    new StringMaxLengthValidationRule(cell.Value, 20)
                };

            if (codigoEmpresa != "" && numeroOrden.ToString() != "" && row.IsNew)
                rules.Add(tareaYaAsociadaOrden);
            else
                rules.Remove(tareaYaAsociadaOrden);

            return new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = rules,
                OnSuccess = this.OnSuccessFormValidateCodigoTarea
            };
        }

        public virtual void OnSuccessFormValidateCodigoTarea(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            if (row.GetCell("CD_TAREA").Value != null)
            {
                Tarea tarea = this._uow.TareaRepository.GetTarea(cell.Value);
                row.GetCell("DS_TAREA").Value = tarea.Descripcion;
                row.GetCell("TP_TAREA").Value = tarea.TipoTarea;
                row.GetCell("NU_COMPONENTE").Value = tarea.NumeroComponente;
            }
        }

    }
}
