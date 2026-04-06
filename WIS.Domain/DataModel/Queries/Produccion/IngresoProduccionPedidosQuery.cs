using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Produccion
{
	public class IngresoProduccionPedidosQuery : QueryObject<V_PRE100_PEDIDO_SAIDA, WISDB>
	{
		string _idIngreso;

		public IngresoProduccionPedidosQuery(string idIngreso)
		{
			_idIngreso = idIngreso;
		}

		public override void BuildQuery(WISDB context)
		{
			Query = context.V_PRE100_PEDIDO_SAIDA.Where(w => w.NU_PRDC_INGRESO == _idIngreso);
		}

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
