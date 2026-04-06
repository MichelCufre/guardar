using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.Registro;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General;
using WIS.Domain.Liberacion;
using WIS.GridComponent;
using WIS.GridComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.GridModules
{
    public class MantenimientoProductoCodigoBarrasValidationModule : GridValidationModule
    {

        protected readonly IUnitOfWork _uow;


        public MantenimientoProductoCodigoBarrasValidationModule(IUnitOfWork uow)
        {
            this._uow = uow;

            this.Schema = new GridValidationSchema
            {

                ["CD_BARRAS"] = this.ValidateCodigo,
                ["TP_CODIGO_BARRAS"] = this.ValidateTipoCodigoBarras,
                ["NU_PRIORIDADE_USO"] = this.ValidateNumeroPrioridadUso,
                ["CD_EMPRESA"] = this.ValidateEmpresa,
                ["CD_PRODUTO"] = this.ValidateProducto

            };
        }


        #region ValidateEmpresa
        public virtual GridValidationGroup ValidateEmpresa(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            if ((!row.IsNew) && (parameters.Any(s => s.Id == "empresa")))
            {
                return null;
            }
            else
            {
                string idEmpresa = parameters.Any(s => s.Id == "empresa") ? parameters.FirstOrDefault(s => s.Id == "empresa").Value : row.GetCell("CD_EMPRESA").Value;

                List<IValidationRule> reglas;

                reglas = new List<IValidationRule> {
                    new NonNullValidationRule(idEmpresa),
                    new PositiveNumberMaxLengthValidationRule(idEmpresa, 10),
                    new ExisteEmpresaValidationRule(_uow, idEmpresa),

                };

                return new GridValidationGroup
                {
                    Rules = reglas,
                    OnSuccess = this.ValidateEmpresa_OnSucess,
                    OnFailure = this.ValidateEmpresa_OnFailure
                };
            }
        }

        public virtual void ValidateEmpresa_OnSucess(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            string idEmpresa = parameters.Any(s => s.Id == "empresa") ? parameters.FirstOrDefault(s => s.Id == "empresa").Value : row.GetCell("CD_EMPRESA").Value;

            if (row.GetCell("NM_EMPRESA") != null)
                row.GetCell("NM_EMPRESA").Value = _uow.EmpresaRepository.GetEmpresa(int.Parse(idEmpresa)).Nombre;
        }

        public virtual void ValidateEmpresa_OnFailure(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            row.GetCell("NM_EMPRESA").Value = string.Empty;
        }
        #endregion

        #region ValidateProducto
        public virtual GridValidationGroup ValidateProducto(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            if ((!row.IsNew) && (parameters.Any(s => s.Id == "producto")) && (parameters.Any(s => s.Id == "empresa")))
            {
                return null;
            }
            else
            {
                string idEmpresa = parameters.Any(s => s.Id == "empresa") ? parameters.FirstOrDefault(s => s.Id == "empresa").Value : row.GetCell("CD_EMPRESA").Value;

                string idProducto = parameters.Any(s => s.Id == "producto") ? parameters.FirstOrDefault(s => s.Id == "producto").Value : row.GetCell("CD_PRODUTO").Value;

                List<IValidationRule> reglas;
                reglas = new List<IValidationRule> {
                    new NonNullValidationRule(idProducto),
                    new StringMaxLengthValidationRule(idProducto, 40),
                    new ProductoExisteEmpresaValidationRule(_uow, idEmpresa, idProducto),

                };

                return new GridValidationGroup
                {
                    Rules = reglas,
                    Dependencies = { "CD_EMPRESA" },
                    OnSuccess = this.ValidateProducto_OnSucess,
                    OnFailure = this.ValidateProducto_Failure,
                };
            }
        }

        public virtual void ValidateProducto_OnSucess(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            string idEmpresa = parameters.Any(s => s.Id == "empresa") ? parameters.FirstOrDefault(s => s.Id == "empresa").Value : row.GetCell("CD_EMPRESA").Value;

            string idProducto = parameters.Any(s => s.Id == "producto") ? parameters.FirstOrDefault(s => s.Id == "producto").Value : row.GetCell("CD_PRODUTO").Value;

            string descProd = _uow.ProductoRepository.GetProductoEmpresaAsignadaUsuario(int.Parse(idEmpresa), idProducto).Descripcion;

            if (row.GetCell("DS_PRODUTO") != null)
                row.GetCell("DS_PRODUTO").Value = descProd;
        }

        public virtual void ValidateProducto_Failure(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            if (row.Cells.Any(x => x.Column.Id == "DS_PRODUTO"))
                row.GetCell("DS_PRODUTO").Value = string.Empty;
        }
        #endregion

        #region ValidateCodigoBarras
        public virtual GridValidationGroup ValidateCodigo(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            if (!(row.IsNew) && (row.IsModified) && (row.GetCell("CD_BARRAS").Old == row.GetCell("CD_BARRAS").Value))
                return null;

            string idEmpresa = parameters.Any(s => s.Id == "empresa") ? parameters.FirstOrDefault(s => s.Id == "empresa").Value : row.GetCell("CD_EMPRESA").Value;

            string idProducto = parameters.Any(s => s.Id == "producto") ? parameters.FirstOrDefault(s => s.Id == "producto").Value : row.GetCell("CD_PRODUTO").Value;

            return new GridValidationGroup
            {
                Rules = new List<IValidationRule> {
                    new NonNullValidationRule(cell.Value),
                    new StringMaxLengthValidationRule(cell.Value, 50),
                    new ExisteCodigoBarrasValidationRule(_uow, cell.Value, idEmpresa, idProducto)
                },
                Dependencies = { " CD_PRODUTO" }
            };
        }
        #endregion

        #region ValidateNumeroPrioridadUso
        public virtual GridValidationGroup ValidateNumeroPrioridadUso(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {

            var reglas = new List<IValidationRule>() {
                    new NonNullValidationRule(cell.Value),
                    new PositiveShortNumberMaxLengthValidationRule(cell.Value, 2)
                };

            return new GridValidationGroup { Rules = reglas };
        }
        #endregion

        #region ValidateTipoCodigoBarras
        public virtual GridValidationGroup ValidateTipoCodigoBarras(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(cell.Value))
                cell.Value = BarcodeDb.InternoWis.ToString();

            return new GridValidationGroup
            {
                Rules = new List<IValidationRule> {
                    new ExisteTipoCodigoBarrasValidationRule(_uow, cell.Value)
                },
                OnSuccess = this.ValidateTipoCodigoBarras_OnSuccess
            };
        }
        public virtual void ValidateTipoCodigoBarras_OnSuccess(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            if (row.GetCell("DS_CODIGO_BARRAS") != null)
                row.GetCell("DS_CODIGO_BARRAS").Value = _uow.ProductoCodigoBarraRepository.GetProductoCodigoBarraTipo(int.Parse(cell.Value)).Descripcion;
        }
        #endregion

    }
}
