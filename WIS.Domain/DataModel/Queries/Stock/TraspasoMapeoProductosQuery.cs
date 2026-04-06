using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Stock
{
    public class TraspasoMapeoProductosQuery : QueryObject<V_STO810_TRASP_MAPEO_PRODUTO, WISDB>
    {
        public TraspasoMapeoProductosQuery()
        {
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_STO810_TRASP_MAPEO_PRODUTO.AsNoTracking().Select(x => x);
        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
