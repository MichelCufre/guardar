using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.General;
using WIS.Application.Validation.Rules.Produccion;
using WIS.Application.Validation.Rules.Registro;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Repositories;
using WIS.Domain.Produccion.Constants;
using WIS.Domain.Recepcion;
using WIS.GridComponent;
using WIS.GridComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.GridModules.Produccion
{
    public class PRD110DetalleTeoricoEntradaGridValidationModule : GridValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly IFormatProvider _formatProvider;

        public PRD110DetalleTeoricoEntradaGridValidationModule(IUnitOfWork uow, IFormatProvider formatProvider)
        {
            _uow = uow;
            _formatProvider = formatProvider;

            Schema = new GridValidationSchema
            {
                ["CD_PRODUTO"] = ValidateCodigoProducto,
                ["NU_IDENTIFICADOR"] = ValidateIdentificador,
                ["QT_TEORICO"] = ValidateCantidad,
            };
        }

        public virtual GridValidationGroup ValidateCodigoProducto(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            var cdEmpresa = int.Parse(parameters.FirstOrDefault(f => f.Id == "empresa").Value);

            var rules = new List<IValidationRule>
            {
                new NonNullValidationRule(cell.Value),
                new StringMaxLengthValidationRule(cell.Value, 40),
                new ProductoExistsValidationRule(this._uow, cdEmpresa.ToString(), cell.Value)
            };

            if (row.IsNew && parameters.Any(f => f.Id == "idIngreso"))
            {
                var idIngreso = parameters.FirstOrDefault(f => f.Id == "idIngreso").Value;
                rules.Add(new ProductoTeoricoUnicoValidationRule(this._uow, cdEmpresa, idIngreso, cell.Value, CIngresoProduccionDetalleTeorico.TipoDetalleEntrada));
            }

            return new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = rules,
                OnSuccess = ValidateProductoOnSuccess
            };
        }

        public virtual void ValidateProductoOnSuccess(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            var cdProducto = row.GetCell("CD_PRODUTO").Value;
            var cdEmpresa = int.Parse(parameters.FirstOrDefault(f => f.Id == "empresa").Value);

            var producto = _uow.ProductoRepository.GetProductoEmpresaAsignadaUsuario(cdEmpresa, cdProducto);

            row.GetCell("DS_PRODUTO").Value = producto.Descripcion;

            if (producto.IsIdentifiedByProducto())
            {
                row.GetCell("NU_IDENTIFICADOR").Editable = true;
                row.GetCell("NU_IDENTIFICADOR").Value = "*";
                row.GetCell("NU_IDENTIFICADOR").Editable = false;
            }
            else if (string.IsNullOrEmpty(row.GetCell("NU_IDENTIFICADOR").Value) || (!producto.IsIdentifiedByProducto() && row.GetCell("NU_IDENTIFICADOR").Value == ManejoIdentificadorDb.IdentificadorProducto))
            {
                row.GetCell("NU_IDENTIFICADOR").Editable = true;
                row.GetCell("NU_IDENTIFICADOR").Value = ManejoIdentificadorDb.IdentificadorAuto;
            }

            row.GetCell("CD_EMPRESA").Value = cdEmpresa.ToString();
        }

        public virtual GridValidationGroup ValidateIdentificador(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            var cdProducto = row.GetCell("CD_PRODUTO").Value;
            var empresa = int.Parse(parameters.FirstOrDefault(f => f.Id == "empresa").Value);

            var rules = new List<IValidationRule>();

            if (_uow.ProductoRepository.ExisteProducto(empresa, cdProducto))
            {
                var producto = this._uow.ProductoRepository.GetProductoEmpresaAsignadaUsuario(empresa, cdProducto);
                cell.Value = producto.ParseIdentificador(cell.Value);

                rules.Add(new StringMaxLengthValidationRule(cell.Value, 40));
                rules.Add(new IdentificadorProductoValidationRule(this._uow, empresa, cdProducto, cell.Value));

                if ((row.IsNew || (!row.IsNew && row.IsModified && cell.Old != cell.Value)) && parameters.Any(f => f.Id == "idIngreso"))
                {
                    var idIngreso = parameters.FirstOrDefault(f => f.Id == "idIngreso").Value;
                    rules.Add(new ProductoLoteTeoricoUnicoValidationRule(this._uow, empresa, idIngreso, cdProducto, cell.Value, CIngresoProduccionDetalleTeorico.TipoDetalleEntrada));

                }
            }

            return new GridValidationGroup
            {
                BreakValidationChain = true,
                Dependencies = { "CD_PRODUTO" },
                Rules = rules,
            };
        }

        public virtual GridValidationGroup ValidateCantidad(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            var empresa = int.Parse(parameters.FirstOrDefault(f => f.Id == "empresa").Value);
            var cdProducto = row.GetCell("CD_PRODUTO").Value;
            var identificador = row.GetCell("NU_IDENTIFICADOR").Value;

            if (string.IsNullOrEmpty(cdProducto) || string.IsNullOrEmpty(identificador))
                return null;

            var producto = this._uow.ProductoRepository.GetProducto(empresa, cdProducto);

            var rules = new List<IValidationRule>
            {
                new NonNullValidationRule(cell.Value),
                new PositiveDecimalValidationRule(this._formatProvider, cell.Value, false),
                new DecimalLengthWithPresicionValidationRule(cell.Value, 12, 3, this._formatProvider,producto.AceptaDecimales),

            };

            if (identificador != ManejoIdentificadorDb.IdentificadorAuto)
                rules.Add(new ManejoIdentificadorSerieCantidadValidationRule(cell.Value, producto.ManejoIdentificador, _formatProvider));

            return new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = rules

            };
        }
    }
}
