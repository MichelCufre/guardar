using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Preparacion
{
    public class AvancesDePreparaciones : QueryObject<V_PRE160_PICKING_PENDIENTE, WISDB>
    {
        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_PRE160_PICKING_PENDIENTE.AsNoTracking();
        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
