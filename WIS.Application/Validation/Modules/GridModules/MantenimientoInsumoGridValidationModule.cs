using System.Collections.Generic;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.General;
using WIS.Application.Validation.Rules.OrdenTarea;
using WIS.Application.Validation.Rules.Registro;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General;
using WIS.Domain.OrdenTarea.Constants;
using WIS.GridComponent;
using WIS.GridComponent.Validation;
using WIS.Security;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.GridModules
{
    public class MantenimientoInsumoGridValidationModule : GridValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly ISecurityService _security;

        public MantenimientoInsumoGridValidationModule(IUnitOfWork uow, ISecurityService security)
        {
            this._uow = uow;
            this._security = security;

            this.Schema = new GridValidationSchema
            {
                ["CD_INSUMO_MANIPULEO"] = this.ValidateInsumoManipuleo,
                ["DS_INSUMO_MANIPULEO"] = this.ValidateDsInsumoManipuleo,
                ["NU_COMPONENTE"] = this.ValidateComponente,
                ["TP_INSUMO_MANIPULEO"] = this.ValidateTpInsumoManipuleo,
                ["CD_PRODUTO"] = this.ValidateProducto,
                ["CD_EMPRESA"] = this.ValidateEmpresa,
                ["FL_TODA_EMPRESA"] = this.ValidateTodaEmpresa

            };
        }

        public virtual GridValidationGroup ValidateInsumoManipuleo(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            if (!row.IsNew)
                return null;

            return new GridValidationGroup
            {
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(cell.Value),
                    new ExisteInsumoManipuleoValidationRule(this._uow, cell.Value),
                    new StringMaxLengthValidationRule(cell.Value, 10),
                    new StringSoloUpperValidationRule(cell.Value)
                }
            };
        }

        public virtual GridValidationGroup ValidateDsInsumoManipuleo(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            return new GridValidationGroup
            {
                Rules = new List<IValidationRule>
                {
                    new StringMaxLengthValidationRule(cell.Value, 60),
                }
            };
        }

        public virtual GridValidationGroup ValidateComponente(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            return new GridValidationGroup
            {
                Rules = new List<IValidationRule>
                {
                    new StringMaxLengthValidationRule(cell.Value, 20),
                    new StringSoloUpperValidationRule(cell.Value)
                }
            };
        }

        public virtual GridValidationGroup ValidateTpInsumoManipuleo(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            string dominio = cell.Value;

            return new GridValidationGroup
            {
                Rules = new List<IValidationRule> {
                    new NonNullValidationRule(cell.Value),
                    new StringMaxLengthValidationRule(cell.Value, 1),
                    new ExisteDominioValidationRule(cell.Value,_uow,CodigoDominioDb.TipoInsumoManipuleo)
                },
                OnSuccess = this.ValidateTpInsumoManipuleo_OnSucess
            };
        }

        public virtual GridValidationGroup ValidateProducto(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {

            var tipo = row.GetCell("TP_INSUMO_MANIPULEO").Value;

            if (tipo != OrdenTareaDb.TipoInsumo || !int.TryParse(row.GetCell("CD_EMPRESA").Value, out _))
                return null;

            var idEmpresa = row.GetCell("CD_EMPRESA").Value;

            return new GridValidationGroup
            {
                Rules = new List<IValidationRule>
                {
                    new StringMaxLengthValidationRule(cell.Value, 40),
                    new ProductoExisteEmpresaValidationRule(_uow, idEmpresa, cell.Value),
                    new IsTipoInsumoValidationRule(tipo)
                },
                Dependencies = { "CD_EMPRESA", "TP_INSUMO_MANIPULEO" }
            };
        }

        public virtual GridValidationGroup ValidateEmpresa(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            var tipo = row.GetCell("TP_INSUMO_MANIPULEO").Value;

            if (tipo != OrdenTareaDb.TipoInsumo || !int.TryParse(cell.Value, out _))
                return null;

            return new GridValidationGroup
            {
                Rules = new List<IValidationRule>
                {
                    new StringMaxLengthValidationRule(cell.Value, 10),
                    new PositiveIntValidationRule(cell.Value),
                    new ExisteEmpresaValidationRule(this._uow, cell.Value),
                    new UserCanWorkWithEmpresaValidationRule(this._security, cell.Value),
                    new IsTipoInsumoValidationRule(tipo)
                },
                Dependencies = { "TP_INSUMO_MANIPULEO" }
            };
        }

        public virtual GridValidationGroup ValidateTodaEmpresa(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            if (row.IsNew && string.IsNullOrEmpty(cell.Value))
                cell.Value = "N";

            return new GridValidationGroup
            {
                Rules = new List<IValidationRule> {
                    new NonNullValidationRule(cell.Value),
                    new StringMaxLengthValidationRule(cell.Value, 1),
                    new StringToBooleanValidationRule(cell.Value)
                }
            };
        }

        public virtual void ValidateTpInsumoManipuleo_OnSucess(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {

            if (cell.Value == OrdenTareaDb.TipoManipuleo)
            {
                row.GetCell("CD_EMPRESA").Value = "";
                row.GetCell("CD_PRODUTO").Value = "";
                row.GetCell("CD_PRODUTO").Editable = false;
                row.GetCell("CD_EMPRESA").Editable = false;

            }
            else
            {
                row.GetCell("CD_PRODUTO").Editable = true;
                row.GetCell("CD_EMPRESA").Editable = true;
            }
        }
    }
}
