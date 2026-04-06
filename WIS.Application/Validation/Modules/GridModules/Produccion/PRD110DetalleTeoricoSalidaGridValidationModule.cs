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
using WIS.Domain.General;
using WIS.Domain.Produccion.Constants;
using WIS.GridComponent;
using WIS.GridComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.GridModules.Produccion
{
    public class PRD110DetalleTeoricoSalidaGridValidationModule : GridValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly IFormatProvider _formatProvider;

        public PRD110DetalleTeoricoSalidaGridValidationModule(IUnitOfWork uow, IFormatProvider formatProvider)
        {
            _uow = uow;
            _formatProvider = formatProvider;

            Schema = new GridValidationSchema
            {
                ["CD_PRODUTO"] = ValidateCodigoProducto,
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
                rules.Add(new ProductoTeoricoUnicoValidationRule(this._uow, cdEmpresa, idIngreso, cell.Value, CIngresoProduccionDetalleTeorico.TipoDetalleSalida));
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
                row.GetCell("NU_IDENTIFICADOR").Value = "*";
            else
                row.GetCell("NU_IDENTIFICADOR").Value = ManejoIdentificadorDb.IdentificadorAuto;

            row.GetCell("CD_EMPRESA").Value = cdEmpresa.ToString();
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
                new PositiveDecimalValidationRule(_formatProvider, cell.Value),
                new DecimalLengthWithPresicionValidationRule(cell.Value, 12, 3, this._formatProvider, producto.AceptaDecimales)
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
