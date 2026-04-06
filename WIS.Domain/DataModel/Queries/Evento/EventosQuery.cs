using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Expedicion
{
    public class EventosQuery : QueryObject<V_EVENTO_WEVT010, WISDB>
    {
        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_EVENTO_WEVT010;
        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
