using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Porteria
{
    public class PorteriaAsociarAgendasVehiculoQuery : QueryObject<V_PORTERIA_ENTRADA_SIN_AGENDA, WISDB>
    {
        public PorteriaAsociarAgendasVehiculoQuery()
        {

        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_PORTERIA_ENTRADA_SIN_AGENDA.AsNoTracking();
        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
