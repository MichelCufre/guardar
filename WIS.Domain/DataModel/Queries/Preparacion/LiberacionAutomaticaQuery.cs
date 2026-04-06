using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Preparacion
{
    public class LiberacionAutomaticaQuery : QueryObject<V_PRE250_REGLA_LIBERACION, WISDB>
    {
        public LiberacionAutomaticaQuery()
        {
        }
        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_PRE250_REGLA_LIBERACION;
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
