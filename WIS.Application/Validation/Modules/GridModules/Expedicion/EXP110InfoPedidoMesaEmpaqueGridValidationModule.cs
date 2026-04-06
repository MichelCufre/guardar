using System.Collections.Generic;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.Expedicion;
using WIS.Application.Validation.Rules.Preparacion;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.GridComponent;
using WIS.GridComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.GridModules.Expedicion
{
    public class EXP110InfoPedidoMesaEmpaqueGridValidationModule : GridValidationModule
    {
        protected readonly IUnitOfWork _uow;

        public EXP110InfoPedidoMesaEmpaqueGridValidationModule(IUnitOfWork uow)
        {
            _uow = uow;

            this.Schema = new GridValidationSchema
            {
                ["TP_EXPEDICION"] = this.ValidateTipoExpedicion
            };
        }
        public virtual GridValidationGroup ValidateTipoExpedicion(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            var tpPedido = row.GetCell("TP_PEDIDO").Value;

            return new GridValidationGroup
            {
                Rules = new List<IValidationRule> {
                    new NonNullValidationRule(cell.Value),
                    new StringMaxLengthValidationRule(cell.Value, 6),
                    new ExisteTipoExpedicionValidationRule(this._uow, cell.Value),
                    new TipoPedidoCompatibilidadValidationRule(this._uow, tpPedido, cell.Value)
                },
            };
        }

    }
}
