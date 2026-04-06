using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Inventario
{
    public class INV060GridQuery : QueryObject<V_ASIGN_MOTIVO_AJUSTE_WINV060, WISDB>
    {
        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_ASIGN_MOTIVO_AJUSTE_WINV060;
        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
