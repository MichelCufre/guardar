using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Produccion
{
    public class LineaProduccionQuery : QueryObject<V_PRDC_LINEA_KIT190, WISDB>
    {
        public LineaProduccionQuery()
        {
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_PRDC_LINEA_KIT190
                .Select(d => d);
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
