using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Impresion
{
    public class ColaImpresionQuery : QueryObject<V_LIMP010_IMPRESION, WISDB>
    {
        public ColaImpresionQuery()
        {
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_LIMP010_IMPRESION.AsNoTracking();
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
