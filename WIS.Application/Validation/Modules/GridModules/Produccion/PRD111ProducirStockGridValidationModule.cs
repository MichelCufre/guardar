using System;
using System.Collections.Generic;
using WIS.Application.Validation.Rules;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.GridComponent.Validation;
using WIS.GridComponent;
using WIS.Validation;
using WIS.Application.Validation.Rules.Preparacion;
using DocumentFormat.OpenXml.Vml.Office;
using WIS.Application.Validation.Rules.Registro;
using System.Linq;
using WIS.Domain.General;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Application.Validation.Rules.General;
using WIS.Domain.DataModel.Mappers;

namespace WIS.Application.Validation.Modules.GridModules.Produccion
{
    public class PRD111ProducirStockGridValidationModule : GridValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly IFormatProvider _formatProvider;

        public PRD111ProducirStockGridValidationModule(IUnitOfWork uow, IFormatProvider formatProvider)
        {
            this._uow = uow;
            _formatProvider = formatProvider;

            this.Schema = new GridValidationSchema
            {
                ["CD_ENDERECO"] = this.ValidateUbicacion,
                ["CD_EMPRESA"] = this.ValidateEmpresa,
                ["CD_PRODUTO"] = this.ValidateProducto,
                ["NU_IDENTIFICADOR"] = this.ValidateIdentificador,
                ["QT_AJUSTAR"] = this.ValidateCantidadAjustar,
                ["DT_FABRICACAO"] = this.ValidateVencimiento,
            };
        }

