using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Configuracion
{
    public class IntegracionServiciosQuery : QueryObject<V_COF110_SERVICIOS_INTEGRACION, WISDB>
    {
        public IntegracionServiciosQuery()
        {
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_COF110_SERVICIOS_INTEGRACION.AsNoTracking();
        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
