using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Expedicion
{
    public class InstanciasEventosQuery : QueryObject<V_EVENTO_INSTANCIA_WEVT040, WISDB>
    {
        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_EVENTO_INSTANCIA_WEVT040;
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no está lista para hacer conteo");

            return this.Query.Count();
        }

    }
}
