using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Expedicion
{
    public class GruposContactos : QueryObject<V_GRUPOS_WEVT030, WISDB>
    {
        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_GRUPOS_WEVT030;
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
