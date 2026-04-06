using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Porteria
{
    public class PorteriaAgendasVehiculoQuery : QueryObject<V_POTERIA_VEHICULO_AGENDA, WISDB>
    {
        public PorteriaAgendasVehiculoQuery()
        {

        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_POTERIA_VEHICULO_AGENDA.AsNoTracking();
        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
