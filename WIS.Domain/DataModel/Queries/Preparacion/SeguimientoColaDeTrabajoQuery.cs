using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Preparacion
{
    public class SeguimientoColaDeTrabajoQuery : QueryObject<V_SEG_COLA_TRABAJO_PRE812, WISDB>
    {
        public SeguimientoColaDeTrabajoQuery() { }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_SEG_COLA_TRABAJO_PRE812;
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
