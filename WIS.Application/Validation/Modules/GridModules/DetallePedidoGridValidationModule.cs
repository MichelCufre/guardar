using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.General;
using WIS.Application.Validation.Rules.Preparacion;
using WIS.Application.Validation.Rules.Registro;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.GridComponent;
using WIS.GridComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.GridModules
{
    public class DetallePedidoGridValidationModule : GridValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly int _empresa;
        protected readonly string _cliente;
        protected readonly string _pedido;
        protected readonly IFormatProvider _formatProvider;

        public DetallePedidoGridValidationModule(IUnitOfWork uow, IFormatProvider formatProvider, int empresa, string cliente, string pedido)
        {
            this._uow = uow;
            this._empresa = empresa;
            this._cliente = cliente;
            this._pedido = pedido;
            this._formatProvider = formatProvider;

            this.Schema = new GridValidationSchema
            {
                ["CD_PRODUTO"] = this.ValidateProducto,
                ["NU_IDENTIFICADOR"] = this.ValidateIdentificador,
                ["QT_PEDIDO"] = this.ValidateCantidadPedido
            };
        }

        public virtual GridValidationGroup ValidateProducto(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            var rules = new List<IValidationRule> {
                new NonNullValidationRule(cell.Value),
                new StringMaxLengthValidationRule(cell.Value, 40),
                new ProductoExistsValidationRule(this._uow, this._empresa.ToString(), cell.Value)
            };

            if (row.IsNew)
                rules.Add(new ProductoPedidoUnicoValidationRule(this._uow, this._empresa, this._cliente, this._pedido, cell.Value));

            return new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = rules,
                OnSuccess = this.ValidateProductoOnSuccess
            };
        }
        public virtual GridValidationGroup ValidateIdentificador(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            var productoId = row.GetCell("CD_PRODUTO").Value;

            var rules = new List<IValidationRule>();

            if (_uow.ProductoRepository.ExisteProducto(this._empresa, productoId))
            {
                var producto = this._uow.ProductoRepository.GetProductoEmpresaAsignadaUsuario(this._empresa, productoId);
                cell.Value = producto.ParseIdentificador(cell.Value);

                rules.Add(new StringMaxLengthValidationRule(cell.Value, 40));
                rules.Add(new IdentificadorProductoValidationRule(this._uow, this._empresa, productoId, cell.Value));
            }

            if (row.IsNew)
                rules.Add(new IdentificadorPedidoUnicoValidationRule(this._uow, this._empresa, this._cliente, this._pedido, productoId, cell.Value));

            return new GridValidationGroup
            {
                BreakValidationChain = true,
                Dependencies = { "CD_PRODUTO" },
                Rules = rules,
                OnSuccess = this.ValidateIdentificadorOnSuccess
            };
        }
        public virtual GridValidationGroup ValidateCantidadPedido(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            var codigoProducto = row.GetCell("CD_PRODUTO").Value;
            var identificador = row.GetCell("NU_IDENTIFICADOR").Value;
            var producto = _uow.ProductoRepository.GetProducto(_empresa, codigoProducto);

            if (producto == null)
                return null;

            var rules = new List<IValidationRule> {
                new NonNullValidationRule(cell.Value),
                new PositiveDecimalValidationRule(this._formatProvider, cell.Value),
                new DecimalLengthWithPresicionValidationRule(cell.Value, 12, 3, this._formatProvider, producto.AceptaDecimales),
            };

            if (!string.IsNullOrEmpty(identificador) && identificador != ManejoIdentificadorDb.IdentificadorAuto)
                rules.Add(new ManejoIdentificadorSerieCantidadValidationRule(cell.Value, producto.ManejoIdentificador, _formatProvider));

            return new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = rules,
            };
        }

        public virtual void ValidateProductoOnSuccess(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            var cdProducto = row.GetCell("CD_PRODUTO").Value;
            var producto = this._uow.ProductoRepository.GetProductoEmpresaAsignadaUsuario(this._empresa, cdProducto);

            if (!parameters.Any(s => s.Id == "importExcel"))
                row.GetCell("DS_PRODUTO").Value = producto.Descripcion;

            if (producto.IsIdentifiedByProducto())
                row.GetCell("NU_IDENTIFICADOR").Value = producto.ParseIdentificador(row.GetCell("NU_IDENTIFICADOR").Value);

        }
        public virtual void ValidateIdentificadorOnSuccess(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            var cdProducto = row.GetCell("CD_PRODUTO").Value;
            var producto = this._uow.ProductoRepository.GetProductoEmpresaAsignadaUsuario(this._empresa, cdProducto);

            cell.Value = producto.ParseIdentificador(cell.Value);
        }
    }
}
