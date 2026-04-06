using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.General;
using WIS.Application.Validation.Rules.Registro;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.GridComponent;
using WIS.GridComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.GridModules
{
    public class MantenimientoUbicacionesPickingProductoValidationModule : GridValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly IFormatProvider _culture;

        public MantenimientoUbicacionesPickingProductoValidationModule(IUnitOfWork uow, IFormatProvider formatProvider)
        {
            this._uow = uow;
            this._culture = formatProvider;

            this.Schema = new GridValidationSchema
            {
                ["CD_EMPRESA"] = this.ValidateEmpresa,
                ["CD_PRODUTO"] = this.ValidateProducto,
                ["CD_ENDERECO_SEPARACAO"] = this.ValidateUbicacion,
                ["QT_ESTOQUE_MINIMO"] = this.ValidateQtEstoqueMin,
                ["QT_ESTOQUE_MAXIMO"] = this.ValidateQtEstoqueMax,
                ["QT_PADRAO_PICKING"] = this.ValidatePadron,
                ["QT_DESBORDE"] = this.ValidateDesborde,
                ["QT_UNIDAD_CAJA_AUT"] = this.ValidateCantUnidadCajaAUT,
                ["NU_PRIORIDAD"] = this.ValidateNuPrioridad
            };
        }

        #region Validate Empresa
        public virtual GridValidationGroup ValidateEmpresa(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            GetDatosParam(row, parameters, out string idEmpresa, out string codigoProducto);

            if ((!row.IsNew) && !string.IsNullOrEmpty(idEmpresa))
            {
                return null;
            }
            else
            {
                return new GridValidationGroup
                {
                    Rules = new List<IValidationRule>
                    {
                        new NonNullValidationRule(idEmpresa),
                        new PositiveNumberMaxLengthValidationRule(idEmpresa, 10),
                        new ExisteEmpresaValidationRule(_uow, idEmpresa),
                    },
                    OnSuccess = this.ValidateEmpresa_OnSuccess,
                    OnFailure = this.ValidateEmpresa_OnFailure
                };
            }
        }
        public virtual void ValidateEmpresa_OnSuccess(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            if (row.GetCell("NM_EMPRESA") != null)
            {
                GetDatosParam(row, parameters, out string idEmpresa, out string codigoProducto);

                var empresa = _uow.EmpresaRepository.GetEmpresa(int.Parse(idEmpresa));

                row.GetCell("NM_EMPRESA").Value = empresa.Nombre;
            }
        }

        public virtual void ValidateEmpresa_OnFailure(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            if (row.GetCell("NM_EMPRESA") != null)
                row.GetCell("NM_EMPRESA").Value = string.Empty;
        }

        #endregion

        #region ValidateProducto
        public virtual GridValidationGroup ValidateProducto(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            GetDatosParam(row, parameters, out string idEmpresa, out string codigoProducto);

            if ((!row.IsNew) && !string.IsNullOrEmpty(idEmpresa) && !string.IsNullOrEmpty(codigoProducto))
            {
                return null;
            }
            else
            {
                return new GridValidationGroup
                {
                    Rules = new List<IValidationRule>
                    {
                        new NonNullValidationRule(codigoProducto),
                        new StringMaxLengthValidationRule(codigoProducto, 40),
                        new ProductoExisteEmpresaValidationRule(_uow, idEmpresa, codigoProducto),
                    },
                    Dependencies = { "CD_EMPRESA" },
                    OnSuccess = this.ValidateProducto_OnSucess,
                    OnFailure = this.ValidateProducto_Failure,
                };
            }
        }

        public virtual void ValidateProducto_OnSucess(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            if (row.GetCell("DS_PRODUTO") != null)
            {
                GetDatosParam(row, parameters, out string idEmpresa, out string codigoProducto);

                var producto = _uow.ProductoRepository.GetProductoEmpresaAsignadaUsuario(int.Parse(idEmpresa), codigoProducto);

                row.GetCell("DS_PRODUTO").Value = producto.Descripcion;
            }
        }

        public virtual void ValidateProducto_Failure(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            if (row.GetCell("DS_PRODUTO") != null)
                row.GetCell("DS_PRODUTO").Value = string.Empty;
        }

        #endregion

        #region ValidateUbicacion
        public virtual GridValidationGroup ValidateUbicacion(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            GetDatosParam(row, parameters, out string idEmpresa, out string codigoProducto);

            int? empresa = null;
            if (int.TryParse(idEmpresa, out int parsedValue))
                empresa = parsedValue;

            return new GridValidationGroup
            {
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(cell.Value),
                    new UbicacionAreaPickingValidationRule(_uow, cell.Value, empresa, codigoProducto, row.IsNew),
                },
                OnSuccess = this.ValidateUbicacion_OnSuccess,
                OnFailure = this.ValidateUbicacion_OnFailure,
                Dependencies = { "CD_EMPRESA" }
            };
        }

        public virtual void ValidateUbicacion_OnSuccess(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            var ubicacion = _uow.UbicacionRepository.GetUbicacion(row.GetCell("CD_ENDERECO_SEPARACAO").Value);

            row.GetCell("NU_PREDIO").Value = ubicacion.NumeroPredio;
        }

        public virtual void ValidateUbicacion_OnFailure(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            row.GetCell("NU_PREDIO").Value = string.Empty;
        }

        #endregion

        #region ValidatePadron
        public virtual GridValidationGroup ValidatePadron(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            GetDatosParam(row, parameters, out string idEmpresa, out string codigoProducto);

            if (!row.IsNew || string.IsNullOrEmpty(idEmpresa) || string.IsNullOrEmpty(codigoProducto))
                return null;

            var empresa = int.Parse(idEmpresa);
            var predio = row.GetCell("NU_PREDIO").Value;
            var prioridad = row.GetCell("NU_PRIORIDAD").Value;
            var producto = _uow.ProductoRepository.GetProducto(empresa, codigoProducto);

            return new GridValidationGroup
            {
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(cell.Value),
                    new NumeroEnteroValidationRule(cell.Value),
                    new ExistenciaPadronPredioProductoValidationRule(_uow, cell.Value, codigoProducto, empresa, predio, prioridad),
                    new ManejoIdentificadorSerieCantidadValidationRule(cell.Value, producto.ManejoIdentificador, _culture)
                },
                OnSuccess = this.ValidatePadron_OnSuccess,
                OnFailure = this.ValidatePadron_OnFailure,
                Dependencies = { "NU_PREDIO", "CD_PRODUTO" }
            };
        }

        public virtual void ValidatePadron_OnSuccess(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
        }

        public virtual void ValidatePadron_OnFailure(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
        }

        #endregion


        public virtual GridValidationGroup ValidateQtEstoqueMin(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            return new GridValidationGroup
            {
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(cell.Value),
                    new PositiveIntValidationRule(cell.Value, allowZero:false),
                },
            };
        }

        public virtual GridValidationGroup ValidateQtEstoqueMax(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            int.TryParse(cell.Value, out int valor);
            int.TryParse(row.GetCell("QT_ESTOQUE_MINIMO").Value, out int estoqueMin);

            return new GridValidationGroup
            {
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(cell.Value),
                    new PositiveIntValidationRule(cell.Value),
                    new NumeroEnteroMayorQueValidationRule(valor, estoqueMin),
                },
            };
        }

        public virtual GridValidationGroup ValidateDesborde(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            return new GridValidationGroup
            {
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(cell.Value),
                    new PositiveIntValidationRule(cell.Value, allowZero:false),
                },
            };
        }

        public virtual GridValidationGroup ValidateCantUnidadCajaAUT(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            return new GridValidationGroup
            {
                Rules = new List<IValidationRule>
                {
                    new PositiveIntValidationRule(cell.Value, allowZero:false),
                },
            };
        }

        public virtual GridValidationGroup ValidateNuPrioridad(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            GetDatosParam(row, parameters, out string idEmpresa, out string codigoProducto);

            if (string.IsNullOrEmpty(idEmpresa) || string.IsNullOrEmpty(codigoProducto))
                return null;

            var empresa = int.Parse(idEmpresa);
            var predio = row.GetCell("NU_PREDIO").Value;
            var padron = row.GetCell("QT_PADRAO_PICKING").Value;

            return new GridValidationGroup
            {
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(cell.Value),
                    new PositiveIntMayorACeroValidationRule(cell.Value),
                    new PositiveIntLessThanValidationRule(cell.Value, 99),
                    new ExistenciaPadronPredioProductoValidationRule(_uow, padron, codigoProducto, empresa, predio, cell.Value),
                },
            };
        }

        public virtual void GetDatosParam(GridRow row, List<ComponentParameter> parameters, out string idEmpresa, out string codigoProducto)
        {
            var paramEmpresa = parameters.FirstOrDefault(s => s.Id == "empresa")?.Value;
            idEmpresa = string.IsNullOrEmpty(paramEmpresa) ? row.GetCell("CD_EMPRESA").Value : paramEmpresa;

            var paramProducto = parameters.FirstOrDefault(s => s.Id == "producto")?.Value;
            codigoProducto = string.IsNullOrEmpty(paramProducto) ? row.GetCell("CD_PRODUTO").Value : paramProducto;
        }
    }
}
