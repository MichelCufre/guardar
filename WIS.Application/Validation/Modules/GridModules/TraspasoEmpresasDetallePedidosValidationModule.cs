using System.Collections.Generic;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.Expedicion;
using WIS.Application.Validation.Rules.Preparacion;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.Picking;
using WIS.GridComponent;
using WIS.GridComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.GridModules
{
    public class TraspasoEmpresasDetallePedidosValidationModule : GridValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly ConfiguracionPedido _configuracion;
        protected readonly string _confPedidoDestino;

        public TraspasoEmpresasDetallePedidosValidationModule(IUnitOfWork uow, ConfiguracionPedido configuracion, string confPedidoDestino)
        {
            this._uow = uow;
            this._configuracion = configuracion;
            this._confPedidoDestino = confPedidoDestino;

            this.Schema = new GridValidationSchema
            {
                ["NU_PEDIDO_DESTINO"] = this.ValidatePedidoDestino,
                ["CD_CLIENTE_DESTINO"] = this.ValidateClienteDestino,
                ["TP_EXPEDICION_DESTINO"] = this.ValidateTipoExpedicionDestino,
                ["TP_PEDIDO_DESTINO"] = this.ValidateTipoPedidoDestino,
            };
        }

        public virtual GridValidationGroup ValidatePedidoDestino(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(cell.Value))
                return null;

            var empresa = row.GetCell("CD_EMPRESA_DESTINO")?.Value;
            var cliente = row.GetCell("CD_CLIENTE_DESTINO")?.Value;

            cell.Value = cell.Value.Trim().ToUpper();

            var rules = new List<IValidationRule>
            {
                new StringMaxLengthValidationRule(cell.Value, 40),
                new NoExistePedidoValidationRule(this._uow, cell.Value, cliente, empresa)
            };

            if (this._configuracion.PedidoDebeSerNumerico)
                rules.Add(new PositiveIntValidationRule(cell.Value));

            return new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = rules,
                Dependencies = { "CD_CLIENTE_DESTINO" }
            };
        }

        public virtual GridValidationGroup ValidateClienteDestino(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            var empresa = row.GetCell("CD_EMPRESA_DESTINO")?.Value;

            var rules = new List<IValidationRule>
            {
                new StringMaxLengthValidationRule(cell.Value, 10),
            };

            if (!string.IsNullOrEmpty(cell.Value) && !string.IsNullOrEmpty(empresa))
                rules.Add(new ExisteClienteValidationRule(this._uow, cell.Value, empresa));

            return new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = rules,
            };
        }

        public virtual GridValidationGroup ValidateTipoExpedicionDestino(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            return new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                   new NonNullValidationRule(cell.Value),
                   new StringMaxLengthValidationRule(cell.Value, 6),
                   new ExisteTipoExpedicionValidationRule(this._uow, cell.Value)
                }
            };
        }

        public virtual GridValidationGroup ValidateTipoPedidoDestino(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            string tipoExpedicion = row.GetCell("TP_EXPEDICION_DESTINO").Value;

            return new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                   new NonNullValidationRule(cell.Value),
                   new StringMaxLengthValidationRule(cell.Value, 6),
                   new TipoPedidoCompatibilidadValidationRule(this._uow, cell.Value, tipoExpedicion)
                },
                Dependencies = { "TP_EXPEDICION_DESTINO" }
            };
        }
    }
}