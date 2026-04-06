using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Automatizacion
{
    public class AutomatismoEjecucionQuery : QueryObject<V_AUT100_EJECUCIONES, WISDB>
    {
        protected readonly int? numeroAutomatismo;

        public AutomatismoEjecucionQuery(int? numeroAutomatismo = null)
        {
            this.numeroAutomatismo = numeroAutomatismo;
        }

        public override void BuildQuery(WISDB context)
        {
            if (numeroAutomatismo == null)
                this.Query = context.V_AUT100_EJECUCIONES.AsNoTracking();
            else
                this.Query = context.V_AUT100_EJECUCIONES.AsNoTracking().Where(i => i.NU_AUTOMATISMO == numeroAutomatismo);
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
