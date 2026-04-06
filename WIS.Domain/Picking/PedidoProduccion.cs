using System;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Expedicion;
using WIS.Domain.General;
using WIS.Domain.Produccion;

namespace WIS.Domain.Picking
{
	public class PedidoProduccion : Pedido
	{
		public PedidoProduccion () : base ()
		{
			this.Tipo = TipoPedidoDb.Produccion;
		}

		public PedidoProduccion (string id) : base ()
		{
			this.Tipo = TipoPedidoDb.Produccion;
			this.Id   = id;
		}
	}
}
