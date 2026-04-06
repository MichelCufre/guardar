using System;
using System.Linq;
using WIS.Data;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Preparacion
{
    public class PedidosPanelQuery : QueryObject<V_PRE100_PEDIDO_SAIDA, WISDB>
    {
		protected readonly bool _pedidosActivos;
		public PedidosPanelQuery()
		{

		}

		public PedidosPanelQuery(bool pedidosActivos)
        {
            _pedidosActivos = pedidosActivos;
		}

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_PRE100_PEDIDO_SAIDA;

            if(_pedidosActivos)
				this.Query = this.Query.Where(p => p.ND_ACTIVIDAD == EstadoPedidoDb.Activo || p.ND_ACTIVIDAD == null);
		}
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
