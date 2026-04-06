using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.OrdenTarea
{
    public class OrdenTrabajoQuery : QueryObject<V_ORT_ORDEN_WORT030, WISDB>
    {
        public OrdenTrabajoQuery()
        {
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_ORT_ORDEN_WORT030;
        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
