using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Preparacion
{
    public class PRE350StockPickingReabastQuery : QueryObject<V_PRE350_STOCK_PICKING_REABAST, WISDB>
    {
        public PRE350StockPickingReabastQuery()
        {}

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_PRE350_STOCK_PICKING_REABAST.AsNoTracking();
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