        public virtual GridValidationGroup ValidateUbicacion(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            var codigoEspacio = parameters.FirstOrDefault(p => p.Id == "codigoEspacio")?.Value;
            var espacioProduccion = _uow.EspacioProduccionRepository.GetEspacioProduccion(codigoEspacio);

            return new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(cell.Value),
                    new StringMaxLengthValidationRule(cell.Value, 40),
                    new ExisteUbicacionValidationRule(this._uow, cell.Value),
                    new UbicacionPredioValidationRule(this._uow, cell.Value, espacioProduccion.Predio),
                }
            };
        }

        public virtual GridValidationGroup ValidateEmpresa(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            return new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                    {
                        new NonNullValidationRule(cell.Value),
                        new PositiveNumberMaxLengthValidationRule(cell.Value, 10),
                        new ExisteEmpresaValidationRule(_uow, cell.Value),
                    },
                OnSuccess = this.ValidateEmpresaOnSuccess,
                OnFailure = this.ValidateEmpresaOnFailure
            };
        }
        public virtual void ValidateEmpresaOnSuccess(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            var empresa = _uow.EmpresaRepository.GetEmpresa(int.Parse(row.GetCell("CD_EMPRESA").Value));
            row.GetCell("NM_EMPRESA").Value = empresa.Nombre;
        }
        public virtual void ValidateEmpresaOnFailure(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            row.GetCell("NM_EMPRESA").Value = string.Empty;
        }

        public virtual GridValidationGroup ValidateProducto(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            var cdEmpresa = row.GetCell("CD_EMPRESA").Value;

            if (string.IsNullOrEmpty(cdEmpresa))
                return null;

            return new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(cell.Value),
                    new StringMaxLengthValidationRule(cell.Value, 40),
                    new ProductoExisteEmpresaValidationRule(_uow, cdEmpresa, cell.Value),
                },
                Dependencies = { "CD_EMPRESA" },
                OnSuccess = this.ValidateProductoOnSucess,
                OnFailure = this.ValidateProductoFailure,
            };
        }
        public virtual void ValidateProductoOnSucess(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            var cdEmpresa = row.GetCell("CD_EMPRESA").Value;
            var cdProducto = row.GetCell("CD_PRODUTO").Value;

            var producto = _uow.ProductoRepository.GetProductoEmpresaAsignadaUsuario(int.Parse(cdEmpresa), cdProducto);

            row.GetCell("DS_PRODUTO").Value = producto.Descripcion;

            if (producto.IsIdentifiedByProducto())
            {
                row.GetCell("NU_IDENTIFICADOR").Value = ManejoIdentificadorDb.IdentificadorProducto;
                row.GetCell("NU_IDENTIFICADOR").Editable = false;
            }
            else if (row.GetCell("NU_IDENTIFICADOR").Value == ManejoIdentificadorDb.IdentificadorProducto)
            {
                row.GetCell("NU_IDENTIFICADOR").Value = string.Empty;
                row.GetCell("NU_IDENTIFICADOR").Editable = true;
            }

            if (producto.IsFefo())
                row.GetCell("DT_FABRICACAO").Editable = true;
            else
                row.GetCell("DT_FABRICACAO").Editable = false;
        }
        public virtual void ValidateProductoFailure(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            row.GetCell("DS_PRODUTO").Value = string.Empty;
            row.GetCell("NU_IDENTIFICADOR").Value = string.Empty;
        }

        public virtual GridValidationGroup ValidateIdentificador(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            var cdEmpresa = row.GetCell("CD_EMPRESA").Value;
            var cdProducto = row.GetCell("CD_PRODUTO").Value;

            if (string.IsNullOrEmpty(cdEmpresa) || string.IsNullOrEmpty(cdProducto))
                return null;

            var empresa = int.Parse(cdEmpresa);

            var rules = new List<IValidationRule>
            {
                new NonNullValidationRule(cell.Value),
                new StringMaxLengthValidationRule(cell.Value, 40),
                new ManejoIdentificadorSerieExistenteValidationRule(_uow, empresa, cdProducto, cell.Value)
            };

            var producto = this._uow.ProductoRepository.GetProducto(empresa, cdProducto);

            if (!producto.IsIdentifiedByProducto())
                rules.Add(new IdentificadorValidationRule(_uow, cell.Value));
            else
                return null;

            return new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = rules,
                OnSuccess = this.ValidateIdentificadorOnSuccess
            };
        }
        public virtual void ValidateIdentificadorOnSuccess(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            var cdEmpresa = row.GetCell("CD_EMPRESA").Value;
            var cdProducto = row.GetCell("CD_PRODUTO").Value;

            var producto = this._uow.ProductoRepository.GetProducto(int.Parse(cdEmpresa), cdProducto);

            if (producto.IsIdentifiedByProducto())
                cell.Value = ManejoIdentificadorDb.IdentificadorProducto;

            cell.Value = cell.Value.ToUpper();
        }

        public virtual GridValidationGroup ValidateCantidadAjustar(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(row.GetCell("CD_EMPRESA").Value) || string.IsNullOrEmpty(row.GetCell("CD_PRODUTO").Value))
                return null;

            var codigoProducto = row.GetCell("CD_PRODUTO").Value;
            var empresa = int.Parse(row.GetCell("CD_EMPRESA").Value);

            var producto = _uow.ProductoRepository.GetProducto(empresa, codigoProducto);

            return new GridValidationGroup
            {
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(cell.Value),
                    new PositiveDecimalValidationRule(this._formatProvider, cell.Value, false),
                    new DecimalLengthWithPresicionValidationRule(cell.Value, 12, 3, this._formatProvider, producto.AceptaDecimales),
                    new ManejoIdentificadorSerieCantidadValidationRule(cell.Value, producto.ManejoIdentificador, _formatProvider)
                }
            };
        }

        public virtual GridValidationGroup ValidateVencimiento(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            if (row.IsModified && !row.IsNew)
                return null;

            var cdEmpresa = row.GetCell("CD_EMPRESA").Value;
            var cdProducto = row.GetCell("CD_PRODUTO").Value;

            var producto = _uow.ProductoRepository.GetProductoEmpresaAsignadaUsuario(int.Parse(cdEmpresa), cdProducto);
            if (!producto.IsFefo())
                return null;

            return new GridValidationGroup
            {
                BreakValidationChain = true,
                Dependencies = { "CD_PRODUTO", "CD_EMPRESA" },
                Rules =
                {
                    new NonNullValidationRule(cell.Value),
                    new DateTimeValidationRule(cell.Value),
                    new DateTimeGreaterThanCurrentDateValidationRule(cell.Value)
                }
            };
        }
    }
}
