using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Registro
{
    public class ZonaQuery : QueryObject<V_REG104_ZONA, WISDB>
    {
        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_REG104_ZONA.AsNoTracking();
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
