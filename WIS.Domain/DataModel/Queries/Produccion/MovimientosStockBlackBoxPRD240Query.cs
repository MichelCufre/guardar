using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Produccion
{
    public class MovimientosStockBlackBoxPRD240Query : QueryObject<V_KIT240_MOVIMIENTOS_STOCK_BB, WISDB>
    {
        public MovimientosStockBlackBoxPRD240Query()
        {
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_KIT240_MOVIMIENTOS_STOCK_BB;
        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
