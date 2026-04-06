using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Preparacion
{
    public class PrediosPickingQuery : QueryObject<V_PREDIOS_PICKING_PRE050, WISDB>
    {
        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_PREDIOS_PICKING_PRE050;
        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }

        public virtual List<string> PrediosPorPicking()
        {
            return  this.Query.GroupBy(x => x.NU_PREDIO).Select(s => s.Key).ToList();
        }
    }
}
