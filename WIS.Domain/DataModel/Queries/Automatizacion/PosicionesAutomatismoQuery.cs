using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Automatizacion
{
    public class PosicionesAutomatismoQuery : QueryObject<V_AUT100_POSICIONES, WISDB>
    {
        protected readonly int numeroAutomatismo;

        public PosicionesAutomatismoQuery(int numeroAutomatismo)
        {
            this.numeroAutomatismo = numeroAutomatismo;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_AUT100_POSICIONES.AsNoTracking().Where(i => i.NU_AUTOMATISMO == numeroAutomatismo);
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
