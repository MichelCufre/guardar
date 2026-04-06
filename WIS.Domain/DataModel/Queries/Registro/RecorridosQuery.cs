using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Registro
{
    public class RecorridosQuery : QueryObject<V_REG700_RECORRIDOS, WISDB>
    {
        public RecorridosQuery()
        {
        }

        public override void BuildQuery(WISDB context)
        {
            Query = context.V_REG700_RECORRIDOS.Select(d => d);
        }

        public virtual int GetCount()
        {
            if (Query == null) throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return Query.Count();
        }
    }
}
