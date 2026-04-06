using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Stock
{
    public class STO210Query : QueryObject<V_STOCK_ENVASE, WISDB>
    {
        public STO210Query()
        {

        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_STOCK_ENVASE.AsNoTracking();
        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
