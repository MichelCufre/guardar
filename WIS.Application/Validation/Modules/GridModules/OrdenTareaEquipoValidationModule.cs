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
    public class OrdenTareaEquipoValidationModule : GridValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly IFormatProvider _culture;

        public OrdenTareaEquipoValidationModule(IUnitOfWork uow, IFormatProvider culture)
        {
            this._uow = uow;
            this._culture = culture;
            this.Schema = new GridValidationSchema
            {
                ["CD_EQUIPO"] = this.ValidateCodigoEquipo,
                ["DS_MEMO"] = this.ValidateMemo,
                ["DT_DESDE"] = this.ValidateFechaDesde,
                ["DT_HASTA"] = this.ValidateFechaHasta,

            };
        }

        public virtual GridValidationGroup ValidateCodigoEquipo(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            var numeroOrden = parameters.Any(s => s.Id == "numeroOrden") ? parameters.FirstOrDefault(s => s.Id == "numeroOrden").Value : "";
            var empresa = parameters.Any(s => s.Id == "descripcionEmpresa") ? parameters.FirstOrDefault(s => s.Id == "descripcionEmpresa").Value : "";
            var cdEquipo = int.Parse(row.GetCell("CD_EQUIPO").Value);

            var existeEquipoValidationRule = new ExisteEquipoValidationRule(this._uow, cdEquipo);

            var rules = new List<IValidationRule>
            {
                new NonNullValidationRule( cell.Value),
                new PositiveIntValidationRule(cell.Value),
            };

            if (!string.IsNullOrEmpty(cdEquipo.ToString()))
            {
                rules.Add(existeEquipoValidationRule);
            }
            else
            {
                rules.Remove(existeEquipoValidationRule);
            }

            return new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = rules,
                OnSuccess = this.OnSuccessValidateCodigoEquipo
            };

        }

        public virtual void OnSuccessValidateCodigoEquipo(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            Equipo equipo = this._uow.EquipoRepository.GetEquipo(int.Parse(cell.Value));

            row.GetCell("DS_EQUIPO").Value = equipo.Descripcion;
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
            var cdEquipo = row.GetCell("CD_EQUIPO").Value;

            var validarFecha = row.IsNew
                ? new FechaDesdeOrdenTareaEquipoValidationRule(this._uow, fechaInicioOrden, cell.Value, cdEquipo, this._culture)
                : new FechaDesdeOrdenTareaEquipoValidationRule(this._uow, fechaInicioOrden, cell.Value, cdEquipo, this._culture, row.GetCell("NU_ORT_ORDEN_TAREA_EQUIPO").Value);

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
            var cdEquipo = row.GetCell("CD_EQUIPO").Value;

            var validarFecha = row.IsNew
                ? new FechaHastaOrdenTareaEquipoValidationRule(this._uow, fechaDesde, cell.Value, cdEquipo, this._culture)
                : new FechaHastaOrdenTareaEquipoValidationRule(this._uow, fechaDesde, cell.Value, cdEquipo, this._culture, row.GetCell("NU_ORT_ORDEN_TAREA_EQUIPO").Value);

            var rules = new List<IValidationRule>
            {
                new NonNullValidationRule( cell.Value),
                new DateTimeValidationRule(cell.Value)
            };

            if (!string.IsNullOrEmpty(cell.Value))
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
