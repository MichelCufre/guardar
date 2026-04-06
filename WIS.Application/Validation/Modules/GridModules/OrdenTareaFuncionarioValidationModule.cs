using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.Facturacion;
using WIS.Application.Validation.Rules.OrdenTarea;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.Facturacion;
using WIS.Domain.General;
using WIS.Domain.OrdenTarea;
using WIS.GridComponent;
using WIS.GridComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.GridModules
{
    class OrdenTareaFuncionarioValidationModule : GridValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly IFormatProvider _culture;

        public OrdenTareaFuncionarioValidationModule(IUnitOfWork uow, IFormatProvider culture)
        {
            this._uow = uow;
            this._culture = culture;
            this.Schema = new GridValidationSchema
            {
                ["CD_FUNCIONARIO"] = this.ValidateCodigoFuncionario,
                ["DS_MEMO"] = this.ValidateMemo,
                ["DT_DESDE"] = this.ValidateFechaDesde,
                ["DT_HASTA"] = this.ValidateFechaHasta,

            };
        }

        public virtual GridValidationGroup ValidateCodigoFuncionario(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            var numeroOrden = parameters.Any(s => s.Id == "numeroOrden") ? parameters.FirstOrDefault(s => s.Id == "numeroOrden").Value : "";
            var empresa = parameters.Any(s => s.Id == "descripcionEmpresa") ? parameters.FirstOrDefault(s => s.Id == "descripcionEmpresa").Value : "";
            var cdFuncionario = int.Parse(row.GetCell("CD_FUNCIONARIO").Value);

            var existeFuncionarioValidationRule = new ExisteFuncionarioValidationRule(this._uow, cdFuncionario);

            var rules = new List<IValidationRule>
            {
                new NonNullValidationRule( cell.Value),
                new PositiveIntValidationRule(cell.Value),
            };

            if (!string.IsNullOrEmpty(cdFuncionario.ToString()))
            {
                rules.Add(existeFuncionarioValidationRule);
            }
            else
            {
                rules.Remove(existeFuncionarioValidationRule);
            }

            return new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = rules,
                OnSuccess = this.OnSuccessValidateCodigoFuncionario
            };

        }

        public virtual void OnSuccessValidateCodigoFuncionario(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            Funcionario funcionario = this._uow.FuncionarioRepository.GetFuncionario(int.Parse(cell.Value));

            row.GetCell("NM_FUNCIONARIO").Value = funcionario.Nombre;
        }

        public virtual GridValidationGroup ValidateMemo(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            var rules = new List<IValidationRule>
                {
                    new StringMaxLengthValidationRule(cell.Value, 200),
                };

            return new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = rules,
            };

        }

        public virtual GridValidationGroup ValidateFechaDesde(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            var fechaInicioOrden = parameters.Any(s => s.Id == "fechaInicioOrden") ? parameters.FirstOrDefault(s => s.Id == "fechaInicioOrden").Value : "";
            var cdFuncionario = row.GetCell("CD_FUNCIONARIO").Value;

            var validarFecha = row.IsNew
                ? new FechaDesdeOrdenTareaFuncionarioValidationRule(this._uow, fechaInicioOrden, cell.Value, cdFuncionario, this._culture)
                : new FechaDesdeOrdenTareaFuncionarioValidationRule(this._uow, fechaInicioOrden, cell.Value, cdFuncionario, this._culture, row.GetCell("NU_ORT_ORDEN_TAREA_FUNC").Value);

            var rules = new List<IValidationRule>
                {
                    new NonNullValidationRule( cell.Value),
                    new DateTimeValidationRule(cell.Value)
                };

            if (!string.IsNullOrEmpty(fechaInicioOrden.ToString()))
                rules.Add(validarFecha);
            else
                rules.Remove(validarFecha);

            return new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = rules,
            };

        }

        public virtual GridValidationGroup ValidateFechaHasta(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            string fechaDesde = row.GetCell("DT_DESDE").Value;
            var cdFuncionario = row.GetCell("CD_FUNCIONARIO").Value;

            var validarFecha = row.IsNew
                ? new FechaHastaOrdenTareaFuncionarioValidationRule(this._uow, fechaDesde, cell.Value, cdFuncionario, this._culture)
                : new FechaHastaOrdenTareaFuncionarioValidationRule(this._uow, fechaDesde, cell.Value, cdFuncionario, this._culture, row.GetCell("NU_ORT_ORDEN_TAREA_FUNC").Value);

            var rules = new List<IValidationRule>
            {
                new NonNullValidationRule( cell.Value),
                new DateTimeValidationRule(cell.Value)
            };

            if (!string.IsNullOrEmpty(cell.Value.ToString()))
                rules.Add(validarFecha);
            else
                rules.Remove(validarFecha);

            return new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = rules,
            };
        }
    }
}
