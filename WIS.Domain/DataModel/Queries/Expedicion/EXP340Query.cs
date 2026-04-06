using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Expedicion
{
    public class EXP340Query : QueryObject<V_RETORNO_BULTO_EXP340, WISDB>
    {
        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_RETORNO_BULTO_EXP340.AsNoTracking();
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
