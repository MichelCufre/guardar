using System;
using System.Collections.Generic;
using System.Linq;
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
    public class MantenimientoControlDeCalidadEnProductoClaseGridValidationModule : GridValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly string _empresa;
        protected readonly string _producto;
        public MantenimientoControlDeCalidadEnProductoClaseGridValidationModule(IUnitOfWork uow)
        {
            this._uow = uow;
            this._empresa = null;
            this._producto = null;

            this.Schema = new GridValidationSchema
            {
                ["CD_CONTROL"] = this.ValidateCodigo,
                ["CD_EMPRESA"] = this.ValidateEmpresa,
                ["CD_PRODUTO"] = this.ValidateProducto,
            };
        }

        public MantenimientoControlDeCalidadEnProductoClaseGridValidationModule(IUnitOfWork uow, string empresa, string producto) : this(uow)
        {
            this._uow = uow;
            this._empresa = empresa;
            this._producto = producto;
        }

        public virtual GridValidationGroup ValidateCodigo(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            var celdaEmpresa = row.GetCell("CD_EMPRESA");
            var celdaProducto = row.GetCell("CD_PRODUTO");
            string empresa = this._empresa != null ? this._empresa : celdaEmpresa.Value;
            string producto = this._producto != null ? this._producto : celdaProducto.Value;
            var rules = new List<IValidationRule>()
            {
                new NonNullValidationRule(cell.Value),
                new StringMaxLengthValidationRule(cell.Value, 10)
            };

            //Chequeo para insert desde excell
            if (parameters.Any(s => s.Id == "importExcel"))
            {
                if (empresa == this._empresa && producto == this._producto)
                    rules.Add(new ControlDeCalidadEnProductoInsertExcellValidationRule(_uow, empresa, producto, celdaEmpresa.Value, celdaProducto.Value));
            }

            if (row.IsNew || row.IsModified)
                rules.Add(new ControlDeCalidadEnProductoInexistenteValidationRule(this._uow, cell.Value, empresa, producto));

            if (!row.IsNew && cell.Old != cell.Value)
                rules.Add(new ControlDeCalidadPendienteValidationRule(this._uow, cell.Value, empresa, producto));

            return new GridValidationGroup
            {
                Rules = rules,
                OnSuccess = ValidateCodigo_onSuccess,
                OnFailure = ValidateCodigo_onFailure,
                Dependencies = { "CD_PRODUTO" }
            };
        }
        public virtual void ValidateCodigo_onSuccess(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            var celdaDesControl = row.GetCell("DS_CONTROL");

            ControlDeCalidad controlClase = _uow.ControlDeCalidadRepository.GetTipoControlDeCalidad(int.Parse(cell.Value));

            if (celdaDesControl != null)
                celdaDesControl.Value = controlClase.Descripcion;
        }
        public virtual void ValidateCodigo_onFailure(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            var celdaControl = row.GetCell("DS_CONTROL");
            if (celdaControl != null)
                celdaControl.Value = string.Empty;
        }

        public virtual GridValidationGroup ValidateEmpresa(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            var celdaEmpresa = row.GetCell("CD_EMPRESA");
            string empresa = this._empresa != null ? this._empresa : celdaEmpresa.Value;
            var rules = new List<IValidationRule>()
            {
                new NonNullValidationRule(empresa),
                new StringMaxLengthValidationRule(empresa, 10),
                new ExisteEmpresaValidationRule(this._uow, empresa),
            };
            return new GridValidationGroup
            {
                Rules = rules,
                OnSuccess = this.ValidateEmpresa_onSuccess,
                OnFailure = this.ValidateEmpresa_onFailure,
            };
        }
        public virtual void ValidateEmpresa_onSuccess(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            var celdaNomEmpresa = row.GetCell("NM_EMPRESA");
            if (this._empresa != null)
                row.GetCell("CD_EMPRESA").Value = this._empresa;

            Empresa empresa = _uow.EmpresaRepository.GetEmpresa(int.Parse(cell.Value));

            if (celdaNomEmpresa != null)
                celdaNomEmpresa.Value = empresa.Nombre;
        }
        public virtual void ValidateEmpresa_onFailure(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            var celdaEmpresa = row.GetCell("NM_EMPRESA");
            if (celdaEmpresa != null)
                celdaEmpresa.Value = string.Empty;
        }

        public virtual GridValidationGroup ValidateProducto(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            var celdaEmpresa = row.GetCell("CD_EMPRESA");
            string empresa = this._empresa != null ? this._empresa : celdaEmpresa.Value;
            string producto = this._producto != null ? this._producto : cell.Value;
            var rules = new List<IValidationRule>
            {
                new NonNullValidationRule(producto),
                new ProductoExistsValidationRule(this._uow, empresa, producto)
            };
            return new GridValidationGroup
            {
                Rules = rules,
                OnSuccess = this.ValidateProducto_onSuccess,
                OnFailure = this.ValidateProducto_onFailure,
                Dependencies = { "CD_EMPRESA" }
            };

        }
        public virtual void ValidateProducto_onSuccess(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            if (this._producto != null)
                row.GetCell("CD_PRODUTO").Value = this._producto;

            var celdaDesProducto = row.GetCell("DS_PRODUTO");
            var empresaId = row.GetCell("CD_EMPRESA").Value;

            Producto producto = _uow.ProductoRepository.GetProductoEmpresaAsignadaUsuario(int.Parse(empresaId), cell.Value);

            if (celdaDesProducto != null)
                celdaDesProducto.Value = producto.Descripcion;
        }
        public virtual void ValidateProducto_onFailure(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            var celdaProducto = row.GetCell("DS_PRODUTO");
            if (celdaProducto != null)
                celdaProducto.Value = string.Empty;
        }
    }
}
