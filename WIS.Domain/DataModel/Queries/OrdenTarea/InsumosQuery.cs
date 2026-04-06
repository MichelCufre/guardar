using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.OrdenTarea
{
    public class InsumosQuery : QueryObject<V_ORT_INSUMOS_WORT020, WISDB>
    {
        public InsumosQuery()
        {
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_ORT_INSUMOS_WORT020;
        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
