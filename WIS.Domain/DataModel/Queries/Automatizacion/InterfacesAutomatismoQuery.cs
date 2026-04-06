using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Automatizacion
{
    public class InterfacesAutomatismoQuery : QueryObject<V_AUT100_INTERFACES, WISDB>
    {
        protected readonly int numeroAutomatismo;

        public InterfacesAutomatismoQuery(int numeroAutomatismo)
        {
            this.numeroAutomatismo = numeroAutomatismo;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_AUT100_INTERFACES.AsNoTracking().Where(i => i.NU_AUTOMATISMO == numeroAutomatismo);
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
