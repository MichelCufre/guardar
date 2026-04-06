using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Stock
{
    public class StockPorProductoQuery : QueryObject<V_STO500_STOCK_POR_PRODUCTO, WISDB>
    {
        public StockPorProductoQuery()
        {
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_STO500_STOCK_POR_PRODUCTO.AsNoTracking();
        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
