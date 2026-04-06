using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Impresion
{
    public class ImpresorasQuery : QueryObject<V_COF040_IMPRESORAS, WISDB>
    {
        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_COF040_IMPRESORAS.AsNoTracking();
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
