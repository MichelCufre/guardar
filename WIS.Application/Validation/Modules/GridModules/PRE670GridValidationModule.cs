using System;
using System.Collections.Generic;
using WIS.Application.Validation.Rules;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.GridComponent;
using WIS.GridComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.GridModules
{
	public class PRE670GridValidationModule : GridValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly IFormatProvider _culture;

        public PRE670GridValidationModule(IUnitOfWork uow, IFormatProvider culture)
        {
            this._uow = uow;
            this._culture = culture;

            this.Schema = new GridValidationSchema
            {
                ["QT_PEDIDO"] = this.GridValidateQTPedido
            };
        }

        public virtual GridValidationGroup GridValidateQTPedido(GridCell cell, GridRow row, List<ComponentParameter> parameters)
        {
            return new GridValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new PositiveDecimalValidationRule( this._culture,cell.Value),
                    new PuedeModicarCantPedidoValidationRule(cell.Value ,row.GetCell("QT_LIBERADO").Value,row.GetCell("QT_ANULADO").Value,row.GetCell("QT_PEDIDO_ORIGINAL").Value,_culture),
                    new PuedeActualizarDetallePedidoValidationRule (row.GetCell("NU_PEDIDO").Value, row.GetCell("CD_EMPRESA").Value,row.GetCell("QT_LIBERADO").Value ,row.GetCell("QT_ANULADO").Value, row.GetCell("CD_CLIENTE").Value,
                        row.GetCell("CD_PRODUTO").Value, row.GetCell("NU_IDENTIFICADOR").Value, row.GetCell("CD_FAIXA").Value,row.GetCell("ID_ESPECIFICA_IDENTIFICADOR").Value,this._uow, this._culture)
                },
            };
        }
    }
}
