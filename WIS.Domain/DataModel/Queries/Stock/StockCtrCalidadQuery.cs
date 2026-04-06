using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Stock
{
    public class StockCtrCalidadQuery : QueryObject<V_STO060_CTR_CALIDAD, WISDB>
    {
        public StockCtrCalidadQuery()
        {
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_STO060_CTR_CALIDAD.AsNoTracking().Select(s => s);
        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
