using System;
using System.Collections.Generic;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.General;
using WIS.Application.Validation.Rules.Preparacion;
using WIS.Application.Validation.Rules.Registro;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Picking.Dtos;
using WIS.GridComponent;
using WIS.GridComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.GridModules
{
    public class DetallePedidoLpnGridValidationModule : GridValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly int _empresa;
        protected readonly string _cliente;
        protected readonly string _pedido;
        protected readonly IFormatProvider _formatProvider;

        public DetallePedidoLpnGridValidationModule(IUnitOfWork uow, IFormatProvider formatProvider, int empresa, string cliente, string pedido)
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
                ["TP_LPN_TIPO"] = this.ValidateTipoLpn,
                ["ID_LPN_EXTERNO"] = this.ValidateIdExternoLpn,
                ["QT_PEDIDO"] = this.ValidateCantidadPedido
            };
        }

        public virtual GridValidationGroup ValidateProducto(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            return new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule> {
                    new NonNullValidationRule(cell.Value),
                    new StringMaxLengthValidationRule(cell.Value, 40),
                    new ProductoExistsValidationRule(this._uow, this._empresa.ToString(), cell.Value)
                },
                OnSuccess = ValidateProductoOnSuccess
            };
        }
        public virtual void ValidateProductoOnSuccess(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            var cdProducto = row.GetCell("CD_PRODUTO").Value;
            var producto = this._uow.ProductoRepository.GetProductoEmpresaAsignadaUsuario(this._empresa, cdProducto);

            row.GetCell("DS_PRODUTO").Value = producto.Descripcion;

            if (producto.IsIdentifiedByProducto())
                row.GetCell("NU_IDENTIFICADOR").Value = producto.ParseIdentificador(row.GetCell("NU_IDENTIFICADOR").Value);
            else if (row.GetCell("NU_IDENTIFICADOR").Value == ManejoIdentificadorDb.IdentificadorProducto)
                row.GetCell("NU_IDENTIFICADOR").Value = ManejoIdentificadorDb.IdentificadorAuto;
        }

        public virtual GridValidationGroup ValidateIdentificador(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            var cdProducto = row.GetCell("CD_PRODUTO").Value;

            if (string.IsNullOrEmpty(cdProducto))
                return null;

            var rules = new List<IValidationRule>();

            if (_uow.ProductoRepository.ExisteProducto(this._empresa, cdProducto))
            {
                var producto = this._uow.ProductoRepository.GetProductoEmpresaAsignadaUsuario(this._empresa, cdProducto);
                cell.Value = producto.ParseIdentificador(cell.Value);

                rules.Add(new StringMaxLengthValidationRule(cell.Value, 40));
                rules.Add(new IdentificadorProductoValidationRule(this._uow, this._empresa, cdProducto, cell.Value));
            }

            return new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = rules,
                OnSuccess = ValidateIdentificadorOnSuccess
            };
        }
        public virtual void ValidateIdentificadorOnSuccess(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            var cdProducto = row.GetCell("CD_PRODUTO").Value;
            var producto = this._uow.ProductoRepository.GetProductoEmpresaAsignadaUsuario(this._empresa, cdProducto);

            cell.Value = producto.ParseIdentificador(cell.Value);
        }

        public virtual GridValidationGroup ValidateTipoLpn(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            return new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(cell.Value),
                    new ExisteTipoLPNValidationRule(this._uow, cell.Value)
                }

            };
        }

        public virtual GridValidationGroup ValidateIdExternoLpn(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            return new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(cell.Value)
                }
            };
        }

        public virtual GridValidationGroup ValidateCantidadPedido(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(row.GetCell("CD_PRODUTO").Value) ||
                string.IsNullOrEmpty(row.GetCell("ID_LPN_EXTERNO").Value))
            {
                cell.Value = string.Empty;
                return null;
            }

            var identificador = string.IsNullOrEmpty(row.GetCell("NU_IDENTIFICADOR").Value) ? ManejoIdentificadorDb.IdentificadorAuto : row.GetCell("NU_IDENTIFICADOR").Value;

            var especificaIdentificador = !string.IsNullOrEmpty(row.GetCell("ID_ESPECIFICA_IDENTIFICADOR").Value) ? row.GetCell("ID_ESPECIFICA_IDENTIFICADOR").Value :
                 (identificador != ManejoIdentificadorDb.IdentificadorAuto ? "S" : "N");

            var producto = _uow.ProductoRepository.GetProducto(_empresa, row.GetCell("CD_PRODUTO").Value);

            var lpnEspecifico = new DetallePedidoLpnEspecifico()
            {
                Pedido = _pedido,
                Cliente = _cliente,
                Empresa = _empresa,
                Producto = row.GetCell("CD_PRODUTO").Value,
                Faixa = !string.IsNullOrEmpty(row.GetCell("CD_FAIXA").Value) ? decimal.Parse(row.GetCell("CD_FAIXA").Value, _formatProvider) : 1,
                Identificador = identificador,
                IdEspecificaIdentificador = especificaIdentificador,
                TipoLpn = row.GetCell("TP_LPN_TIPO").Value,
                IdExternoLpn = row.GetCell("ID_LPN_EXTERNO").Value,
                Cantidad = (!string.IsNullOrEmpty(cell.Value) && decimal.TryParse(cell.Value, _formatProvider, out decimal parsedValue)) ? decimal.Parse(cell.Value, _formatProvider) : 0,
                Update = !string.IsNullOrEmpty(cell.Old) && cell.Old != cell.Value,
                ManejoIdentificador = producto.ManejoIdentificador
            };

            var rules = new List<IValidationRule> {
                new NonNullValidationRule(cell.Value),
                new PositiveDecimalValidationRule(this._formatProvider, cell.Value,false),
                new DecimalLengthWithPresicionValidationRule(cell.Value, 12, 3, this._formatProvider),
                new CantidadPedidaLpnEspecificoValidationRule(_uow, lpnEspecifico)
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
